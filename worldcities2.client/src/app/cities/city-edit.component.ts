import { Component, OnInit } from '@angular/core';
import { AbstractControl, AsyncValidatorFn, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { City } from './city';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { environment } from '../../environments/environment';
import { Country } from '../countries/country';
import { map, Observable } from 'rxjs';
import { BaseFormComponent } from '../base-form.component';
import { CityService } from './city.service';

/**
 * A component that allows the user to edit a city.
 */ 
@Component({
  selector: 'app-city-edit',
  standalone: false,
  templateUrl: './city-edit.component.html',
  styleUrl: './city-edit.component.scss'
})
export class CityEditComponent extends BaseFormComponent implements OnInit {

  // #region Properties

  /**
   * The view title.
   */ 
  public title?: string;

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
  public constructor(private activatedRoute: ActivatedRoute, private router: Router, private http: HttpClient, private formBuilder: FormBuilder, private cityService: CityService) {
    super();
  }

  // #endregion

  // #region Public Methods

  /**
   * Called when the component is initialized.
   */
  public ngOnInit(): void {

    // Build form
    this.form = this.formBuilder.group({
      name: ['', Validators.required],
      latitude: ['', [Validators.required, Validators.pattern(/^-?\d+(\.\d{1,4})?$/)]],
      longitude: ['', [Validators.required, Validators.pattern(/^-?\d+(\.\d{1,4})?$/)]],
      countryId: ['', Validators.required]
    }, { asyncValidators: this.isDuplicateCity() });

    this.loadData();
  }

  /**
   * Checks if the city is a duplicate.
   */ 
  isDuplicateCity(): AsyncValidatorFn {

    // Return a function that takes a control as an argument.
    return (control: AbstractControl): Observable<{ [key: string]: any } | null> => {

      // Create a new city object
      var city = <City>{};
      city.id = (this.id) ? this.id : 0;
      city.name = this.form.controls['name'].value;
      city.latitude = +this.form.controls['latitude'].value;
      city.longitude = +this.form.controls['longitude'].value;
      city.countryID = +this.form.controls['countryId'].value;

      // Call the server to check if the city is a duplicate
      return this.cityService.isDuplicateCity(city)
        .pipe(map(result => {
          return (result ? { isDuplicateCity: true } : null); 
        }));
    };
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

      this.cityService.get(this.id).subscribe({
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
    this.cityService.getCountries(0, 9999, "name", "asc", null, null)
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

    // Update the given city.
    if (this.id) {
      this.cityService
        .put(city)
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
    this.cityService
      .post(city)
      .subscribe({
        next: (result) => {
          console.log(`City ${result.id} has been created.`);
          this.router.navigate(['/cities']);
        },
        error: (error) => { console.error(error); }
      });
  }

  // #endregion
}
