import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-cases-list',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="cases-list-container">
      <h1>Cases (Sprawy)</h1>
      <p>List of administrative cases - empty</p>
    </div>
  `,
  styles: [`
    .cases-list-container {
      padding: 2rem;
    }
  `]
})
export class CasesListComponent {}
