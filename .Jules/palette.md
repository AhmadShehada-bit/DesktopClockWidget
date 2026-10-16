## 2024-05-18 - ToolTips for Icon-Only Buttons
**Learning:** Icon-only buttons (like "+", "-", "☆") in WPF applications can be inaccessible and confusing for users who rely on screen readers or hover for context.
**Action:** Always add the `ToolTip` property to `Button` controls that lack descriptive text, ensuring they communicate their function clearly.
## 2026-09-08 - [Add Accessible Tooltips to Icon-Only Buttons]
**Learning:** Adding accessible names to icon-only buttons via an overloaded factory method significantly improves screen reader usability while eliminating redundant, manual `ToolTip` assignments scattered across the UI code.
**Action:** When adding or updating symbolic/iconic buttons in WPF, always provide an accessible label (e.g. `AutomationProperties.Name`) alongside a visual tooltip to ensure complete UI accessibility without relying strictly on web-centric ARIA paradigms.
## 2026-09-10 - [UX Improvements on List Controls]
**Learning:** Destructive actions on lists, such as removing custom configurations, often lead to accidental data loss without prompt confirmation. In WPF, simple MessageBox prompts are sufficient for mitigating these issues for small, custom blocks.
**Action:** When adding generic 'Delete' buttons on custom element lists, always include a confirmation prompt and provide clear UI feedback. Avoid making changes directly without user acknowledgment for potentially destructive events.
## 2026-10-16 - Prevent Accidental Data Loss on Deletion
**Learning:** When dealing with multiple list items containing user data (like messages, schedules, and timezones) in a settings window, users can easily click the wrong 'Delete' button or mistakenly trigger a destructive action.
**Action:** Standardize the use of `MessageBox.Show` with `MessageBoxButton.YesNo` and `MessageBoxImage.Warning` on all destructive actions related to custom data structures. Always ensure that 'Delete' buttons also have descriptive tooltips to explicitly state what is being deleted.
