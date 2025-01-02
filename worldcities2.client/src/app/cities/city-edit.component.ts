import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { City } from './city';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { environment } from '../../environments/environment';

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

  // #endregion

  // #region Constructors

  /**
   * Initializes a new instance of the CityEditComponent class.
   */ 
  public constructor(private activatedRoute: ActivatedRoute, private router: Router, private http: HttpClient) { }

  // #endregion

  // #region Public Methods

  public ngOnInit() {
    this.form = new FormGroup({
      name: new FormControl(''),
      latitude: new FormControl(''),
      longitude: new FormControl('')
    });

    this.loadData();
  }

  public loadData() {

    // Retrieve the ID of the currently selected city in the table
    var idParam = this.activatedRoute.snapshot.paramMap.get('id');
    var id = idParam ? parseInt(idParam) : 0;

    // Fetch the city from the server.
    var url = environment.baseUrl + `api/cities/${id}`;

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
  }

  public onSubmit() {
    var city = this.city;

    if (city) {
      city.name = this.form.controls['name'].value;
      city.latitude = this.form.controls['latitude'].value;
      city.longitude = this.form.controls['longitude'].value;

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
  }

  // #endregion
}
