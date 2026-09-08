## 2024-05-18 - ToolTips for Icon-Only Buttons
**Learning:** Icon-only buttons (like "+", "-", "☆") in WPF applications can be inaccessible and confusing for users who rely on screen readers or hover for context.
**Action:** Always add the `ToolTip` property to `Button` controls that lack descriptive text, ensuring they communicate their function clearly.
## 2026-09-08 - [Add Accessible Tooltips to Icon-Only Buttons]
**Learning:** Adding accessible names to icon-only buttons via an overloaded factory method significantly improves screen reader usability while eliminating redundant, manual `ToolTip` assignments scattered across the UI code.
**Action:** When adding or updating symbolic/iconic buttons in WPF, always provide an accessible label (e.g. `AutomationProperties.Name`) alongside a visual tooltip to ensure complete UI accessibility without relying strictly on web-centric ARIA paradigms.
