User Prompt:
Fix uploading reports, frontend does not work with backend.

Assistant Response:
Adjusted frontend ReportService.uploadReport to align with backend API expectations:
- Renamed multipart form keys to File and ReportingPeriod (PascalCase) which backend SubmitReportFormModel requires.
- Extracted quarter token (Q1/Q2/Q3/Q4) from UI value like Q1_2025 before sending.
- Removed unsupported extra fields from form data (entity, correction, remarks) pending backend contract extension.
- Added response mapping from backend ReportDto (PascalCase) to existing frontend Report interface (Polish field names), preserving submitted metadata where backend lacks fields (entity id, correction flag).
- Left TODO note for future backend extension.

No backend changes needed immediately; next step if required: extend SubmitReportFormModel and SubmitReportRequest plus ReportsService.SubmitReportAsync to capture IsCorrection, EntityId, Remarks, then regenerate Swagger and update frontend accordingly.
