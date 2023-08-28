import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, delay, finalize } from 'rxjs';
import { LoadingIndicatorService } from '../_services/loading-indicator.service';

@Injectable()
export class LoadingInterceptor implements HttpInterceptor {

  constructor(private loadingIndicatorService: LoadingIndicatorService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    this.loadingIndicatorService.requestIncoming();

    return next.handle(request).pipe(
      delay(500),
      finalize(() => {
        this.loadingIndicatorService.requestCompleted()
      })
    );
  }
}
