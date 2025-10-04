# HackYeah 2025 Project

This repository hosts the full-stack application for HackYeah 2025, featuring a React frontend and a Django backend.

## Project layout

- `Frontend/` — React 18 application built with TypeScript and Tailwind CSS (Create React App). See the [frontend README](Frontend/README.md).
- `Backend/` — Django 5 project that serves API endpoints. See the [backend README](Backend/README.md).

## Local development

You can run the frontend and backend independently during development:

1. Follow the setup instructions in each subdirectory's README.
2. Start the backend (`python manage.py runserver`) so API requests can be served under `http://localhost:8000/`.
3. Start the frontend (`npm start`) to serve the React app at `http://localhost:3000/`.

## Contributing

- Keep the TypeScript frontend and Django backend in sync when introducing new features.
- Add or update tests whenever behavior changes.
- Document configuration flags in the corresponding README files.
