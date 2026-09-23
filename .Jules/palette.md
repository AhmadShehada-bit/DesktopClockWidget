## 2024-05-18 - ToolTips for Icon-Only Buttons
**Learning:** Icon-only buttons (like "+", "-", "☆") in WPF applications can be inaccessible and confusing for users who rely on screen readers or hover for context.
**Action:** Always add the `ToolTip` property to `Button` controls that lack descriptive text, ensuring they communicate their function clearly.
## 2026-09-08 - [Add Accessible Tooltips to Icon-Only Buttons]
**Learning:** Adding accessible names to icon-only buttons via an overloaded factory method significantly improves screen reader usability while eliminating redundant, manual `ToolTip` assignments scattered across the UI code.
**Action:** When adding or updating symbolic/iconic buttons in WPF, always provide an accessible label (e.g. `AutomationProperties.Name`) alongside a visual tooltip to ensure complete UI accessibility without relying strictly on web-centric ARIA paradigms.
## 2026-09-10 - [UX Improvements on List Controls]
**Learning:** Destructive actions on lists, such as removing custom configurations, often lead to accidental data loss without prompt confirmation. In WPF, simple MessageBox prompts are sufficient for mitigating these issues for small, custom blocks.
**Action:** When adding generic 'Delete' buttons on custom element lists, always include a confirmation prompt and provide clear UI feedback. Avoid making changes directly without user acknowledgment for potentially destructive events.
## 2024-05-24 - Sub-list Control Accessibility and Destructive Action Confirmations
**Learning:** I discovered a pattern in this application where secondary controls within lists (like block messages, schedules, and timezones) were missing tooltips and ARIA labels. Additionally, destructive actions (deletions) on these list items lacked a warning confirmation dialog, which could lead to accidental data loss.
**Action:** When adding or modifying interactive elements within lists, always ensure `CreateStyledButton` is provided with a third string argument to set `ToolTip` and `AutomationProperties.Name`. Wrap any destructive action logic (like `RemoveAt`) in a `MessageBox.Show` with `MessageBoxButton.YesNo` and `MessageBoxImage.Warning`.
