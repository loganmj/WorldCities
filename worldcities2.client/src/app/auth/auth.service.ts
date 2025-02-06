import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { LoginRequest } from "./login-request";
import { LoginResult } from "./login-result";
import { Observable, tap } from "rxjs";
import { environment } from "../../environments/environment";

/**
 * A JWT authentication service.
 */ 
@Injectable({
  providedIn: 'root'
})
export class AuthService {

  // #region Properties

  /**
   * The JWT token key.
   */ 
  public tokenKey: string = "token";

  // #endregion

  // #region Constructors

  /**
   * Constructor
   */ 
  constructor(protected http: HttpClient) { }

  // #endregion

  // #region Public Methods

  /**
   * Checks if we have a local authentication token.
   */ 
  public isAuthenticated(): boolean {
    return this.getToken() !== null;
  }

  /**
   * Attempts to retrieve a stored authentication token.
   */ 
  public getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  /**
   * Attempts to log in.
   * On success, creates and stores an authentication token.
   */ 
  public login(request: LoginRequest): Observable<LoginResult> {

    var url = `${environment.baseUrl}api/Account/Login`;

    return this.http.post<LoginResult>(url, request)
      .pipe(tap(loginResult => {
        if (loginResult.success && loginResult.token) {
          localStorage.setItem(this.tokenKey, loginResult.token);
        }
      }));
  }

  // #endregion
}
