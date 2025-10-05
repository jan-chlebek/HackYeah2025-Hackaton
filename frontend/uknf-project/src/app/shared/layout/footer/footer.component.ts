import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [CommonModule],
  template: `
    <footer class="app-footer bg-primary-dark text-white py-4 px-4 mt-auto">
      <div class="grid">
        <div class="col-12 md:col-4">
          <h3 class="text-lg font-semibold mb-2">Platforma Komunikacyjna UKNF</h3>
          <p class="text-sm opacity-80">
            System komunikacji dla podmiotów nadzorowanych przez Urząd Komisji Nadzoru Finansowego
          </p>
        </div>

        <div class="col-12 md:col-4">
          <h4 class="text-base font-semibold mb-2">Przydatne linki</h4>
          <ul class="list-none p-0 m-0">
            <li class="mb-1">
              <a href="https://www.knf.gov.pl" target="_blank" rel="noopener" class="text-white opacity-80 hover:opacity-100 no-underline">
                <i class="pi pi-external-link mr-1"></i>
                Portal UKNF
              </a>
            </li>
            <li class="mb-1">
              <a href="/faq" class="text-white opacity-80 hover:opacity-100 no-underline">
                <i class="pi pi-question-circle mr-1"></i>
                FAQ
              </a>
            </li>
            <li class="mb-1">
              <a href="/contacts" class="text-white opacity-80 hover:opacity-100 no-underline">
                <i class="pi pi-phone mr-1"></i>
                Kontakt
              </a>
            </li>
          </ul>
        </div>

        <div class="col-12 md:col-4">
          <h4 class="text-base font-semibold mb-2">Pomoc i wsparcie</h4>
          <ul class="list-none p-0 m-0">
            <li class="mb-1">
              <i class="pi pi-phone mr-2"></i>
              <span class="text-sm opacity-80">+48 22 262 50 00</span>
            </li>
            <li class="mb-1">
              <i class="pi pi-envelope mr-2"></i>
              <span class="text-sm opacity-80">support&#64;uknf.gov.pl</span>
            </li>
            <li class="mb-1">
              <i class="pi pi-clock mr-2"></i>
              <span class="text-sm opacity-80">Pn-Pt: 8:00-16:00</span>
            </li>
          </ul>
        </div>
      </div>

      <div class="border-top-1 border-white-alpha-30 mt-3 pt-3">
        <div class="flex justify-content-between align-items-center flex-wrap gap-2">
          <p class="text-sm opacity-80 m-0">
            &copy; {{ currentYear }} Urząd Komisji Nadzoru Finansowego. Wszelkie prawa zastrzeżone.
          </p>
          <div class="flex gap-3">
            <a href="/polityka-prywatnosci" class="text-white text-sm opacity-80 hover:opacity-100 no-underline">
              Polityka prywatności
            </a>
            <span class="opacity-50">|</span>
            <a href="/regulamin" class="text-white text-sm opacity-80 hover:opacity-100 no-underline">
              Regulamin
            </a>
            <span class="opacity-50">|</span>
            <a href="/deklaracja-dostepnosci" class="text-white text-sm opacity-80 hover:opacity-100 no-underline">
              Deklaracja dostępności
            </a>
          </div>
        </div>
      </div>
    </footer>
  `,
  styles: [`
    .app-footer {
      margin-top: auto;
    }

    a {
      transition: opacity 0.2s ease;
    }

    .grid {
      max-width: 1200px;
      margin: 0 auto;
    }
  `]
})
export class FooterComponent {
  currentYear = new Date().getFullYear();
}
