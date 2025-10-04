import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-case-create',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="case-create-container">
      <h1>Create Case</h1>
      <p>Create new administrative case - empty</p>
    </div>
  `,
  styles: [`
    .case-create-container {
      padding: 2rem;
    }
  `]
})
export class CaseCreateComponent {}
