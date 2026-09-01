## 2024-05-14 - Add ToolTip and AutomationProperties.Name to Icon-Only Favorite Buttons
**Learning:** In C# WPF applications targeting .NET Framework 4.0, UI accessibility is handled via `AutomationProperties.Name` for screen readers, unlike web-based applications which use ARIA attributes. Also, adding `ToolTip` properties provides crucial visible hover text context for icon-only buttons.
**Action:** When asked to improve UX in WPF, look for buttons with symbols/icons as content (like "\u2606") and set both `ToolTip` and `System.Windows.Automation.AutomationProperties.SetName`.
