import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { City } from './city';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { environment } from '../../environments/environment';
import { Country } from '../countries/country';

/**
 * A component that allows the user to edit a city.
 */ 
@Component({
  selector: 'app-city-edit',
  standalone: false,
  templateUrl: './city-edit.component.html',
  styleUrl: './city-edit.component.scss'
})
export class CityEditComponent implements OnInit {

  // #region Properties

  /**
   * The view title.
   */ 
  public title?: string;

  /**
   * A reference to the form model.
   */ 
  public form!: FormGroup;

  /**
   * The city being edited.
   */ 
  public city?: City;

  /**
   * The city object id, as fetched from the active route.
   * This number is null when adding a new city,
   * and not null when editing an existing city.
   */ 
  public id?: number;

  /**
   * The list of countries for the select component.
   */ 
  public countries?: Country[];

  // #endregion

  // #region Constructors

  /**
   * Initializes a new instance of the CityEditComponent class.
   */ 
  public constructor(private activatedRoute: ActivatedRoute, private router: Router, private http: HttpClient) { }

  // #endregion

  // #region Public Methods

  /**
   * Called when the component is initialized.
   */
  public ngOnInit(): void {
    this.form = new FormGroup({
      name: new FormControl(''),
      latitude: new FormControl(''),
      longitude: new FormControl(''),
      countryId: new FormControl('')
    });

    this.loadData();
  }

  /**
   * Loads the city data.
   */ 
  public loadData(): void {

    // Load countries
    this.loadCountries();

    // Retrieve the ID of the currently selected city in the table
    var idParam = this.activatedRoute.snapshot.paramMap.get('id');
    this.id = idParam ? +idParam : 0;

    // Edit mode
    if (this.id) {

      // Fetch the city from the server.
      var url = environment.baseUrl + `api/cities/${this.id}`;

      // NOTE: This is called AJAX. It is asynchronous data communication.
      this.http.get<City>(url).subscribe({
        next: (result) => {
          this.city = result;
          this.title = `Edit - ${this.city.name}`;

          // Update the form with the city value.
          this.form.patchValue(this.city);
        },
        error: (error) => console.error(error)
      });

      return;
    }

    // Add new node
    this.title = "Create a new city";
  }

  /**
   * Loads the countries from the server.
   */ 
  public loadCountries(): void {

    // Fetch all the countries from the server
    var url = `${environment.baseUrl}api/countries`;
    var params = new HttpParams()
      .set("pageIndex", "0")
      .set("pageSize", "9999")
      .set("sortColumn", "name");

    this.http.get<any>(url, { params })
      .subscribe({
        next: (result) => {
          this.countries = result.data;
        },
        error: (error) => { console.error(error); }
      });
  }

  /**
   * Called when the user clicks the submit button.
   */ 
  public onSubmit(): void {
    var city = (this.id) ? this.city : <City>{};

    // If city is falsey, return.
    if (!city) {
      return;
    }

    // Set city data
    city.name = this.form.controls['name'].value;
    city.latitude = +this.form.controls['latitude'].value;
    city.longitude = +this.form.controls['longitude'].value;
    city.countryID = +this.form.controls['countryId'].value;

    // Edit city
    if (this.id) {

      // Put the city data to the server.
      var url = `${environment.baseUrl}api/Cities/${city.id}`;

      this.http
        .put<City>(url, city)
        .subscribe({
          next: (result) => {
            console.log(`City ${city!.id} has been updated.`);
            this.router.navigate(['/cities']);
          },
          error: (error) => { console.error(error); }
        });

      return;
    }

    // Post a new city to the server.
    var url = `${environment.baseUrl}api/Cities`;

    this.http
      .post<City>(url, city)
      .subscribe({
        next: (result) => {
          console.log(`City ${result.id} has been created.`);
          this.router.navigate(['/cities']);
        },
        error: (error) => { console.error(error); }
      });

    /*

      var url = environment.baseUrl + `api/cities/${city.id}`;

      this.http.put<City>(url, city).subscribe({
        next: (result) => {
          console.log(`City {{city!.id}} has been updated.`);

          // Go back to cities view
          this.router.navigate(['/cities']);
        },
        error: (error) => { console.error(error); }
      });
    }

    */
  }

  // #endregion
}
