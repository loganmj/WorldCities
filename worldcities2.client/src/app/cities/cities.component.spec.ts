import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CitiesComponent } from './cities.component';
import { AngularMaterialModule } from '../angular-material.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterTestingModule } from '@angular/router/testing';
import { CityService } from './city.service';
import { of } from 'rxjs';
import { ApiResult } from '../apiResult';
import { City } from './city';

/**
 * Describing our test component.
 */ 
describe('CitiesComponent', () => {

  // Create fixture and component references
  let fixture: ComponentFixture<CitiesComponent>;
  let component: CitiesComponent;

  beforeEach(async () => {

    // Create a mock cityService object with a mock 'getData' method
    let cityService = jasmine.createSpyObj<CityService>('CityService', ['getData']);

    // Configure the 'getData' spy method to return an Observable with some test data
    cityService.getData.and.returnValue(
      of<ApiResult<City>>(<ApiResult<City>>{
        data: [
          <City>{
            name: 'TestCity1',
            id: 1,
            latitude: 1,
            longitude: 1,
            countryID: 1,
            country: 'TestCountry1'
          },
          <City>{
            name: 'TestCity2',
            id: 2,
            latitude: 1,
            longitude: 1,
            countryID: 1,
            country: 'TestCountry1'
          },
          <City>{
            name: 'TestCity3',
            id: 3,
            latitude: 1,
            longitude: 1,
            countryID: 1,
            country: 'TestCountry1'
          }
        ],
        totalCount: 3,
        pageIndex: 0,
        pageSize: 10
      })
    );

    await TestBed.configureTestingModule({
      declarations: [CitiesComponent],
      imports: [
        BrowserAnimationsModule,
        AngularMaterialModule,
        RouterTestingModule
      ],
      providers: [
        {
          provide: CityService,
          useValue: cityService
        }
      ]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CitiesComponent);
    component = fixture.componentInstance;
    component.paginator = jasmine.createSpyObj(
      "MatPaginator",
      ["length", "pageIndex", "pageSize"]
    );

    fixture.detectChanges();
  });

  // Instantiation test
  it('should create', () => {
    expect(component).toBeTruthy();
  });

  // Title test:
  it("should display a 'Cities' title", () => {
    let title = fixture.nativeElement.querySelector('h1');
    expect(title.textContent).toEqual('Cities');
  });

  // Cities table test
  it("should contain a table with a list of one or more cities", () => {
    let table = fixture.nativeElement.querySelector('table.mat-mdc-table');
    let tableRows = table.querySelectorAll('tr.mat-mdc-row');
    expect(tableRows.length).toBeGreaterThan(0);
  });
});
