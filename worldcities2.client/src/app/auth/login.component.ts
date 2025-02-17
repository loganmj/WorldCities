import { Component, OnInit } from '@angular/core';
import { BaseFormComponent } from '../base-form.component';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from './auth.service';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { LoginRequest } from './login-request';
import { LoginResult } from './login-result';

/**
 * Allows the user to log in to the app.
 */ 
@Component({
  selector: 'app-login',
  standalone: false,
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent extends BaseFormComponent implements OnInit {

  // #region Properties

  /**
   * The title of the component.
   */ 
  title?: string;

  /**
   * The login result reference.
   */
  loginResult?: LoginResult;

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
   * On initialization lifecycle method.
   */ 
  public ngOnInit() : void {
    this.form = new FormGroup({
      email: new FormControl('', Validators.required),
      password: new FormControl('', Validators.required)
    });
  }

  /**
   * Handles a submission request.
   */ 
  public onSubmit(): void {
    var loginRequest = <LoginRequest>{};
    loginRequest.email = this.form.controls['email'].value;
    loginRequest.password = this.form.controls['password'].value;

    this.authService
      .login(loginRequest)
      .subscribe({
        next: (result) => {
          console.log(result);
          this.loginResult = result;

          // If the login was successful, check for a return URL (in case the user was trying to log in to access a specific page)
          // If there is no return url, navigate h
          if (result.success) {
            this.router.navigateByUrl(this.activatedRoute.snapshot.queryParams['returnUrl'] || '/');
          }
        },
        error: (error) => {

          // Marshal login result if we receive UnauthorizedError
          if (error.status == 401) {
            this.loginResult = error.error;
          }
        }
      });
  }

  // #endregion

}
