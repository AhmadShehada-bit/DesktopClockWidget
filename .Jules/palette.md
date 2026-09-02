## 2024-10-24 - Accessible icon-only buttons in WPF
**Learning:** In C# WPF applications, web-based ARIA attributes (`aria-label`) cannot be used. Instead, `ToolTip` is used for visual hover context, and `AutomationProperties.Name` (or `ToolTip` as fallback) provides screen reader accessibility. Icon-only buttons (like `...`, `-`, `+`, `☆`) desperately need these properties for a good experience.
**Action:** When adding or auditing icon-only WPF controls, apply `ToolTip` explicitly.
