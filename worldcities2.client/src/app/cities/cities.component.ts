import { City } from './city';
import { environment } from '../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { Component, OnInit, ViewChild } from '@angular/core';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';

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
  private readonly DEFAULT_SORT_COLUMN: string = "Name";
  private readonly DEFAULT_SORT_ORDER: "asc" | "desc" = "asc";
  private readonly DEFAULT_FILTER_COLUMN: string = "name";

  // #endregion

  // #region Properties

  /**
   * The desired page index.
   */
  public pageIndex: number = this.DEFAULT_PAGE_INDEX;

  /**
   * The desired page size.
   */
  public pageSize: number = this.DEFAULT_PAGE_SIZE;

  /**
   * The name of the data column to sort by.
   */
  public sortColumn: string = this.DEFAULT_SORT_COLUMN;

  /**
   * The direction to sort by (ascending or descending). 
   * Can have a value of "ASC" or "DESC"
   */
  public sortOrder: "asc" | "desc" = this.DEFAULT_SORT_ORDER;

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
  constructor(private http: HttpClient) { }

  // #endregion

  // #region Private Methods
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
   * Populates the data.
   */
  public loadData(query?: string): void {

    var pageEvent = new PageEvent();
    pageEvent.pageIndex = this.pageIndex;
    pageEvent.pageSize = this.pageSize;
    this.filterQuery = query;
    this.getData(pageEvent);
  }

  /**
   * Retrieves paginated City data from the API.
   */
  public getData(event: PageEvent) {
    var url = environment.baseUrl + 'api/Cities';

    // Check sort values
    const sortColumn = this.sort && this.sort.active ? this.sort.active : this.sortColumn;
    const sortOrder = this.sort && this.sort.direction ? this.sort.direction : this.sortOrder;

    // Get page index and page size from the url parameters.
    var params = new HttpParams()
      .set("pageIndex", event.pageIndex.toString())
      .set("pageSize", event.pageSize.toString())
      .set("sortColumn", sortColumn)
      .set("sortOrder", sortOrder);

    // Apply filter
    if (this.filterQuery) {
      params = params
        .set("filterColumn", this.DEFAULT_FILTER_COLUMN)
        .set("filterQuery", this.filterQuery);
    }

    // Assume we are receiving our custom paginated data type from the API.
    // Retrieve the city data and the additional metadata from the return object.
    this.http.get<any>(url, { params })
      .subscribe({
        next: (result) => {
          this.paginator.length = result.totalCount;
          this.paginator.pageIndex = result.pageIndex;
          this.paginator.pageSize = result.pageSize;
          this.cities.data = result.data
        },
        error: (error) => console.error(error)
      });
  }

  /**
   * Called when filter text is changed
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
