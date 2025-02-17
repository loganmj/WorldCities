import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { AuthService } from '../auth/auth.service';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrl: './nav-menu.component.scss',
  standalone: false
})
export class NavMenuComponent implements OnInit, OnDestroy{

  // #region Fields

  private _destroySubject = new Subject();

  // #endregion

  // #region Properties

  /**
   * Is the user logged in.
   */ 
  public isLoggedIn: boolean = false;

  // #endregion

  // #region Constructors

  /**
   * Constructor
   */ 
  public constructor(private authService: AuthService, private router: Router) {
    this.authService.authStatus
      .pipe(takeUntil(this._destroySubject))
      .subscribe(result => {
        this.isLoggedIn = result;
      })
  }

  // #endregion

  // #region Public Methods

  /**
   * Handles a user logout.
   */ 
  public onLogout(): void {
    this.authService.logout();
    this.router.navigate(["/"]);
  }

  /**
   * On init.
   */ 
  public ngOnInit(): void {
    this.isLoggedIn = this.authService.isAuthenticated();
  }

  /**
   * On destroy
   */ 
  public ngOnDestroy(): void {
    this._destroySubject.next(true);
    this._destroySubject.complete();
  }

  // #endregion
}
