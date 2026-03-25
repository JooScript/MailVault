# MailVault

MailVault is a background email backup worker that pulls messages from IMAP folders and stores them in a GitHub-backed archive.

---

## Overview

The service:

* runs continuously as a .NET worker
* checks mail on a fixed interval (currently every 6 hours)
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

This project references:

* [https://github.com/JooScript/CS_Utilities](https://github.com/JooScript/CS_Utilities)

---

## Configuration

Configuration is loaded using the standard .NET configuration pipeline:

1. appsettings.json
2. appsettings.{Environment}.json
3. **Environment Variables (recommended for secrets)**

---

## ⚠️ Security Best Practice

**Never store secrets in source control.**

Sensitive values such as passwords and tokens must NOT be placed in appsettings.json.

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
    "ImapHost": "imap.gmail.com",
    "ImapPort": 993,
    "GitHubUrl": "https://github.com/your-user/your-backup-repo.git",
    "BackupDir": "C:/MailVaultBackup",
    "Mailboxes": [
      "INBOX"
    ],
    "IntervalHours": 6,
    "Owner": "your-github-user"
  }
}
```

---

## Environment Variables (Required for Secrets)

Use environment variables for all sensitive data.

### Mapping Rule

```
Section:Key → Section__Key
```

### Required Variables

| Config Key         | Environment Variable |
| ------------------ | -------------------- |
| MailVault:ImapUser | MailVault__ImapUser  |
| MailVault:ImapPass | MailVault__ImapPass  |
| MailVault:Token    | MailVault__Token     |
| MailVault:Email    | MailVault__Email     |

---

## Setting Environment Variables

### Windows (Persistent)

```powershell
setx MailVault__ImapUser "your-email@gmail.com" /M
setx MailVault__ImapPass "your-app-password" /M
setx MailVault__Token "your-github-token" /M
setx MailVault__Email "your-email@gmail.com" /M
```

Restart the service after setting variables.

---

### Windows (Temporary Session)

```powershell
$env:MailVault__ImapUser="your-email@gmail.com"
$env:MailVault__ImapPass="your-app-password"
$env:MailVault__Token="your-github-token"
```

---

### Linux / macOS

```bash
export MailVault__ImapUser="your-email@gmail.com"
export MailVault__ImapPass="your-app-password"
export MailVault__Token="your-github-token"
```

---

## Local Development

### Prerequisites

* .NET SDK 10+
* Access to CS_Utilities project
* IMAP credentials
* GitHub repository + token

### Run

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

Install:

```powershell
sc.exe create MailVault binPath= "C:\Path\To\MailVault.Worker.exe" start= auto
sc.exe start MailVault
```

Remove:

```powershell
sc.exe stop MailVault
sc.exe delete MailVault
```

---

## Logs

* Console logging for development
* Windows Event Log when running as a service

---

## Project Structure

```text
MailVault/
├─ README.md
└─ Src/
   └─ Worker/
      ├─ Program.cs
      ├─ Worker.cs
      ├─ AppConfig.cs
      ├─ appsettings.json
      └─ MailVault.Worker.csproj
```

---

## Notes

* Use environment variables for secrets
* Rotate tokens immediately if exposed
* Use app passwords for Gmail (not your main password)
* Ensure IMAP is enabled for your email provider

---

## Recommended Next Improvements

* Add configuration validation on startup
* Add retry policies for IMAP and Git operations
* Add structured logging (Serilog)
* Add health checks for monitoring
