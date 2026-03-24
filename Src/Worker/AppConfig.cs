namespace MailVault.Worker;

/// <summary>
/// Strongly-typed configuration.
/// Bound from the "MailVault" section in appsettings.json,
/// which can be overridden by environment variables or .env.
/// </summary>
public sealed class AppConfig
{
    /// <summary>IMAP server hostname (e.g. imap.gmail.com).</summary>
    public string ImapHost { get; set; } = "";

    /// <summary>IMAP port. 993 = SSL, 143 = STARTTLS.</summary>
    public int ImapPort { get; set; } = 993;

    /// <summary>Email address used to log in.</summary>
    public string ImapUser { get; set; } = "";

    /// <summary>Password or App Password.</summary>
    public string ImapPass { get; set; } = "";

    /// <summary>GitHub repo URL with embedded PAT: https://TOKEN@github.com/user/MailVault.git</summary>
    public string GitHubUrl { get; set; } = "";

    /// <summary>Local path where the repo is cloned and .eml files are stored.</summary>
    public string BackupDir { get; set; } = "./mailvault_backup";

    /// <summary>IMAP folder names to back up.</summary>
    public List<string> Mailboxes { get; set; } = ["INBOX"];

    /// <summary>How often to run the backup (hours). Default: 6.</summary>
    public int IntervalHours { get; set; } = 6;

    public string Owner { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Token { get; set; } = null!;
}
