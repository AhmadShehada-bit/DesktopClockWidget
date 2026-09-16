## 2024-05-18 - ToolTips for Icon-Only Buttons
**Learning:** Icon-only buttons (like "+", "-", "☆") in WPF applications can be inaccessible and confusing for users who rely on screen readers or hover for context.
**Action:** Always add the `ToolTip` property to `Button` controls that lack descriptive text, ensuring they communicate their function clearly.
## 2026-09-08 - [Add Accessible Tooltips to Icon-Only Buttons]
**Learning:** Adding accessible names to icon-only buttons via an overloaded factory method significantly improves screen reader usability while eliminating redundant, manual `ToolTip` assignments scattered across the UI code.
**Action:** When adding or updating symbolic/iconic buttons in WPF, always provide an accessible label (e.g. `AutomationProperties.Name`) alongside a visual tooltip to ensure complete UI accessibility without relying strictly on web-centric ARIA paradigms.
## 2026-09-10 - [UX Improvements on List Controls]
**Learning:** Destructive actions on lists, such as removing custom configurations, often lead to accidental data loss without prompt confirmation. In WPF, simple MessageBox prompts are sufficient for mitigating these issues for small, custom blocks.
**Action:** When adding generic 'Delete' buttons on custom element lists, always include a confirmation prompt and provide clear UI feedback. Avoid making changes directly without user acknowledgment for potentially destructive events.
## 2024-11-20 - Accessible Names for Ambiguous Controls
**Learning:** In highly customized WPF UI like this app where standard controls are heavily modified or styled (e.g. `CreateStyledButton`), multiple instances of buttons with generic visible text like "Choose..." (for color pickers) or icon-based buttons like "✎ Rename..." create severe accessibility issues for screen readers. They read just the visible text which lacks context.
**Action:** When adding or auditing repetitive utility buttons (like "Choose..." or "Edit") or buttons relying on symbols/icons, always use the 3-argument overload of `CreateStyledButton` (or explicitly set `AutomationProperties.Name` and `ToolTip`) to provide a descriptive, context-specific accessible name (e.g. "Choose Global Color").
