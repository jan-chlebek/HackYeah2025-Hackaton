import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-announcement-details',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="announcement-details-container">
      <h1>Announcement Details</h1>
      <p>Announcement details - empty</p>
    </div>
  `,
  styles: [`
    .announcement-details-container {
      padding: 2rem;
    }
  `]
})
export class AnnouncementDetailsComponent {}
