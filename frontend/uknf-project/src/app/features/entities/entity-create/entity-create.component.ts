import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-entity-create',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="entity-create-container">
      <h1>Create Entity</h1>
      <p>Create new supervised entity - empty</p>
    </div>
  `,
  styles: [`
    .entity-create-container {
      padding: 2rem;
    }
  `]
})
export class EntityCreateComponent {}
