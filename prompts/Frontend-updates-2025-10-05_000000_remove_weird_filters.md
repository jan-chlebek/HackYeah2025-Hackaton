User Prompt (2025-10-05): "In the messages in wyszukiwanie there is weird fields: please remove it"

Assistant Action Summary:
- Removed two checkbox filter fields (Moje podmioty, Wymagana odpowiedź UKNF) from `messages-list.component.html`.
- Left a comment placeholder noting their removal.
- No TypeScript logic referencing them required removal beyond template (fields were optional in `MessageFilters`).
- Updated TODO list marking task completed.

Rationale:
These fields appeared visually as unlabeled square checkboxes (screenshot) and are not part of confirmed requirements for message search. Removing prevents user confusion and cleans UI.

Next Potential Enhancements:
- If later required, reintroduce with labels & accessible descriptions.
- Consider moving advanced filters into collapsible panel with clear grouping.

End of log.