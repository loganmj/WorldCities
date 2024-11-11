import { Component, OnInit, ViewChild, viewChild } from '@angular/core';
import { City } from './city';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';

@Component({
  selector: 'app-cities',
  templateUrl: './cities.component.html',
  styleUrl: './cities.component.scss'
})

/**
 * A data transfer component for City data objects.
 */
export class CitiesComponent implements OnInit {

  // #region Properties

  /**
   * The columns to display in the data table.
   */
  public displayedColumns: string[] = ['id', 'name', 'latitude', 'longitude'];

  /**
   * The '!' character (definite assignment assertion operator)
   * tells TypeScript that this property will be assigned a value before it is used,
   * even though that cannot be confirmed at compile time.
   */
  public cities!: MatTableDataSource<City>;

  /**
   * A reference to the paginator in the DOM.
   */ 
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  // #endregion

  // #region Constructors

  /**
   * Constructs a component with a given http client object.
   * The shorthand in the parameter creates a private HttpClient field
   * that is accessable by the rest of the class.
   */ 
  constructor(private http: HttpClient) { }

  // #endregion

  // #region Public Methods

  /**
   * A lifecycle hook that gets called when the component is initialized.
   */ 
  public ngOnInit(): void {
    this.http.get<City[]>(environment.baseUrl + 'api/Cities').subscribe({
      next: (result) => {
        this.cities = new MatTableDataSource<City>(result);
        this.cities.paginator = this.paginator;
      },
      error: (error) => console.error(error)
    });
  }

  // #endregion
}
