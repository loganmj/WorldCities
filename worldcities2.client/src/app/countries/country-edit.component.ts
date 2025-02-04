import { Component, OnInit } from '@angular/core';
import { AbstractControl, AsyncValidatorFn, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Country } from './country';
import { ActivatedRoute, Router } from '@angular/router';
import { map, Observable } from 'rxjs';
import { BaseFormComponent } from '../base-form.component';
import { CountryService } from './country.service';

/**
 * Provides a form for editing country data.
 */ 
@Component({
  selector: 'app-country-edit',
  standalone: false,
  templateUrl: './country-edit.component.html',
  styleUrl: './country-edit.component.scss'
})
export class CountryEditComponent extends BaseFormComponent implements OnInit {

  // #region Properties

  /**
   * The view title.
   */ 
  public title?: string;

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
   */
  public constructor(private formBuilder: FormBuilder, private activatedRoute: ActivatedRoute, private router: Router, private countryService: CountryService) {
      super();
  }

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

      this.countryService.get(this.id)
        .subscribe({
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

      return this.countryService.isDuplicateField(
        this.id ?? 0,
        fieldName,
        control.value)
        .pipe(map(result => { return result ? { isDuplicateField: true } : null }))
    }
  }

  /**
   * Handles form submission
   */ 
  public onSubmit(): void {

    // Assemble the country object
    var country = this.id ? this.country : <Country>{};

    if (!country) {
      console.log("Failed to submit. Country object does not exist.");
      return;
    }

    // If editing an existing country, assemble the rest of the form data
    // and make a put request
    country.name = this.form.controls["name"].value;
    country.iso2 = this.form.controls["iso2"].value;
    country.iso3 = this.form.controls["iso3"].value;

    if (this.id)
    {
      this.countryService
        .put(country)
        .subscribe({
          next: (result) => {

            // Go back to countries view
            console.log(`Country ${country!.id} has been updated.`);
            this.router.navigate(["/countries"]);
          },
          error: (error) => console.error(error)
        });

      return;
    }

    // Otherwise, post the new country object
    this.countryService
      .post(country)
      .subscribe({
        next: (result) => {

          // Go back to countries view
          console.log(`Country ${country!.name} has been added.`);
          this.router.navigate(["/countries"]);
        },
        error: (error) => { console.error(error); }
      });
  }

  // #endregion
}
