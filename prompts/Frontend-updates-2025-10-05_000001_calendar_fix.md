User Prompt (2025-10-05): "Calendar is not fitting, please fix"

Assistant Response Summary:
- Updated `messages-list.component.css` to allow the filter panel to let the PrimeNG DatePicker overlay extend (overflow: visible) and increased z-index.
- Added high z-index to `.p-datepicker` overlay and ensured a minimum width for header controls to prevent clipping.
- This should resolve cropped calendar navigation arrows as seen in screenshot.

Code Changes:
```
.filter-panel.expanded { overflow: visible; position: relative; z-index: 20; }
:host ::ng-deep .p-datepicker:not(.p-datepicker-inline) { z-index: 1000 !important; }
:host ::ng-deep .p-datepicker .p-datepicker-group-container { min-width: 300px; }
```

Next Steps (Optional):
- If multiple stacked overlays appear elsewhere, centralize z-index tokens in a shared SCSS partial.
- Consider replacing ::ng-deep usage with global style override if future Angular removes it.

End of log.