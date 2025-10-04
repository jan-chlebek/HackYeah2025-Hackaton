from __future__ import annotations

from django.http import JsonResponse
from django.views.decorators.http import require_GET


@require_GET
def health(request):
    """Return basic application health information."""
    return JsonResponse(
        {
            "status": "ok",
            "service": "hackyeah-backend",
        }
    )
