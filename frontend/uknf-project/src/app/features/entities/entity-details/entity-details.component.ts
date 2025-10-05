import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-entity-details',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="entity-details-container">
      <h1>Entity Details</h1>
      <p>Entity details and history - empty</p>
    </div>
  `,
  styles: [`
    .entity-details-container {
      padding: 2rem;
    }
  `]
})
export class EntityDetailsComponent {}
