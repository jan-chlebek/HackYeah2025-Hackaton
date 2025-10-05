import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-file-upload',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="file-upload-container">
      <h1>Upload File</h1>
      <p>Upload new file to library - empty</p>
    </div>
  `,
  styles: [`
    .file-upload-container {
      padding: 2rem;
    }
  `]
})
export class FileUploadComponent {}
