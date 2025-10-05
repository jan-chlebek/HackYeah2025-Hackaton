import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AccessibilityControlsComponent } from './accessibility-controls.component';
import { AccessibilityService } from '../../services/accessibility.service';

class MockAccessibilityService {
  fontSize: 'small' | 'medium' | 'large' = 'medium';
  highContrast = false;

  setFontSize(size: 'small' | 'medium' | 'large'): void {
    this.fontSize = size;
  }

  toggleHighContrast(): void {
    this.highContrast = !this.highContrast;
  }
}

describe('AccessibilityControlsComponent', () => {
  let component: AccessibilityControlsComponent;
  let fixture: ComponentFixture<AccessibilityControlsComponent>;
  let service: MockAccessibilityService;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AccessibilityControlsComponent],
      providers: [{ provide: AccessibilityService, useClass: MockAccessibilityService }]
    }).compileComponents();

    fixture = TestBed.createComponent(AccessibilityControlsComponent);
    component = fixture.componentInstance;
    service = TestBed.inject(AccessibilityService) as unknown as MockAccessibilityService;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should toggle panel visibility', () => {
    expect(component['panelOpen']()).toBeFalse();
    component.togglePanel();
    expect(component['panelOpen']()).toBeTrue();
  });

  it('should change font size via service', () => {
    component.setFontSize('large');
    expect(service.fontSize).toBe('large');
  });

  it('should toggle high contrast via service', () => {
    component.toggleContrast();
    expect(service.highContrast).toBeTrue();
  });
});
