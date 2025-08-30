# PinkyToe Install Wizard

**Version: 1.2.1**

A simple Windows Forms installer and shortcut creator for pre-installed PC games. Just select your archive and install!

---

## Features

- **Multi-part RAR archive support:**  
  Select the first part (e.g., `.part1.rar` or `.rar`), and the wizard will guide you and check for missing parts.

- **Drag & drop archive selection:**  
  Simply drag a `.rar` archive onto the window to select!

- **Password notifications:**  
  Instantly see if your archive is password-protected, whether a saved password will be tried, or if you'll need to enter one.

- **Password manager:**  
  Add, edit, and remove passwords. Saved passwords are automatically tried during extraction.

- **Password prompt:**  
  If extraction fails due to a password, you'll be prompted to enter one.

- **Custom install location:**  
  Choose any folder, or use your default. Installs into a subfolder named after your archive.

- **Progress bar with status messages:**  
  Clear feedback during extraction.

- **Logo image:**  
  Your project logo is displayed in the UI (replace `logo.jpg`).

- **Shortcut creation:**  
  After extraction, select which `.exe` to make a shortcut for.

- **Desktop and Start Menu shortcut options:**  
  Choose whether to create a Start Menu shortcut in addition to the desktop shortcut.

- **Settings persistence:**  
  Default install path and passwords saved in AppData.

---

## Getting Started

### Prerequisites

- [.NET 6.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- Visual Studio 2022 or newer (recommended)
- Windows operating system

### Setup

1. **Clone this repository.**

2. **Add your project logo:**  
   Place your logo image as `logo.jpg` in the project root.  
   In Visual Studio, right-click it > Properties > Set "Build Action" to "Embedded Resource".

3. **Restore NuGet packages:**  
   - `SharpCompress` for archive handling  
   - `Newtonsoft.Json` for settings/passwords

4. **Build and run!**

---

## Usage

1. **Step 1:** Select your archive (`.rar`), or drag & drop it onto the window.
2. **Step 2:** Choose your install location (optional).
3. **Step 3:** Click "Extract & Install!".
4. **If password is needed:**  
   The wizard will try saved passwords, then prompt you if necessary.
5. **Select your game's `.exe`** for shortcut creation.
6. **Choose if you want a Start Menu shortcut.**
7. **Done!** Shortcuts are created, and your game is ready to play.

---

## License

This project is licensed under the [MIT License](LICENSE).

---

## Credits

- [SharpCompress](https://github.com/adamhathcock/sharpcompress)
- [Newtonsoft.Json](https://www.newtonsoft.com/json)

---

## Support & Contributions

- Issues and pull requests are welcome!
- For questions, open a GitHub Issue.
