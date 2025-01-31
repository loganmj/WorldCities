import { Country } from './country';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, } from '@angular/material/sort';
import { Component, OnInit, ViewChild } from '@angular/core';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { CountryService } from './country.service';

/**
 * A component for Country data objects.
 */
@Component({
  selector: 'app-countries',
  templateUrl: './countries.component.html',
  styleUrl: './countries.component.scss',
  standalone: false
})
export class CountriesComponent implements OnInit {

  // #region Constants

  private readonly DEFAULT_PAGE_INDEX: number = 0;
  private readonly DEFAULT_PAGE_SIZE: number = 10;
  private readonly DEFAULT_SORT_COLUMN: string = "name";
  private readonly DEFAULT_SORT_ORDER: "asc" | "desc" = "asc";
  private readonly DEFAULT_FILTER_COLUMN: string = "name";

  // #endregion

  // #region Properties

  /**
 * The query string for a data filter.
 */
  public filterQuery?: string;

  /**
   * The columns to display in the data table.
   */
  public displayedColumns: string[] = ['id', 'name', 'iso2', 'iso3', 'numberOfCities'];

  /**
   * The '!' character (definite assignment assertion operator)
   * tells TypeScript that this property will be assigned a value before it is used,
   * even though that cannot be confirmed at compile time.
   */
  public countries = new MatTableDataSource<Country>();

  /**
   * A reference to the paginator in the DOM.
   */
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  /**
   * A reference to the sort component in DOM.
   */
  @ViewChild(MatSort) sort!: MatSort;

  /**
   * An observable, async subject that emits a string value.
   * Used to emit changes to the filter text.
   */ 
  public filterTextChanged: Subject<string> = new Subject<string>();

  // #endregion

  // #region Constructors

  /**
   * Constructs a component with a given http client object.
   * The shorthand in the parameter creates a private HttpClient field
   * that is accessable by the rest of the class.
   */
  constructor(private countryService: CountryService) { }

  // #endregion

  // #region Public Methods

  /**
   * Lifecycle hook that gets called when the component is initialized.
   */
  public ngOnInit(): void {
    this.countries.sort = this.sort;
    this.countries.paginator = this.paginator;
    this.loadData();
  }

  /**
   * Populates the data.
   * Sets the pagination to default config values.
   */
  public loadData(query?: string): void {

    var pageEvent = new PageEvent();
    pageEvent.pageIndex = this.DEFAULT_PAGE_INDEX;
    pageEvent.pageSize = this.DEFAULT_PAGE_SIZE;
    this.filterQuery = query;
    this.getData(pageEvent);
  }

  /**
   * Retrieves paginated Country data from the API.
   */
  public getData(event: PageEvent) {

    // Set parameters
    var sortColumn = this.sort && this.sort.active ? this.sort.active : this.DEFAULT_SORT_COLUMN;
    var sortOrder = this.sort && this.sort.direction ? this.sort.direction : this.DEFAULT_SORT_ORDER;
    var filterColumn = this.filterQuery ? this.DEFAULT_FILTER_COLUMN : null;
    var filterQuery = this.filterQuery ? this.filterQuery : null;

    // Get data using the city data service
    this.countryService.getData(event.pageIndex, event.pageSize, sortColumn, sortOrder, filterColumn, filterQuery)
      .subscribe({
        next: (result) => {
          this.paginator.length = result.totalCount;
          this.paginator.pageIndex = result.pageIndex;
          this.paginator.pageSize = result.pageSize;
          this.countries = new MatTableDataSource<Country>(result.data);
        },
        error: (error) => console.error(error)
      });
  }

  /**
   * Debounces the filter text change and emits the next value.
   */ 
  public onFilterTextChanged(filterText: string) {

    // Lazily subscribe to the filter text changed subject
    // Add pipe to debounce value
    if (!this.filterTextChanged.observed) {
      this.filterTextChanged
        .pipe(debounceTime(1000), distinctUntilChanged())
        .subscribe(query => { this.loadData(query); });
    }

    // Emit the next value
    this.filterTextChanged.next(filterText);
  }

  // #endregion
}
