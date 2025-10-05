User Prompt (2025-10-05): "Searching on messages doesn't work, please fix"

Assistant Response Summary:
- Enhanced `message.service.ts` filter handling.
- Normalized empty string filters (omit from query), mapped UI Polish field names to probable backend parameter names (identifier, caseSignature, entityName, status, priority, etc.).
- Added date normalization (ISO) and boolean mappings (myEntities, requiresUknfResponse).
- Added debug log of final query string.
- Fixed TypeScript index signature access errors via bracket notation.

Next Steps:
- Verify actual backend param names; adjust mapping if needed.
- Remove debug log in production build.

End of log.