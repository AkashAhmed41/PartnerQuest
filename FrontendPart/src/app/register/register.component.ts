import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { AbstractControl, FormBuilder, FormGroup, ValidatorFn, Validators } from '@angular/forms';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter();
  userData: any = {};
  registerForm: FormGroup = new FormGroup({});

  constructor(private accountService: AccountService, private toastr: ToastrService, private formBuilder: FormBuilder) {}

  ngOnInit(): void {
    this.formInitializer();
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
    console.log(this.registerForm?.value);
    
    // this.accountService.register(this.userData).subscribe({
    //   next: () => {
    //     // console.log(response);
    //     this.cancel();
    //   },
    //   error: error => {
    //     console.log(error);
    //     this.toastr.error(error.error);
    //   }
    // });
  }

  cancel() {
    this.cancelRegister.emit(false);
  }

}
