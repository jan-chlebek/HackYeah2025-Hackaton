import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-announcements-list',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="announcements-list-container">
      <h1>Announcements (Komunikaty)</h1>
      <p>Bulletin board - empty</p>
    </div>
  `,
  styles: [`
    .announcements-list-container {
      padding: 2rem;
    }
  `]
})
export class AnnouncementsListComponent {}
