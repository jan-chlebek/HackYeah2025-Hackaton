import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-entities-list',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="entities-list-container">
      <h1>Entity Registry (Kartoteka Podmiotów)</h1>
      <p>List of supervised entities - empty</p>
    </div>
  `,
  styles: [`
    .entities-list-container {
      padding: 2rem;
    }
  `]
})
export class EntitiesListComponent {}
