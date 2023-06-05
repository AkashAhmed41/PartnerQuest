import { Injectable } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable({
  providedIn: 'root'
})
export class LoadingIndicatorService {
  httpRequestCount = 0;

  constructor(private spinner: NgxSpinnerService) { }

  requestIncoming() {
    this.httpRequestCount++;
    this.spinner.show(undefined, {
      type: 'ball-spin-clockwise',
      bdColor: 'rgba(0, 0, 0, 0.8)',
      color: '#fff'
    })
  }

  requestCompleted() {
    this.httpRequestCount--;
    if(this.httpRequestCount <= 0) {
      this.httpRequestCount = 0;
      this.spinner.hide();
    }
  }
}
