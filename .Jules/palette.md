## 2023-09-07 - Added Tooltips to ambiguous buttons
**Learning:** Adding `ToolTip` to ambiguous buttons like "..." and "Choose..." in WPF `SettingsWindow` improves accessibility and clarity since ARIA labels aren't applicable in WPF C# native Desktop App.
**Action:** When working on WPF C# native applications, consider using the native properties like `ToolTip` or `AutomationProperties.Name` for a11y improvements instead of the typical web ARIA equivalents.
