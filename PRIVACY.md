# Privacy Policy for DesktopClock Widget

**Last Updated**: August 30, 2026  
**Applicable Version**: v1.1.0+

DesktopClock Widget is engineered with a strict **local-first, privacy-by-design** philosophy. We believe your desktop setup, personal routines, and system activity belong exclusively to you.

---

## 1. Zero Telemetry & Zero Analytics
- DesktopClock Widget **does not** collect, transmit, or store any personal telemetry, diagnostic data, keystrokes, application logs, or unique device identifiers.
- There are **no tracking pixels**, analytics SDKs, advertising beacons, or background telemetry services embedded in the application.

---

## 2. Local Settings & Preset Storage
- All configuration data (theme selections, custom presets, element typography, coordinates, colors, and scheduled messages) is stored purely locally on your machine at:
  - %LocalAppData%\DesktopClock\DesktopClockWidget.settings
  - %LocalAppData%\DesktopClock\themes.json
- These files never leave your computer and are never synchronized to external cloud databases.

---

## 3. Network Usage (Weather Module Only)
- If and **only if** you explicitly enable the optional **Weather Module** in Settings, the widget connects to the public Open-Meteo weather API (https://api.open-meteo.com/v1/forecast) using your configured geographic coordinates to fetch current temperature and condition data.
- **No private API keys**, user tokens, or personally identifiable information are sent or required.
- Weather queries are cached locally in memory for 15–30 minutes to minimize network activity.
- When the Weather Module is disabled, **zero network connections** are made.

---

## 4. System Metrics (CPU & RAM)
- If and **only if** you explicitly enable the optional **System Metrics Module**, the widget queries local Windows kernel counters (GetSystemTimes and GlobalMemoryStatusEx) in real-time to compute current CPU load and RAM consumption.
- This data is rendered directly to your screen and is never logged to disk or transmitted over the network.

---

## 5. Contact & Open Source Transparency
DesktopClock Widget is fully open-source under the MIT License. You can audit every line of source code, build scripts, and dependencies on GitHub:

- **Repository**: [https://github.com/AhmadShehada-bit/DesktopClockWidget](https://github.com/AhmadShehada-bit/DesktopClockWidget)
- **Maintainer**: Ahmad Shehada ([AhmadShehada-bit](https://github.com/AhmadShehada-bit))
- **Security & Privacy Inquiries**: File an issue or security advisory on the GitHub repository.
