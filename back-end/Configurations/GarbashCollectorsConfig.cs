using chopify.Data.Repositories.Implementations;
using chopify.Data.Repositories.Interfaces;
using chopify.Helpers;

namespace chopify.Configurations
{
    public class GarbashCollectorsConfig
    {
        private readonly IWinnerRepository _winnerRepository;
        private readonly ICooldownRepository _cooldownRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<GarbashCollectorsConfig> _logger;

        public GarbashCollectorsConfig(Scheduler scheduler, ICooldownRepository cooldownRepository, IWinnerRepository winnerRepository, IUserRepository userRepository, ILogger<GarbashCollectorsConfig> logger)
        {
            _logger = logger;
            _cooldownRepository = cooldownRepository;
            _winnerRepository = winnerRepository;
            _userRepository = userRepository;

            scheduler.AddPeriodicTask(async () => await ClearCooldowns(), TimeSpan.FromHours(1));
            scheduler.AddPeriodicTask(async () => await ClearWinners(), TimeSpan.FromHours(12));
            scheduler.AddPeriodicTask(async () => await ClearUsers(), TimeSpan.FromHours(12));
        }

        public async Task ClearCooldowns()
        {
            try
            {
                _logger.LogInformation("Running cooldown garbage collector...");
                await _cooldownRepository.DeleteAllExpiredAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while running cooldown garbage collector.");
            }
        }

        public async Task ClearWinners()
        {
            try
            {
                _logger.LogInformation("Running winner garbage collector...");
                await _winnerRepository.DeleteAllExpiredAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while running winner garbage collector.");
            }
        }

        public async Task ClearUsers()
        {
            try
            {
                _logger.LogInformation("Running user garbage collector...");
                await _userRepository.DeleteAllExpiredAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while running user garbage collector.");
            }
        }
    }
}
