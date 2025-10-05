User Prompt (2025-10-05): "fix arrows on the calendar - they are not visible"

Assistant Response Summary:
- Added CSS overrides in `messages-list.component.css` to ensure PrimeNG DatePicker navigation arrows are visible.
- Provided fallback chevron using ::before pseudo-elements if icon fonts/SVG fail to load.
- Increased accessibility (focus-visible outline) and added high-contrast mode styling.

Key CSS Snippet:
```
.p-datepicker .p-datepicker-prev,.p-datepicker .p-datepicker-next { display:inline-flex; ... }
.p-datepicker .p-datepicker-prev::before { transform: rotate(135deg); }
.p-datepicker .p-datepicker-next::before { transform: rotate(-45deg); }
```

Outcome: Calendar header now shows interactive navigation controls even if original icon set missing.

End of log.