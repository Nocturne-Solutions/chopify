using AutoMapper;
using chopify.Data.Entities;
using chopify.Data.Repositories.Interfaces;
using chopify.External;
using chopify.Helpers;
using chopify.Models;
using chopify.Services.Interfaces;

namespace chopify.Services.Implementations
{
    public class VotingSystemService(ILogger<VotingSystemService> logger, IMapper mapper, ISuggestionRepository suggestionRepository, IVoteRepository voteRepository, IWinnerRepository winnerRepository) : IVotingSystemService
    {
        private static readonly AsyncReaderWriterLock _lock = new();
        private static readonly AsyncReaderWriterLock _servicesLock = new();

        private static bool _isActive = false;
        private static bool _roundInProgress = false;
        private static int _currentRound = 0;
        private static DateTime? _stateEndTime;
        private static CancellationTokenSource? _roundCancellationTokenSource;

        private readonly ILogger<VotingSystemService> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly ISuggestionRepository _suggestionRepository = suggestionRepository;
        private readonly IVoteRepository _voteRepository = voteRepository;
        private readonly IWinnerRepository _winnerRepository = winnerRepository;

        public async Task<IVotingSystemService.ResultCode> Start()
        {
            await _lock.LockWriteAsync();

            try
            {
                if (_isActive)
                    return IVotingSystemService.ResultCode.IsActive;

                _isActive = true;

                Winner winner = await TakeRandomWinner();

                await _winnerRepository.CreateAsync(winner);

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

                _roundInProgress = false;

                await _servicesLock.LockWriteAsync();

                try
                {
                    _stateEndTime = null;
                    _isActive = false;

                    return IVotingSystemService.ResultCode.Success;
                }
                finally
                {
                    _servicesLock.UnlockWrite();
                }        
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

                _roundInProgress = false;

                await _servicesLock.LockWriteAsync();

                try
                {
                    _isActive = false;
                    _stateEndTime = null;
                    _currentRound = 0;

                    await _voteRepository.DeleteAllAsync();
                    await _suggestionRepository.DeleteAllAsync();

                    return IVotingSystemService.ResultCode.Success;
                }
                finally
                {
                    _servicesLock.UnlockWrite();
                }
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

        private async Task RoundHandler(TimeSpan duration)
        {
            _roundCancellationTokenSource = new CancellationTokenSource();

            try
            {
                await Task.Delay(duration, _roundCancellationTokenSource.Token);
            }
            finally
            {
                await _lock.LockWriteAsync();

                _roundInProgress = false;

                await _servicesLock.LockWriteAsync();

                try
                {
                    if (_roundCancellationTokenSource.IsCancellationRequested)
                        await CancelVotingRound();
                    else
                    {
                        _roundCancellationTokenSource = new CancellationTokenSource();

                        var winner = await EndVotingRound();

                        _ = Task.Run(async () =>
                        {
                            try
                            {
                                _stateEndTime = DateTime.UtcNow.Add(TimeSpan.FromSeconds(30));

                                await Task.Delay(TimeSpan.FromSeconds(30), _roundCancellationTokenSource.Token);

                                await _lock.LockWriteAsync();

                                try
                                {
                                    await StartVotingRound(winner.Duration - TimeSpan.FromSeconds(30));
                                }
                                finally
                                {
                                    _lock.UnlockWrite();
                                }
                            }
                            catch (OperationCanceledException) { }
                        });
                    }
                }
                finally
                {
                    _servicesLock.UnlockWrite();
                    _lock.UnlockWrite();
                }
            }
        }

        private async Task StartVotingRound(TimeSpan duration)
        {
            _currentRound = await _winnerRepository.GetLastRoundNumberAsync() + 1;
            _roundInProgress = true;
            _stateEndTime = DateTime.UtcNow.Add(duration);

            _logger.LogInformation($"Comenzando la ronda {_currentRound} con duración de {duration.TotalSeconds} segundos.");

            _ = Task.Run(async () => await RoundHandler(duration));
        }

        private async Task<Winner> EndVotingRound()
        {
            _logger.LogInformation($"Finalizando la ronda {_currentRound}.");

            var topN = await _suggestionRepository.GetTopNAsync(10);

            if (topN == null || !topN.Any())
            {
                var random = await TakeRandomWinner();

                await _winnerRepository.CreateAsync(random);

                return random;
            }

            var winner = _mapper.Map<Winner>(topN.First());

            winner.RoundNumber = _currentRound;

            await _winnerRepository.CreateAsync(winner);

            _logger.LogInformation($"Canción ganadora de la ronda {_currentRound}: {winner.Name} - {winner.Artist}.");

            var excepted = topN.Skip(1).Select(s => s.SpotifySongId);

            await _voteRepository.DeleteAllExceptAsync(excepted);
            await _suggestionRepository.DeleteAllExceptAsync(excepted);

            return winner;
        }

        private async Task CancelVotingRound()
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

        private async Task<Winner> TakeRandomWinner()
        {
            SongReadDTO? randomWinner;

            do
            {
                randomWinner = _mapper.Map<SongReadDTO>(await SpotifyService.Instance.GetRandomTrackAsync());
            } while (randomWinner == null || await _winnerRepository.GetBySongIdAsync(randomWinner.Id) != null);

            var winner = _mapper.Map<Winner>(randomWinner);

            winner.SuggestedBy = "Sistema";
            winner.RoundNumber = _currentRound == 0 ? await _winnerRepository.GetLastRoundNumberAsync() + 1 : _currentRound;
            winner.Votes = 0;

            return winner;
        }
    }
}
