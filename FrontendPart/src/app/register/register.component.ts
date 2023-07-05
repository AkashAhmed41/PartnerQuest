import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { AbstractControl, FormBuilder, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter();
  registerForm: FormGroup = new FormGroup({});
  maxDate: Date = new Date();
  validationErrors: string[] | undefined;

  constructor(private accountService: AccountService, private formBuilder: FormBuilder, private router: Router) {}

  ngOnInit(): void {
    this.formInitializer();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);
  }

  formInitializer() {
    this.registerForm = this.formBuilder.group({
      gender: ['male'],
      username: ['', Validators.required],
      nickname: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword: ['', [Validators.required, this.matchPasswords('password')]]
    });
    this.registerForm.controls['password'].valueChanges.subscribe({
      next: () => {
        this.registerForm.controls['confirmPassword'].updateValueAndValidity();
      }
    })
  }

  matchPasswords(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control.value === control.parent?.get(matchTo)?.value ? null : {notMatching: true};
    }
  }

  register() {
    const extractedDate = this.extractDateOnly(this.registerForm.controls['dateOfBirth'].value);
    const formValues = {...this.registerForm.value, dateOfBirth: extractedDate};
    
    this.accountService.register(formValues).subscribe({
      next: () => {
        this.router.navigateByUrl('/members');
      },
      error: error => {
        this.validationErrors = error;
      }
    });
  }

  cancel() {
    this.cancelRegister.emit(false);
  }

  private extractDateOnly(longDobOfForm: string | undefined) {
    if (!longDobOfForm) return;
    let theObjectDob = new Date(longDobOfForm);
    return new Date(theObjectDob.setMinutes(theObjectDob.getMinutes() - theObjectDob.getTimezoneOffset())).toISOString().slice(0, 10);
  }

}
