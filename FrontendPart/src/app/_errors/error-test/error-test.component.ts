import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-error-test',
  templateUrl: './error-test.component.html',
  styleUrls: ['./error-test.component.css']
})
export class ErrorTestComponent implements OnInit {
  baseUrl = 'http://localhost:5137/api/';
  validationErrors: string[] = [];

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    
  }

  get500Error() {
    this.http.get(this.baseUrl + 'error/server-error').subscribe({
      next: respose => console.log(respose),
      error: error => console.log(error)
    });
  }

  get401Error() {
    this.http.get(this.baseUrl + 'error/auth').subscribe({
      next: respose => console.log(respose),
      error: error => console.log(error)
    });
  }

  get404Error() {
    this.http.get(this.baseUrl + 'error/not-found').subscribe({
      next: respose => console.log(respose),
      error: error => console.log(error)
    });
  }

  get400Error() {
    this.http.get(this.baseUrl + 'error/bad-request').subscribe({
      next: respose => console.log(respose),
      error: error => console.log(error)
    });
  }

  get400ValidationError() {
    this.http.post(this.baseUrl + 'account/register', {}).subscribe({
      next: respose => console.log(respose),
      error: error => {
        console.log(error);
        this.validationErrors = error;
      }
    });
  }

}
