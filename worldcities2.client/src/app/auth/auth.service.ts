import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { LoginRequest } from "./login-request";
import { LoginResult } from "./login-result";
import { BehaviorSubject, Observable, tap } from "rxjs";
import { environment } from "../../environments/environment";
import { RegisterRequest } from "./register-request";
import { RegisterResult } from "./register-result";

/**
 * A JWT authentication service.
 */ 
@Injectable({
  providedIn: 'root'
})
export class AuthService {

  // #region Fields

  private _tokenKey: string = "token";
  private _authStatus = new BehaviorSubject<boolean>(false);

  // #endregion

  // #region Properties

  /**
   * Observable authentication status property.
   */ 
  public authStatus = this._authStatus.asObservable();

  // #endregion

  // #region Constructors

  /**
   * Constructor
   */ 
  constructor(protected http: HttpClient) { }

  // #endregion

  // #region Private Methods

  /**
   * Sets the authStatus property
   */ 
  private setAuthStatus(isAuthenticated: boolean): void {
    this._authStatus.next(isAuthenticated);
  }

  // #endregion

  // #region Public Methods

  /**
   * Initializes the observable authentication status property.
   */ 
  public init(): void {
    if (this.isAuthenticated()) {
      this.setAuthStatus(true);
    }
  }

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
    return localStorage.getItem(this._tokenKey);
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
          localStorage.setItem(this._tokenKey, loginResult.token);
          this.setAuthStatus(true);
        }
      }));
  }

  /**
   * Logs the user out.
   */ 
  public logout(): void {
    localStorage.removeItem(this._tokenKey);
    this.setAuthStatus(false);
  }

  /**
   * Attempts to register a new user account.
   */ 
  public register(request: RegisterRequest): Observable<RegisterResult> {
    var url = `${environment.baseUrl}api/Account/Register`;
    return this.http.post<RegisterResult>(url, request);
  }

  // #endregion
}
