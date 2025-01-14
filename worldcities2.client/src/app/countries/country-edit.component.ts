import { Component, OnInit } from '@angular/core';
import { AbstractControl, AsyncValidatorFn, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Country } from './country';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { map, Observable } from 'rxjs';
import { environment } from '../../environments/environment';

/**
 * Provides a form for editing country data.
 */ 
@Component({
  selector: 'app-country-edit',
  standalone: false,
  
  templateUrl: './country-edit.component.html',
  styleUrl: './country-edit.component.scss'
})
export class CountryEditComponent implements OnInit {

  // #region Properties

  /**
   * The view title.
   */ 
  public title?: string;

  /**
   * The form model.
   */ 
  public form!: FormGroup;

  /**
   * The country object to edit or create.
   */ 
  public country?: Country;

  /**
   * The country object id, as fetched from the active route.
   * NULL when we are adding a new country.
   * Not NULL when we are editing an existing country.
   */ 
  public id?: number;

  /**
   * The countries array for the select component.
   */ 
  countries?: Country[];

  // #endregion

  // #region Constructors

  /**
   * Constructor
   * @param formBuilder
   * @param activatedRoute
   * @param router
   * @param http
   */ 
  public constructor(private formBuilder: FormBuilder, private activatedRoute: ActivatedRoute, private router: Router, private http: HttpClient) { }

  // #endregion

  // #region Public Methods

  /**
   * Lifecycle method that runs when the component is initialized.
   */ 
  public ngOnInit() {

    // Build the form
    this.form = this.formBuilder.group ({
      name: ['', Validators.required, this.isDuplicateField("name")],
      iso2: ['', [Validators.required, Validators.pattern(/^[a-zA-Z]{2}$/)], this.isDuplicateField("iso2")],
      iso3: ['', [Validators.required, Validators.pattern(/^[a-zA-Z]{3}$/)], this.isDuplicateField("iso3")]
    });

    // Load the country data
    this.loadData();
  }



  /**
   * Loads data from the database.
   */ 
  public loadData(): void {

    // Retrieve the ID from the 'id' property of the country
    // Set to 0 if creating a new country
    var idParameter = this.activatedRoute.snapshot.paramMap.get('id');
    this.id = idParameter ? +idParameter : 0;

    // If editing an existing ID, fetch the country from the server
    if (this.id) {

      var url = `${environment.baseUrl}api/Countries/${this.id}`;

      this.http.get<Country>(url).subscribe({
        next: (result) => {
          this.country = result;
          this.title = `Edit - ${this.country.name}`;

          // Update the form with the country value
          this.form.patchValue(this.country);
        },
        error: (error) => { console.error(error); }
      });

      return;
    }

    // Otherwise, setup the form to create a new country
    this.title = "Create a new Country";
  }

  /**
   * Checks if the field value already exists in the country database.
   * Returns a validator method.
   */
  public isDuplicateField(fieldName: string): AsyncValidatorFn {

    // The returned method takes a form control as a parameter, and returns an observable.
    return (control: AbstractControl): Observable<{ [key: string]: any } | null> => {

      // Create http parameters
      var params = new HttpParams()
        .set("countryId", (this.id) ? this.id.toString() : "0")
        .set("fieldName", fieldName)
        .set("fieldValue", control.value);

      // Makes an http post request, and returns the result of the IsDuplicateField method from the API controller.
      var url = `${environment.baseUrl}api/Countries/IsDuplicateField`;
      return this.http.post<boolean>(url, null, { params })
        .pipe(map(result => {
          return (result ? { isDuplicateField: true } : null);
        }));
    }
  }

  /**
   * Handles form submission
   */ 
  public onSubmit(): void {

    // Assemble the country object
    var country = (this.id) ? this.country : <Country>{};

    if (country) {
      country.name = this.form.controls["name"].value;
      country.iso2 = this.form.controls["iso2"].value;
      country.iso3 = this.form.controls["iso3"].value;
    }

    // If editing an existing country, make a put request
    if (this.id) {
      var url = `${environment.baseUrl}api/Countries/${this.id}`;

      this.http.put<Country>(url, country)
        .subscribe({
          next: (result) => {
            console.log(`Country ${country!.id} has been updated.`);

            // Go back to countries view
            this.router.navigate(["/countries"]);
          },
          error: (error) => { console.error(error) }
        });
    }

    // Otherwise, post the new country object
    var url = `${environment.baseUrl}api/Countries`;

    this.http.post<Country>(url, country)
      .subscribe({
        next: (result) => {
          console.log(`Country ${country!.name} has been added.`);

          // Go back to countries view
          this.router.navigate(["/countries"]);
        },
        error: (error) => { console.error(error); }
      });
  }

  // #endregion
}
