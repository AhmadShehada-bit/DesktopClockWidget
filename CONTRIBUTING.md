# Contributing to DesktopClock Widget

Thank you for your interest in contributing to **DesktopClock Widget**! We welcome community contributions, bug fixes, typography improvements, and feature suggestions.

---

## 🚀 Getting Started

1. **Fork the Repository** on GitHub: [DesktopClockWidget](https://github.com/AhmadShehada-bit/DesktopClockWidget).
2. **Clone your fork** locally:
   ```bash
   git clone https://github.com/YOUR_USERNAME/DesktopClockWidget.git
   cd DesktopClockWidget
   ```
3. **Build the project** using the standard build script:
   ```cmd
   build.bat
   ```
4. **Run self-tests and validation**:
   ```cmd
   bin\Release\DesktopClockWidget.exe --selftest
   bin\Release\DesktopClockWidget.exe --dragtest
   ```

---

## 🛠️ Development Guidelines

- **Preserve Lightweight Performance**: DesktopClock Widget is designed for 24/7 background desktop use. Keep idle CPU and memory consumption minimal.
- **Zero Startup Preloading**: Font files and heavy UI elements must resolve on-demand rather than during application launch.
- **Stable Anchor Invariance**: Position and anchor coordinates must remain strictly invariant across layout, time, and text size changes.
- **No `HWND_BOTTOM`**: Never force windows to `HWND_BOTTOM` (which causes flickering on Windows desktop wallpaper). Always use native desktop window composition.

---

## 📦 Pull Request Workflow

1. Create a feature branch:
   ```bash
   git checkout -b feat/your-feature-name
   ```
2. Make clean, focused changes with clear commit messages.
3. Verify that all 55+ self-tests and drag tests pass.
4. Push to your fork and submit a Pull Request to `main`.
5. Describe your changes clearly with screenshots or test verification logs where applicable.

---

## 🧩 Submitting Ideas

Have an idea for a new feature or custom desktop widget?
- Use our [Feature Request](https://github.com/AhmadShehada-bit/DesktopClockWidget/issues/new?template=feature_request.yml) form.
- Use our [Custom Widget Request](https://github.com/AhmadShehada-bit/DesktopClockWidget/issues/new?template=custom_widget_request.yml) form.
