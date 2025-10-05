import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-announcement-create',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="announcement-create-container">
      <h1>Create Announcement</h1>
      <p>Create new announcement - empty</p>
    </div>
  `,
  styles: [`
    .announcement-create-container {
      padding: 2rem;
    }
  `]
})
export class AnnouncementCreateComponent {}
