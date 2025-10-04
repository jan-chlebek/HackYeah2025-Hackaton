# HackYeah 2025 Backend

This directory contains the Django backend that powers the HackYeah 2025 project. It exposes a REST-friendly foundation with environment-driven configuration, CORS support for the React frontend, and a basic health check endpoint you can extend with new business logic.

## Prerequisites

- Python 3.11+
- pip (ships with recent Python installations)

## Quick start

1. Create a virtual environment:
   ```pwsh
   py -3.11 -m venv .venv
   .\.venv\Scripts\Activate.ps1
   ```
2. Install dependencies:
   ```pwsh
   pip install -r requirements.txt
   ```
3. Copy the environment template and adjust as needed:
   ```pwsh
   Copy-Item .env.example .env
   ```
4. Run database migrations and start the development server:
   ```pwsh
   python manage.py migrate
   python manage.py runserver
   ```

The API will be available at `http://127.0.0.1:8000/api/` and the health check at `http://127.0.0.1:8000/api/health/`.

## Running tests

```pwsh
python manage.py test
```

## Project layout

- `hackyeah_backend/` — Django project configuration
- `api/` — Example application with a health check endpoint
- `requirements.txt` — Prod + dev dependencies (install inside a virtualenv)
- `.env.example` — Safe defaults for local development

## Next steps

- Add models, serializers, and views inside `api/` or new apps
- Configure CORS origins/credentials in `.env` when deploying
- Wire up authentication (e.g., JWT) as requirements evolve
