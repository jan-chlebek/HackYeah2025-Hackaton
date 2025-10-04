# HackYeah 2025 – Copilot Instructions

## Project in a nutshell
- Build a demo communication platform for UKNF that covers three domains: Communication, Identity & Access, and Administration.
- Target architecture: Angular 20 SPA that talks to an ASP.NET Core 8/9 REST API backed by a relational database (MSSQL preferred) and optional messaging for async work.
- Shipping equals working code **plus** a documented prompt engineering process in `prompts.md` that lists every AI prompt chronologically and highlights the most effective ones.

## How we collaborate with AI
- When generating code, prefer incremental prompts that describe intent, interfaces, edge cases, and tests before implementation. Preserve the prompt/response sequence in `prompts.md`.
- Call out when you hand-edit code versus when it was AI-generated. Flag risky suggestions (security, migrations, infra) for manual review.
- Favour open-source/licence-compatible snippets. Never paste content from paid or proprietary sources.

## Architecture guardrails
- Keep a clear separation between the Angular SPA (`frontend/`) and the ASP.NET Core Web API (`backend/`). Shared models belong in versioned contracts (OpenAPI/TypeScript DTOs generated from C#).
- Enforce REST conventions: plural nouns, `/api/v1/...`, problem+json for errors, and include pagination/filter metadata in list responses.
- All user-facing flows must respect the functional spec: communication dashboard, messaging with attachments, file library, case folders, FAQs, contact registry, and podmiot data updater.
- Non-functional must-haves:
  - File management: chunked uploads, ZIP support, metadata search, permissions; design APIs that can plug into virus-scanning later.
  - Security: OAuth2/OIDC with JWT, encrypted secrets at rest, HTTPS-only assumptions, input validation, audit trails.
  - Performance: caching strategy, background jobs via queue (Kafka/RabbitMQ placeholder), pagination, and health/metrics endpoints.
  - Accessibility & UX: WCAG 2.2 compliance, high-contrast theme toggle, responsive layouts, sticky table headers, keyboard navigation.
- Every feature needs automated tests (unit + integration) plus a quick smoke scenario documented in PR descriptions.

## Delivery expectations
- Update or create Docker assets (Dockerfile + `docker-compose.yml`) whenever dependencies change. Containers must build from the repo root without manual tweaks.
- Surface environment variables, secrets, connection strings, and feature flags via configuration docs and sample `.env` templates—never hardcode secrets.
- Generate or refresh API docs (e.g. Swashbuckle, ReDoc) and front-end Storybook/Chromatic previews when changing public contracts/UI components.
- Align migrations, seed data, and fixtures with the supplied test data set. Include rollback/backfill notes when altering schemas.

## Quick checklist before finishing a task
- ✅ Code conforms to Angular/ASP.NET Core style guides and linting rules.
- ✅ Tests (unit + integration) and linters pass locally; capture command output in the PR/issue comment.
- ✅ Security, accessibility, and performance considerations from the requirements are addressed or explicitly deferred.
- ✅ `.documentation/prompts.md` updated with the latest prompting steps and lessons learned.

Always take into consideration the contents of and all:
- .requirements/UX&UI Recomendations/KNF (Komisja Nadzoru Finansowego) UI_UX Design.md
- .requirements/DETAILS_UKNF_Prompt2Code2.md
- .requirements/RULES_UKNF_Prompt2Code2.md
- .requirements/UX&UI Recomendations/knf_color_palette_recommendations.csv
- .requirements/Prompt2Code2/ENG_attachments/F. test data of supervised entities.html
- .requirements/Prompt2Code2/ENG_attachments/E. prototypes of selected low-detailed screens/00 - Pulpit.png
- .requirements/Prompt2Code2/ENG_attachments/E. prototypes of selected low-detailed screens/01 - Pulpit.png
- .requirements/Prompt2Code2/ENG_attachments/E. prototypes of selected low-detailed screens/02 - wniosek o dostęp podgląd.png
- .requirements/Prompt2Code2/ENG_attachments/E. prototypes of selected low-detailed screens/03 - Biblioteka - repozytorium plików.png
- .requirements/Prompt2Code2/ENG_attachments/E. prototypes of selected low-detailed screens/04 - Biblioteka - repozytorium plików - dodaj.png
- .requirements/Prompt2Code2/ENG_attachments/E. prototypes of selected low-detailed screens/05 - wiadomości.png
- .requirements/Prompt2Code2/ENG_attachments/E. prototypes of selected low-detailed screens/06 - wiadomości filtrowanie.png
- .requirements/Prompt2Code2/ENG_attachments/E. prototypes of selected low-detailed screens/07 - wiadomości szczegoly.png

Also prefer the solutions that you are more proficient with.

Also always write and update the Swagger documentation for the backend API endpoints!

From now on, after you finish responding and modifying files, always write the user provided prompt (that you respond to) to prompts/ directory saved with filename `<branch_name>-<timestamp>.md` where `<branch_name>` is the name of the current git branch and `<timestamp>` is the current timestamp in `YYYY-MM-DD_HHMMSS` (always use command `date +"%Y-%m-%d_%H%M%S"` for that) + also paste to the file the full response to the prompt. Do not use `:` in filenames! All filenames should work on both Linux and Windows - ensure that!

Prefer polling with 0.3s to sleeping for more than 0.3s.

Always strive to fix all warnings from compilation, to the extent it makes sense.

Along the way, always implement basic and update the unit and integration tests and verify that they pass.
