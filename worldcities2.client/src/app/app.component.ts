import { Component, OnInit } from '@angular/core';
import { AuthService } from './auth/auth.service';

/**
 * Main app component.
 */ 
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
  standalone: false
})
export class AppComponent implements OnInit {

  // #region Properties

  title = 'worldcities2.client';

  // #endregion

  // #region Constructors

  /**
   * Constructor
   */ 
  public constructor(private authService: AuthService) { }

  // #endregion

  // #region Public Methods

  /**
   * On init
   */ 
  ngOnInit() {
    this.authService.init();
  }

  // #endregion
}
