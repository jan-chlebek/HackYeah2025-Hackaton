import { TestBed } from '@angular/core/testing';
import { provideHttpClient, withFetch } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { MessagesListComponent } from './messages-list.component';
import { MessageService } from '../../../services/message.service';
import { authTokenInterceptor } from '../../../interceptors/auth-token.interceptor';
import { provideRouter } from '@angular/router';
import { withInterceptors } from '@angular/common/http';

describe('MessagesListComponent', () => {
  let httpMock: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MessagesListComponent],
      providers: [
        MessageService,
        provideRouter([]),
        provideHttpClient(withFetch(), withInterceptors([authTokenInterceptor])),
        provideHttpClientTesting()
      ]
    }).compileComponents();

    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should create and call messages endpoint', () => {
    const fixture = TestBed.createComponent(MessagesListComponent);
    const component = fixture.componentInstance;
    fixture.detectChanges();

    // initial load deferred via setTimeout; flush macro tasks
    const req = httpMock.expectOne(r => r.url.includes('/api/v1/messages'));
    expect(req.request.method).toBe('GET');
    req.flush({ data: [], pagination: { page:1, pageSize:10, totalCount:0, totalPages:0 } });
  });
});
