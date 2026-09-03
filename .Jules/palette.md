## 2026-09-03 - Accessible Icon Buttons in WPF
**Learning:** In C# WPF applications (unlike web applications), you cannot use web-based ARIA attributes (`aria-label`) for accessibility.
**Action:** Use `ToolTip` properties for visible mouse hints and `System.Windows.Automation.AutomationProperties.SetName()` for screen reader accessible names on icon-only interactive elements.
