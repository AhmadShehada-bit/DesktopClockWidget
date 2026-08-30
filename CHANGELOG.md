# Changelog

All notable changes to DesktopClock Widget are documented in this file.

## [1.0.0] - 2026-08-30

### Initial Release
- **Lightweight Desktop Clock Core**: High performance, single-process WPF architecture with zero idle CPU overhead and borderless transparent overlay.
- **Stable Screen-Space Anchor**: Absolute `AnchorX` / `AnchorY` visual centering system preventing position drift across time, weekday, greeting, scale, font, and block changes.
- **Dynamic Greetings**: Scheduled time-based greeting engine (Morning / Afternoon / Evening / Night) with custom text and hours override.
- **Per-Element Typography & Colors**: Independent font family, font weight, size, color, opacity, and text case settings for Greeting, Weekday, Time, and Date.
- **Curated 146 Font Catalog**: Bundled open-source typefaces across Futuristic, Athletic, Condensed, Minimal, Monospace, Serif, Display, Handwritten, and Script categories with integrated search, category filter, and favorites.
- **Per-Element Visual Effects Engine**:
  - **Outline**: Crisp, sub-pixel glyph contour stroke rendering.
  - **Cyber Glitch**: Dynamic chromatic ghost channels (Cyan / Magenta) with controlled displacement jitter.
  - **Digital Noise**: Micro-scanline texture and luminance modulation.
- **Custom Blocks System**: Add arbitrary decoration symbols, static motivation banners, scheduled text, and interval-based rotating messages across 12 layout positions.
- **Position & Interaction Controls**: Click-through desktop behavior, lock/edit mode toggle via `Ctrl + Alt + C`, system tray menu with instant live preview.
- **Windows Startup Integration**: Seamless optional launch on Windows boot.
