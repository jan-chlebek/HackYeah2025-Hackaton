import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-library-list',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="library-list-container">
      <h1>Library (Biblioteka)</h1>
      <p>File library - empty</p>
    </div>
  `,
  styles: [`
    .library-list-container {
      padding: 2rem;
    }
  `]
})
export class LibraryListComponent {}
