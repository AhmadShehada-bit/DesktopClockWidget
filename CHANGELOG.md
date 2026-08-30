# Changelog

All notable changes to DesktopClock Widget are documented in this file.

## [1.1.0] - 2026-08-30

### Added & Enhanced
- **Theme Presets & One-Click Profiles**:
  - 10 curated aesthetic built-in themes: Cyberpunk Neon, Minimalist Slate, Aesthetic Serif, Handwritten Studio, Emerald HUD, Neon Horizon, Midnight Slate, Vintage Digital, Rose Gold, and Monochrome Pure.
  - Non-destructive live preview with instant apply and cancel snapshot rollback.
  - Custom preset management: save current layout as user preset to local `themes.json`, duplicate, rename, and delete.
- **Optional Desktop Widget Modules (Lazy Architecture)**:
  - **Weather Module**: Asynchronous legal weather fetching from Open-Meteo REST API with WMO weather glyph mapping, Celsius/Fahrenheit units, 15–30 min memory caching, offline fallback, and zero network/timer overhead when disabled.
  - **System Metrics Module**: Ultra-low overhead native CPU (`GetSystemTimes`) and RAM (`GlobalMemoryStatusEx`) monitoring with configurable sampling rate (1s, 2s, 5s) and zero timer overhead when disabled.
- **Multi-Monitor & Multi-Timezone Clocks**:
  - **Multi-Monitor Support**: Display detection, Per-Monitor DPI scaling preservation, "Move to Display" / "Center on Display", and automatic safe clamping to visible screen on monitor disconnect.
  - **Multi-Timezone World Clocks**: Configurable secondary clocks using Windows `TimeZoneInfo` database, automatic Daylight Saving Time calculation, 12h/24h toggles, and custom city/office labels.
- **Zero-Visual-Jitter Layout Envelopes**:
  - Precomputed geometry and ink bounds across all 24h times, days, dates, greetings, weather conditions, and timezones ensuring strictly 0.000 DIP visual jitter during minute/second updates.
- **Fast Mouse-Wheel Font Browsing & Bounded LRU Cache**:
  - 217+ curated bundled fonts across 10 categories with immediate mouse-wheel cycling and bounded LRU font cache (<=10 items).
- **Distribution Readiness (MSIX, Portable ZIP, and Winget)**:
  - Clean portable release package (`DesktopClockWidget-v1.1.0-portable.zip`) with SHA256 checksums.
  - Windows Package Manager (`winget`) v1.6.0 manifest trio (`AhmadShehada.DesktopClockWidget`).
  - Microsoft Store MSIX package configuration (`msix/AppxManifest.xml`) and store readiness specifications.
  - Transparent `PRIVACY.md` compliance policy.
- **Expanded Automated Test Suite**:
  - 80 automated self-test checks and drag tests covering all core and modular capabilities with 100% pass rate.

## [1.0.1] - 2026-08-30

### Added & Enhanced
- **Official Branding & Logo**: Minimalist cyber/futuristic clock logo with electric cyan accents and multi-resolution vector icon assets (16px to 1024px, SVG, and multi-res app.ico).
- **High-DPI Tray Icon**: Enhanced pixel-perfect system tray icon renderer.
- **Migration & Deployment**: Dedicated portable runtime packaging and automated startup configuration.

## [1.0.0] - 2026-08-30

### Initial Release
- **Lightweight Desktop Clock Core**: High performance, single-process WPF architecture with zero idle CPU overhead and borderless transparent overlay.
- **Stable Screen-Space Anchor**: Absolute AnchorX / AnchorY visual centering system preventing position drift.
- **Dynamic Greetings**: Scheduled time-based greeting engine (Morning / Afternoon / Evening / Night) with custom text.
- **Per-Element Typography & Colors**: Independent typography and color configurations.
- **Curated Font Catalog**: Bundled open-source typefaces with search and categories.
- **Visual Effects Engine**: Outline, Cyber Glitch, and Digital Noise.
- **Custom Blocks System**: Decoration symbols, motivation banners, and scheduled/rotating messages.
- **Position & Interaction Controls**: Click-through desktop behavior, lock/edit mode toggle via Ctrl+Alt+C.
