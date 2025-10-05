import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-select-entity',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="select-entity-container">
      <h1>Select Entity</h1>
      <p>Select entity to represent - empty</p>
    </div>
  `,
  styles: [`
    .select-entity-container {
      padding: 2rem;
    }
  `]
})
export class SelectEntityComponent {}
