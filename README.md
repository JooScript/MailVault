# MailVault

MailVault is a background email backup worker that pulls messages from IMAP folders and stores them in a GitHub-backed archive.

---

## Overview

The service:

* runs continuously as a .NET worker
* checks mail on a fixed interval
* saves new messages into a local backup repository
* commits and pushes changes to GitHub when new mail is found

---

## What This Project Does

MailVault is designed for long-running, unattended backup of email accounts.

Main flow per cycle:

1. Ensure the local backup repository exists (clone if missing)
2. Connect to IMAP with configured credentials
3. Download new messages from configured folders
4. If new mail exists and internet is available, commit and push to GitHub

---

## Tech Stack

* .NET 10 Worker Service
* Microsoft.Extensions.Hosting.WindowsServices
* Shared utilities project: CS_Utilities

---

## External Dependency

* [https://github.com/JooScript/CS_Utilities](https://github.com/JooScript/CS_Utilities)

---

## Configuration

Configuration is loaded using:

1. appsettings.json (non-sensitive only)
2. Environment Variables (**all secrets**)

---

## ⚠️ Security Best Practice

Never store secrets in source control.

This includes:

* IMAP host & credentials
* GitHub repository URL
* GitHub token
* Email

---

## appsettings.json (Safe Template)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "MailVault": {
    "ImapPort": 993,
    "BackupDir": "C:/MailVaultBackup",
    "Mailboxes": ["INBOX"],
    "IntervalHours": 6,
    "Owner": "your-github-user"
  }
}
```

---

## Environment Variables (All Sensitive Values)

### Mapping Rule

``` text
Section:Key → Section__Key
```

### Required Variables

| Config Key          | Environment Variable |
| ------------------- | -------------------- |
| MailVault:ImapHost  | MailVault__ImapHost  |
| MailVault:ImapUser  | MailVault__ImapUser  |
| MailVault:ImapPass  | MailVault__ImapPass  |
| MailVault:GitHubUrl | MailVault__GitHubUrl |
| MailVault:Token     | MailVault__Token     |
| MailVault:Email     | MailVault__Email     |

---

## Setting Environment Variables

### Windows (Persistent)

```powershell
setx MailVault__ImapHost "imap.gmail.com" /M
setx MailVault__ImapUser "your-email@gmail.com" /M
setx MailVault__ImapPass "your-app-password" /M
setx MailVault__GitHubUrl "https://github.com/your-user/your-backup-repo.git" /M
setx MailVault__Token "your-github-token" /M
setx MailVault__Email "your-email@gmail.com" /M
```

Restart the service after setting variables.

---

### Linux / macOS

```bash
export MailVault__ImapHost="imap.gmail.com"
export MailVault__ImapUser="your-email@gmail.com"
export MailVault__ImapPass="your-app-password"
export MailVault__GitHubUrl="https://github.com/your-user/your-backup-repo.git"
export MailVault__Token="your-github-token"
```

---

## Local Development

```powershell
cd .\Src\Worker
dotnet restore
dotnet run
```

---

## Build

```powershell
dotnet publish -c Release -r win-x64 --self-contained false -o .\publish\win
```

---

## Windows Service Hosting

```powershell
sc.exe create MailVault binPath= "C:\Path\To\MailVault.Worker.exe" start= auto
sc.exe start MailVault
```

---

## Logs

* Console logging for development
* Windows Event Log when running as a service

---

## Notes

* Use environment variables for all sensitive values
* Rotate credentials if exposed
* Use app passwords for Gmail
* Ensure IMAP is enabled
