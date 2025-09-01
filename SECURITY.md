# Security Policy

## Supported Versions
This is a beta (v1.2.2beta1). Only the latest main branch and tagged releases are intended to receive fixes.

## Reporting a Vulnerability
If you discover a vulnerability (path traversal bypass, malicious archive causing a crash, password handling issue, etc.):

1. Do **NOT** open a public issue with exploit details.
2. Email (replace with your preferred contact) or open a private security advisory on GitHub (if enabled).
3. Provide:
   - Version / commit hash
   - Reproduction steps
   - Expected vs actual behavior
   - Any POC archive (sanitized if possible)

## Scope
This application:
- Performs local archive extraction only.
- Creates optional shortcuts.
- Does not intentionally send network requests.
- Stores user-added passwords encrypted (Windows DPAPI CurrentUser).
- Does not log sensitive data.

## Exclusions
Issues outside scope:
- Malicious archives containing malware (use anti-virus).
- Lack of code signing (release binaries are unsigned by design in this beta).
- Passwords visible to other OS users with full admin rights (DPAPI still per-user).

## Hardening Recommendations for Users
- Run only on trusted archives.
- Keep AV software active.
- Review source & rebuild if concerned about third-party distributed binaries.
- Avoid adding personally identifiable information to password entries.

## Developer Practices
- No secrets or personal info should be committed. (.gitignore excludes build artifacts and user settings.)
- Before pushing, scan repo history for accidental secrets:
  - `git log -p`
  - Tools like `trufflehog` or `gitleaks`.

## Planned Improvements
- Optional hash verification step
- Additional PE sanity checks before extraction
- Sandboxed icon parsing (future; current implementation does in-memory parsing only)

Thank you for helping keep the project secure.