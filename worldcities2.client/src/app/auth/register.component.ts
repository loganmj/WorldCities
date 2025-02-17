import { Component, OnInit } from '@angular/core';
import { BaseFormComponent } from '../base-form.component';
import { RegisterResult } from './register-result';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from './auth.service';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { RegisterRequest } from './register-request';

/**
 * Allows a user to create a new account.
 */ 
@Component({
  selector: 'app-register',
  standalone: false,
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent extends BaseFormComponent implements OnInit {

  // #region Properties

  /**
   * The title of the component.
   */ 
  public title?: string = "Create Account";

  /**
   * The registration result object.
   */ 
  public registerResult?: RegisterResult;

  // #endregion

  // #region Constructors

  /**
   * Constructor
   */
  public constructor(private activatedRoute: ActivatedRoute, private router: Router, private authService: AuthService) {
    super();
  }

  // #endregion

  // #region Public Methods

  /**
   * On Init
   */ 
  public ngOnInit(): void {
    this.form = new FormGroup({
      email: new FormControl('', Validators.required),
      password: new FormControl('', Validators.required)
    });
  }

  /**
   * Handles a submission request.
   */ 
  public onSubmit(): void {
    var registerRequest = <RegisterRequest>{};
    registerRequest.email = this.form.controls['email'].value;
    registerRequest.password = this.form.controls['password'].value;

    /* This is the login code, we need to handle registration instead

    this.authService
      .login(loginRequest)
      .subscribe({
        next: (result) => {
          console.log(result);
          this.loginResult = result;

          // If the login was successful, navigate back to home
          if (result.success) {
            this.router.navigate(["/"]);
          }
        },
        error: (error) => {

          // Marshal login result if we receive UnauthorizedError
          if (error.status == 401) {
            this.loginResult = error.error;
          }
        }
      });

      */
  }

  // #endregion

}
