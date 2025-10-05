import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-file-details',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="file-details-container">
      <h1>File Details</h1>
      <p>File details and metadata - empty</p>
    </div>
  `,
  styles: [`
    .file-details-container {
      padding: 2rem;
    }
  `]
})
export class FileDetailsComponent {}
