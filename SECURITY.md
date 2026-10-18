# Security policy

## Contents

- [Reporting a vulnerability](#reporting-a-vulnerability)
- [Verifying a download](#verifying-a-download)

## Reporting a vulnerability

Please do not open a public issue. Use [**Report a vulnerability**](https://github.com/Sokenzane/PilePoil/security/advisories/new) (Security tab) to contact the maintainer privately.

## Verifying a download

Only download `PilePoil.exe` from this repository's [Releases](https://github.com/Sokenzane/PilePoil/releases). Each release lists the SHA-256 of the exe; to check it, run in PowerShell:

```powershell
Get-FileHash .\PilePoil.exe -Algorithm SHA256
```
