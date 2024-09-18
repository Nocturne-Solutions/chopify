using chopify.Data.Repositories.Interfaces;
using chopify.Helpers;

namespace chopify.Configurations
{
    public class GarbashCollectorsConfig
    {
        private readonly ILogger<GarbashCollectorsConfig> _logger;

        public GarbashCollectorsConfig(Scheduler scheduler, ICooldownRepository cooldownRepository, ILogger<GarbashCollectorsConfig> logger)
        {
            _logger = logger;

            scheduler.AddPeriodicTask(async () =>
            {
                try
                {
                    _logger.LogInformation("Running cooldown garbage collector...");
                    await cooldownRepository.DeleteAllExpiredAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while running cooldown garbage collector.");
                }
            }, TimeSpan.FromHours(1));
        }
    }
}
