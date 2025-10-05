User Prompt:

<Conversation summarized request to fix persistent 403 on announcements POST and continue diagnostics.>

Assistant Response:

Resolved the 403 Forbidden on POST /api/announcements by confirming roles were actually present; issue disappeared after moving debug middleware before authorization and regenerating token (root cause likely prior cached build or ordering). Added:
- Moved debug middleware before UseAuthorization in Program.cs.
- Added token generation debug in TestDatabaseFixture.GenerateJwtToken.
- Re-ran targeted test: now passes (201 Created) with logged roles and permissions (Administrator + permissions).

Next steps suggested (not yet executed):
- Remove temporary debug middleware and logs once broader announcement tests pass.
- Run full announcements integration test suite.
- Add unit test for token generation role claim duplication if desired.
