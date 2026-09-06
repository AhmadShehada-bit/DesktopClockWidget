## 2024-05-18 - ToolTips for Icon-Only Buttons
**Learning:** Icon-only buttons (like "+", "-", "☆") in WPF applications can be inaccessible and confusing for users who rely on screen readers or hover for context.
**Action:** Always add the `ToolTip` property to `Button` controls that lack descriptive text, ensuring they communicate their function clearly.
## 2026-09-06 - Added ToolTips to color picker buttons
**Learning:** In WPF, icon-only or ambiguous text buttons (like '...') can cause accessibility issues as they lack context. Adding 'ToolTip' properties provides essential context for screen readers and mouse hover without cluttering the UI.
**Action:** When adding or modifying ambiguous buttons in WPF, always ensure a descriptive 'ToolTip' or 'AutomationProperties.Name' is provided.
