using Microsoft.Extensions.Options;
using Utils.IMAP;
using Utils.Validate;

namespace MailVault.Worker;

public class Worker(IOptions<AppConfig> options, ILogger<Worker> logger) : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromHours(6);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("MailVault service started. Backup interval: {Interval}h.", 6);

        // Run once immediately on start, then wait for the interval
        while (!stoppingToken.IsCancellationRequested)
        {
            await RunBackupCycleAsync(stoppingToken);

            logger.LogInformation("Next backup in {Hours} hours at {Time:HH:mm} UTC.",
                6, DateTime.UtcNow.Add(Interval));

            try
            {
                await Task.Delay(Interval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Service is stopping — exit cleanly
                break;
            }
        }
    }

    private async Task RunBackupCycleAsync(CancellationToken cancellationToken)
    {
        AppConfig? cfg = options.Value;

        logger.LogInformation("=== Backup cycle starting ===");

        try
        {
            var backupRoot = new DirectoryInfo(cfg.BackupDir);

            GitHub Syncorizer = new GitHub(cfg.Token, cfg.Owner, cfg.Email, logger);

            Syncorizer.CloneIfMissing(cfg.GitHubUrl, cfg.BackupDir);

            var imap = new Imap(
                logger,
                cfg.ImapHost,
                cfg.ImapPort,
                cfg.ImapUser,
                cfg.ImapPass,
                cfg.Mailboxes);

            int newMail = await imap.BackupAllAsync(backupRoot, cancellationToken);

            if (newMail > 0)
            {
                if (!await ValidationHelper.HasInternetConnectionAsync())
                {
                    logger.LogWarning("No internet connection. Aborting github synchronization.");
                }
                else
                {
                    await Syncorizer.SyncAsync(cfg.GitHubUrl, cfg.BackupDir, $"backup: +{newMail} emails ({DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm UTC")})");
                }

            }
            else
            {
                logger.LogInformation("No new emails — nothing to push.");
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Backup cycle failed: {Message}", ex.Message);
        }

        logger.LogInformation("=== Backup cycle complete ===");
    }
}
