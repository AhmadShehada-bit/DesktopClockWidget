## 2026-08-31 - Accessibility labels in WPF
**Learning:** For C# WPF applications, the equivalent to ARIA labels is `System.Windows.Automation.AutomationProperties.Name`, and ToolTip provides visual assistance on hover.
**Action:** When adding accessibility to icon-only buttons in WPF, set the ToolTip and use AutomationProperties.SetName to ensure screen readers provide context.
