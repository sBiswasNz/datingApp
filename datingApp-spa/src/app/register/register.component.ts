import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ThrowStmt } from '@angular/compiler';
import { AuthService } from '../_services/auth.service';
import { error } from 'protractor';
import { AlertifyService } from '../_services/alertify.service';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter();
  model: any = {};
  registerForm: FormGroup;
  bsDatepickerConfig: Partial<BsDatepickerConfig>;

  constructor(private authService: AuthService, private alertify: AlertifyService, private frb: FormBuilder) { }

  ngOnInit() {
    this.bsDatepickerConfig = {
      containerClass: 'theme-dark-blue'
    };
    this.createRegistrationForm();
/*     this.registerForm = new FormGroup({
      username: new FormControl('', Validators.required),
      password: new FormControl('', [Validators.required, Validators.maxLength(9), Validators.minLength(4)]),
      confirmPassword: new FormControl('', Validators.required)
    }, this.passwordMatchValidator); */
  }

  createRegistrationForm(){
    this.registerForm = this.frb.group({
      gender: ['Male'],
      username: ['', Validators.required],
      knownAs: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      city: ['', Validators.required],
      country : ['', Validators.required],
      password: ['', [Validators.required, Validators.maxLength(8), Validators.minLength(4)]],
      confirmPassword: ['', Validators.required]
    }, {validator: this.passwordMatchValidator});
  }

  passwordMatchValidator(fg: FormGroup){
    return fg.get('password').value === fg.get('confirmPassword').value ? null : {mismatch: true};
  }

  register(){
    /* this.authService.register(this.model).subscribe( next => {
      this.alertify.success('registration successful.. ');
    }, error => {
      this.alertify.error(error);
    }); */
    console.log(this.registerForm.value);
  }

  cancel(){
    this.cancelRegister.emit(false);
    console.log('cancelled');
  }

}
