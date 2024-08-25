using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SecureChat.Data;

namespace SecureChat.Services
{
    public class SessionCleanupService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<SessionCleanupService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(5);
        
        public SessionCleanupService(
            IServiceProvider services,
            ILogger<SessionCleanupService> logger)
        {
            _services = services;
            _logger = logger;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Session Cleanup Service is starting");
            
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Session cleanup check running");
                
                try
                {
                    await CleanupInactiveSessions(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during session cleanup");
                }
                
            try
            {
                await Task.Delay(_checkInterval, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                // This is expected during shutdown, no need to throw or log an error
                _logger.LogInformation("Session cleanup service is shutting down");
                break;
            }
            }
        }
        
        private async Task CleanupInactiveSessions(CancellationToken cancellationToken = default)
        {
            using var scope = _services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<SecureChatContext>();
            
            // Find sessions inactive for 30 minutes or more
            var cutoffTime = DateTime.UtcNow.AddMinutes(-30);
            var inactiveSessions = await context.ChatSessions
                .Where(s => s.LastActivity < cutoffTime)
                .ToListAsync(cancellationToken);
                
            if (inactiveSessions.Any())
            {
                _logger.LogInformation("Removing {Count} inactive sessions", inactiveSessions.Count);
                context.ChatSessions.RemoveRange(inactiveSessions);
                await context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}