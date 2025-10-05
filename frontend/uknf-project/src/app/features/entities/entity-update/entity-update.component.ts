import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-entity-update',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="entity-update-container">
      <h1>Update Entity Data</h1>
      <p>Update entity information - empty</p>
    </div>
  `,
  styles: [`
    .entity-update-container {
      padding: 2rem;
    }
  `]
})
export class EntityUpdateComponent {}
