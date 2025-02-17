import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { catchError, Observable, throwError } from "rxjs";
import { AuthService } from "./auth.service";

/**
 * An interceptor class for checking authorization.
 */ 
@Injectable({
  providedIn: 'root'
})
export class AuthInterceptor implements HttpInterceptor {

  // #region Properties
  // #endregion

  // #region Constructors

  /**
   * Constructor
   */ 
  public constructor(private authService: AuthService, private router: Router) { }

  // #endregion

  // #region Public Methods

  /**
   * Perform intercept.
   */ 
  public intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

    // Get the auth token
    var token = this.authService.getToken();

    // If the token is present, clone the request,
    // replacing the original headers with the authorization
    if (token) {
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      });
    }

    // Send the request to the next handler
    return next.handle(request).pipe(
      catchError((error) => {

        // Perform logout on 401 0 Unauthorized HTTP response errors
        if (error instanceof HttpErrorResponse && error.status === 401) {
          this.authService.logout();
          this.router.navigate(['/login']);
        }

        return throwError(() => error);
      })
    );
  }

  // #endregion
}
