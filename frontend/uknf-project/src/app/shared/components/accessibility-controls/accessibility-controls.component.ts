import { booleanAttribute, Component, HostBinding, Input, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AccessibilityService, FontSize } from '../../services/accessibility.service';

@Component({
  selector: 'app-accessibility-controls',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './accessibility-controls.component.html',
  styleUrls: ['./accessibility-controls.component.scss']
})
export class AccessibilityControlsComponent {
  private readonly accessibilityService = inject(AccessibilityService);

  @Input({ transform: booleanAttribute }) forceVisible = false;

  @HostBinding('class.force-visible')
  protected get hostForceVisible(): boolean {
    return this.forceVisible;
  }

  protected readonly panelOpen = signal(false);
  protected readonly currentFontSize = computed<FontSize>(() => this.accessibilityService.fontSize);
  protected readonly highContrastMode = computed(() => this.accessibilityService.highContrast);

  togglePanel(): void {
    this.panelOpen.update(value => !value);
  }

  closePanel(): void {
    this.panelOpen.set(false);
  }

  setFontSize(size: FontSize): void {
    this.accessibilityService.setFontSize(size);
  }

  toggleContrast(): void {
    this.accessibilityService.toggleHighContrast();
  }
}
