# MailVault

MailVault is a background email backup worker that pulls messages from IMAP folders and stores them in a GitHub-backed archive.

The service:

- runs continuously as a .NET worker,
- checks mail on a fixed interval (currently every 6 hours),
- saves new messages into a local backup repository,
- commits and pushes changes to GitHub when new mail is found.

## What This Project Does

MailVault is designed for long-running, unattended backup of email accounts.

Main flow per cycle:

1. Ensure the local backup repository exists (`clone if missing`).
2. Connect to IMAP with configured credentials.
3. Download new messages from configured folders.
4. If new mail exists and internet is available, commit and push to GitHub.

## Tech Stack

- `.NET 10` Worker Service
- `Microsoft.Extensions.Hosting.WindowsServices` for Windows service hosting
- Shared utilities project: `CS_Utilities`

> Note: IMAP and GitHub sync logic are implemented in the shared utilities dependency referenced by this project.

## External Dependency

This project references the `CS_Utilities` repository:

- [JooScript/CS_Utilities](https://github.com/JooScript/CS_Utilities)

## Configuration (`appsettings.json`)

Settings are read from the `MailVault` section in `Src/Worker/appsettings.json`.

### Copy This Template

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "MailVault": {
    "ImapHost": "imap.gmail.com",
    "ImapPort": 993,
    "ImapUser": "your-email@gmail.com",
    "ImapPass": "your-app-password",
    "GitHubUrl": "https://github.com/your-user/your-backup-repo.git",
    "BackupDir": "C:/MailVaultBackup",
    "Mailboxes": [
      "INBOX"
    ],
    "IntervalHours": 6,
    "Owner": "your-github-user",
    "Email": "your-email@gmail.com",
    "Token": "your-github-token"
  }
}
```

### Setting Reference

| Key             | Description                                                    |
|-----------------|----------------------------------------------------------------|
| `ImapHost`      | IMAP server host (example: `imap.gmail.com`)                   |
| `ImapPort`      | IMAP port (`993` SSL, `143` STARTTLS)                          |
| `ImapUser`      | Email/username for IMAP login                                  |
| `ImapPass`      | Password or app password                                       |
| `GitHubUrl`     | Git remote URL for backups                                     |
| `BackupDir`     | Local path where backup repo is stored                         |
| `Mailboxes`     | Mail folder list (array)                                       |
| `IntervalHours` | Backup interval in hours (currently fixed to 6 in worker code) |
| `Owner`         | GitHub account/owner used by sync utility                      |
| `Email`         | Commit author email for backup commits                         |
| `Token`         | GitHub token used for authenticated sync                       |

## Local Development

### Prerequisites

- .NET SDK 10+
- Access to the referenced utilities project at `Utilities/CS_Utilities`
- IMAP credentials for the source mailbox
- GitHub repository and token for backup sync

### Run

```powershell
cd .\Src\Worker
dotnet restore
dotnet run
```

## Build

From `Src/Worker`:

```powershell
dotnet publish -c Release -r win-x64 --self-contained false -o .\publish\win
```

Use a different runtime identifier (`-r`) for Linux/macOS when needed.

## Windows Service Hosting

The worker is configured with `UseWindowsService(...)` and service name `MailVault`.

Typical install flow (after publish):

```powershell
sc.exe create MailVault binPath= "C:\Path\To\MailVault.Worker.exe" start= auto
sc.exe start MailVault
```

To remove:

```powershell
sc.exe stop MailVault
sc.exe delete MailVault
```

## Logs

- Console logging is enabled for normal runs.
- On Windows Service hosts, logs are also written to Event Log under source `MailVault`.

## Project Structure

```text
MailVault/
├─ README.md
└─ Src/
   └─ Worker/
      ├─ Program.cs                 # Host setup + Windows service integration
      ├─ Worker.cs                  # Backup loop and sync orchestration
      ├─ AppConfig.cs               # Strongly-typed MailVault settings
      ├─ appsettings.json           # Logging defaults
      ├─ MailVault.Worker.csproj
      └─ MailVault.slnx
```

## Notes

- Keep secrets out of source control.
- If you use Gmail, enable IMAP and use an app password instead of your account password.
