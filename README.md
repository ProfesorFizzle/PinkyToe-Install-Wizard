# PinkyToe Install Wizard (v1.2.2beta1)
*This project was coded entirely in CoPilot, I am not a coder.

![PinkyToe Install Wizard Logo](logo.jpg)

A tiny Windows tool to drop your “pre‑installed” game archive into and get:
- Clean extraction into a chosen games folder
- Automatic / assisted main EXE detection (now smarter + shows icons)
- Desktop / Start Menu shortcuts (optional)
- One‑click launch after install
- Password + multi‑part archive handling
- Clear progress, speed, and ETA

Standalone EXE (no install needed):
[https://mega.nz/file/uR8GQA4Y#FuHesmcJa3KXCYkQ_LXSFgBy8OJ6uHSwKbZJSn2om2w](https://mega.nz/file/DNcmUCjS#s1HGzS8KhkvayWfqcGBGNjioeWbpBrjamRqAol36C_U)

> If you just want to use it, download the EXE above.  
> If you want to tinker or verify, build from source (instructions below).

---

## Quick Start (Using the EXE)

1. Run PinkyToeInstallWizard.exe  
2. Drag & drop (or browse to) your game archive (RAR / 7z / ZIP / multi‑part)  
3. Pick (or accept) an install folder  
4. Click “Extract & Install!”  
5. If the tool isn’t 100% sure about the main game EXE, a chooser pops up (now with icons) – pick the right one  
6. (Optional) Leave “Create Desktop Shortcut” / “Launch After Install” checked  
7. Done – play!

---

## What’s New in v1.2.2beta1 (since v1.2.1)

- Smarter main EXE detection:
  - Exact / near‑name matching improved
  - Embedded icon resource scanning boosts the real game EXE
  - Better filtering of launchers / tools / benchmarks
  - Clear scoring reasons (shown if you open the chooser)
- EXE chooser now shows each file’s icon
- Updated UI footer with version label
- Better multi‑part handling (RAR & 7z.001 sequences)
- Improved progress display (speed + ETA tweaks)
- Update detection: detects an existing install of the same game EXE and lets you update in place or create a fresh folder

---

## Features (Simple Overview)

- Drag & Drop everything
- Multi‑part archives (.part1.rar chains, .7z.001 sets)
- Password manager:
  - Built‑in list of common scene/repack passwords
  - Add / remove your own (encrypted locally with Windows DPAPI)
- Heuristic + icon‑assisted EXE picker (auto when confident)
- Desktop & Start Menu shortcut creation
- Optional auto‑launch after extraction
- Safe extraction (blocks path traversal attempts)
- Progress: MB done / total, MB/s, ETA
- “Open Install Folder” button when finished

---

## Supported Archive Types

- .rar (single + .partN.rar)
- .7z (single + .7z.001 / .7z.002 …)
- .zip

(Other formats may appear to work if SharpCompress supports them, but the above are the intended ones.)

---

## Building from Source

1. Install .NET 6 SDK (or later compatible with `net6.0-windows`)  
2. Clone this repo  
3. Place `pinkytoe.ico` and `logo.jpg` in the project root (same folder as the `.csproj`)  
4. Open `PinkyToeInstallWizard.sln` in Visual Studio (or run `dotnet build -c Release`)  
5. Output EXE: `bin/Release/net6.0-windows/PinkyToeInstallWizard.exe`

No extra setup steps. NuGet will fetch dependencies automatically (SharpCompress, Newtonsoft.Json).

---

## FAQ (Short)

**It picked the wrong EXE. What do I do?**  
Click “Change…” (or choose from the popup if auto-select wasn’t confident), pick the correct one, continue.

**Archive asks for a password.**  
It will try saved + built‑in passwords. If all fail, you’ll be prompted.

**Does this include any game files?**  
No. You must have your own legally obtained archives.

**Can I use this commercially?**  
Yes — the license below is permissive (no non‑commercial restriction). Just keep the no‑warranty notice.

---

## License

Released under the **0BSD** license (zero‑clause BSD) – do whatever you want, no warranty.

```
Copyright (c) 2025 PinkyToe Team
Permission to use, copy, modify, and/or distribute this software for any purpose with or without fee is hereby granted.
THE SOFTWARE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND...
```

(See LICENSE file for full text.)

---

## Disclaimer

This tool only automates extraction & shortcut creation.  
You are solely responsible for ensuring you have the rights to any game archives you process.

---

## Feedback / Issues

Open an issue or suggest improvements. Short, plain requests are fine.  
Enjoy!

---
_PinkyToe Install Wizard — v1.2.2beta1_
