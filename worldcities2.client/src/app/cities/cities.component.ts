import { City } from './city';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { Component, OnInit, ViewChild } from '@angular/core';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { CityService } from './city.service';

/**
 * A component for City data objects.
 */
@Component({
  selector: 'app-cities',
  templateUrl: './cities.component.html',
  styleUrl: './cities.component.scss',
  standalone: false
})
export class CitiesComponent implements OnInit {

  // #region Constants

  private readonly DEFAULT_PAGE_INDEX: number = 0;
  private readonly DEFAULT_PAGE_SIZE: number = 10;
  private readonly DEFAULT_SORT_COLUMN: string = "name";
  private readonly DEFAULT_SORT_ORDER: "asc" | "desc" = "asc";
  private readonly DEFAULT_FILTER_COLUMN: string = "name";

  // #endregion

  // #region Properties

  /**
   * The name of the column to filter by.
   */ 
  public filterColumn?: string;

  /**
   * The query string for a data filter.
   */ 
  public filterQuery?: string;

  /**
   * The columns to display in the data table.
   */
  public displayedColumns: string[] = ['id', 'name', 'latitude', 'longitude', 'country'];

  /**
   * The '!' character (definite assignment assertion operator)
   * tells TypeScript that this property will be assigned a value before it is used,
   * even though that cannot be confirmed at compile time.
   */
  public cities = new MatTableDataSource<City>();

  /**
   * A reference to the paginator in the DOM.
   */
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  /**
   * A reference to the sort component in DOM.
   */
  @ViewChild(MatSort) sort!: MatSort;

  /**
   * Subjects are an RxJS tool for handling async data streams.
   * This property is a Subject object that emits string values.
   * Used to handle changes to the filter text.
   */
  public filterTextChanged: Subject<string> = new Subject<string>();

  // #endregion

  // #region Constructors

  /**
   * Constructs a component with a given http client object.
   * The shorthand in the parameter creates a private HttpClient field
   * that is accessable by the rest of the class.
   */
  constructor(private cityService: CityService) { }

  // #endregion

  // #region Public Methods

  /**
   * Lifecycle hook that gets called when the component is initialized.
   */
  public ngOnInit(): void {
    this.cities.sort = this.sort;
    this.cities.paginator = this.paginator;
    this.loadData();
  }

  /**
   * Populates the data, setting the page parameters to default.
   * Allows the user to pass in a filter query.
   * @param filterQuery 
   */
  public loadData(filterQuery?: string): void {

    var pageEvent = new PageEvent();
    pageEvent.pageIndex = this.DEFAULT_PAGE_INDEX;
    pageEvent.pageSize = this.DEFAULT_PAGE_SIZE;
    this.filterQuery = filterQuery;
    this.getData(pageEvent);
  }

  /**
   * Retrieves City data from the API.
   * Takes a page event as a parameter, allowing for pagination of the data.
   * @param event
   */
  public getData(event: PageEvent) {

    // Set parameters
    var sortColumn = this.sort && this.sort.active ? this.sort.active : this.DEFAULT_SORT_COLUMN;
    var sortOrder = this.sort && this.sort.direction ? this.sort.direction : this.DEFAULT_SORT_ORDER;
    var filterColumn = this.filterQuery ? this.DEFAULT_FILTER_COLUMN : null;
    var filterQuery = this.filterQuery ? this.filterQuery : null;

    // Get data using the city data service
    this.cityService.getData(event.pageIndex, event.pageSize, sortColumn, sortOrder, filterColumn, filterQuery)
      .subscribe({
        next: (result) => {
          this.paginator.length = result.totalCount;
          this.paginator.pageIndex = result.pageIndex;
          this.paginator.pageSize = result.pageSize;
          this.cities = new MatTableDataSource<City>(result.data);
        },
        error: (error) => console.error(error)
      });
  }

  /**
   * Called when filter text is changed.
   * @param filterText 
   */
  public onFilterTextChanged(filterText: string) {

    // Lazy subscription to the filter text subject.
    // Done here, because if the filter is never used, we don't have to
    // allocate resources subscribing to it.
    if (!this.filterTextChanged.observed) {
      this.filterTextChanged
        .pipe(debounceTime(1000), distinctUntilChanged())
        .subscribe(query => { this.loadData(query); });
    }

    // Emit the filter text value
    this.filterTextChanged.next(filterText);
  }

  // #endregion
}
