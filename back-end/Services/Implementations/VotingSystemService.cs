using AutoMapper;
using chopify.Data.Entities;
using chopify.Data.Repositories.Interfaces;
using chopify.External;
using chopify.Helpers;
using chopify.Models;
using chopify.Services.Interfaces;

namespace chopify.Services.Implementations
{
    public class VotingSystemService(ILogger<VotingSystemService> logger, Scheduler scheduler, IMapper mapper, ISuggestionRepository suggestionRepository, IVoteRepository voteRepository, IWinnerRepository winnerRepository, ICooldownRepository cooldownRepository) : IVotingSystemService
    {
        private static readonly AsyncReaderWriterLock _lock = new();
        private static readonly AsyncReaderWriterLock _servicesLock = new();

        private static readonly TimeSpan _cooldownDuration = TimeSpan.FromHours(1);
        private static bool _isActive = false;
        private static bool _roundInProgress = false;
        private static int _currentRound = 0;
        private static DateTime? _stateEndTime;
        private static Guid _roundTask;
        private static Guid _roundCooldownTask;

        private readonly ILogger<VotingSystemService> _logger = logger;
        private readonly Scheduler _scheduler = scheduler;
        private readonly IMapper _mapper = mapper;
        private readonly ISuggestionRepository _suggestionRepository = suggestionRepository;
        private readonly IVoteRepository _voteRepository = voteRepository;
        private readonly IWinnerRepository _winnerRepository = winnerRepository;
        private readonly ICooldownRepository _cooldownRepository = cooldownRepository;

        public async Task<IVotingSystemService.ResultCode> Start()
        {
            await _lock.LockWriteAsync();

            try
            {
                if (_isActive)
                    return IVotingSystemService.ResultCode.IsActive;

                _isActive = true;

                Winner? winner = await TakeRandomWinner();

                if (winner == null)
                    return IVotingSystemService.ResultCode.FailToGetFirstSong;

                await RegisterWinner(winner);

                await StartVotingRound(winner.Duration - TimeSpan.FromSeconds(30));

                return IVotingSystemService.ResultCode.Success;
            }
            finally
            {
                _lock.UnlockWrite();
            }
        }

        public async Task<IVotingSystemService.ResultCode> Stop()
        {
            await _lock.LockWriteAsync();

            try
            {
                if (!_isActive)
                    return IVotingSystemService.ResultCode.IsNotActive;

                _scheduler.CancelTask(_roundTask);
                _scheduler.CancelTask(_roundCooldownTask);

                _roundInProgress = false;
                _stateEndTime = null;
                _isActive = false;

                return IVotingSystemService.ResultCode.Success;
            }
            finally
            {
                _lock.UnlockWrite();
            }
        }

        public async Task<IVotingSystemService.ResultCode> Reset()
        {
            await _lock.LockWriteAsync();

            try
            {
                if (_isActive)
                    return IVotingSystemService.ResultCode.IsActive;

                await _voteRepository.DeleteAllAsync();
                await _suggestionRepository.DeleteAllAsync();
                await _cooldownRepository.DeleteAllAsync();

                return IVotingSystemService.ResultCode.Success;
            }
            finally
            {
                _lock.UnlockWrite();
            }
        }

        public async Task<bool> Lock()
        {
            await _lock.LockReadAsync();

            try
            {
                if(_roundInProgress)
                {
                    await _servicesLock.LockReadAsync();
                    return true;
                }

                return false;
            }
            finally
            {
                await _lock.UnlockReadAsync();
            }   
        }

        public async Task Unlock()
        {
            await _servicesLock.UnlockReadAsync();
        }

        public async Task<VotingSystemStatusDTO> GetStatus()
        {
            await _lock.LockReadAsync();

            try
            {
                double? remainingTime;

                if (_stateEndTime == null || _stateEndTime < DateTime.UtcNow)
                    remainingTime = null;
                else
                    remainingTime = (_stateEndTime - DateTime.UtcNow)?.TotalSeconds;

                return new VotingSystemStatusDTO
                {
                    State = _roundInProgress ? VotingSystemStatusDTO.States.RoundInProgress : (_isActive ? VotingSystemStatusDTO.States.InBetweenRounds : VotingSystemStatusDTO.States.Stoped),
                    CurrentStateRemainingTimeSeconds = remainingTime
                };
            }
            finally
            {
                await _lock.UnlockReadAsync();
            }
        }

        public async Task<int> GetCurrentRoundNumber()
        {
            await _lock.LockReadAsync();

            try
            {
                if (!_roundInProgress)
                    throw new InvalidOperationException("No hay una ronda en curso.");

                return _currentRound;
            }
            finally
            {
                await _lock.UnlockReadAsync();
            }
        }

        private async Task StartVotingRound(TimeSpan duration)
        {
            _currentRound = await _winnerRepository.GetLastRoundNumberAsync() + 1;
            _roundInProgress = true;
            _stateEndTime = DateTime.UtcNow.Add(duration);

            _logger.LogInformation($"Comenzando la ronda {_currentRound} con duración de {duration.TotalSeconds} segundos.");

            _roundTask = _scheduler.AddTask(async () => await RoundEnding(), duration, async () => await RoundCancel());
        }

        private async Task RoundEnding()
        {
            await _lock.LockWriteAsync();

            _roundInProgress = false;

            await _servicesLock.LockWriteAsync();

            try
            {
                var winnerDuration = await EndVotingRound();

                _stateEndTime = DateTime.UtcNow.Add(TimeSpan.FromSeconds(30));

                _roundCooldownTask = _scheduler.AddTask(async () => await RoundCooldownEnd(winnerDuration - TimeSpan.FromSeconds(30)), TimeSpan.FromSeconds(30));
            }
            finally
            {
                _servicesLock.UnlockWrite();
                _lock.UnlockWrite();
            }
        }

        private async Task RoundCooldownEnd(TimeSpan nextRoundDuration)
        {
            await _lock.LockWriteAsync();

            try
            {
                await StartVotingRound(nextRoundDuration);
            }
            finally
            {
                _lock.UnlockWrite();
            }
        }

        private async Task RoundCancel()
        {
            _logger.LogInformation($"La ronda {_currentRound} fue cancelada.");

            var votes = await _voteRepository.GetByRoundAsync(_currentRound);

            Dictionary<string, int> votesCount = votes
                .GroupBy(v => v.SpotifySongId)
                .ToDictionary(g => g.Key, g => -g.Count());

            await _suggestionRepository.AddOrRemoveVotesAsync(votesCount);
            await _voteRepository.DeleteAllByRoundAsync(_currentRound);
            await _suggestionRepository.DeleteAllByRoundAsync(_currentRound);
        }

        private async Task<TimeSpan> EndVotingRound()
        {
            _logger.LogInformation($"Finalizando la ronda {_currentRound}.");

            var topN = await _suggestionRepository.GetTopNAsync(10);

            if (topN == null || !topN.Any())
            {
                var random = await TakeRandomWinner();

                await RegisterWinner(random);

                return random.Duration;
            }

            var winner = _mapper.Map<Winner>(topN.First());

            winner.RoundNumber = _currentRound;

            await RegisterWinner(winner);

            _logger.LogInformation($"Canción ganadora de la ronda {_currentRound}: {winner.Name} - {winner.Artist}.");

            var excepted = topN.Skip(1).Select(s => s.SpotifySongId);

            await _voteRepository.DeleteAllExceptAsync(excepted);
            await _suggestionRepository.DeleteAllExceptAsync(excepted);

            return winner.Duration;
        }

        private async Task<Winner?> TakeRandomWinner()
        {
            SongReadDTO? randomWinner;

            do
            {
                randomWinner = _mapper.Map<SongReadDTO>(await SpotifyService.Instance.GetRandomTrackAsync());
            } while (randomWinner == null || await _cooldownRepository.GetBySongIdAsync(randomWinner.Id) != null);

            var winner = _mapper.Map<Winner>(randomWinner);

            winner.SuggestedBy = "Sistema";
            winner.RoundNumber = _currentRound == 0 ? await _winnerRepository.GetLastRoundNumberAsync() + 1 : _currentRound;
            winner.Votes = 0;

            return winner;
        }

        private async Task RegisterWinner(Winner winner)
        {
            await _winnerRepository.CreateAsync(winner);
            await _cooldownRepository.CreateAsync(new Cooldown
            {
                SpotifySongId = winner.SpotifySongId,
                CooldownEnd = DateTime.UtcNow.Add(_cooldownDuration)
            });
        }
    }
}
