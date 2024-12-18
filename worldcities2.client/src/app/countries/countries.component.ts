import { Country } from './country';
import { environment } from '../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort';
import { Component, OnInit, ViewChild } from '@angular/core';

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
  private readonly DEFAULT_SORT_COLUMN: string = "Name";
  private readonly DEFAULT_SORT_ORDER: "asc" | "desc" = "asc";

  // #endregion

  // #region Properties

  /**
   * The desired page index.
   */
  public PageIndex: number = this.DEFAULT_PAGE_INDEX;

  /**
   * The desired page size.
   */
  public PageSize: number = this.DEFAULT_PAGE_SIZE;

  /**
   * The name of the data column to sort by.
   */
  public SortColumn: string = this.DEFAULT_SORT_COLUMN;

  /**
   * The direction to sort by (ascending or descending). 
   * Can have a value of "ASC" or "DESC"
   */
  public SortOrder: "asc" | "desc" = this.DEFAULT_SORT_ORDER;

  /**
   * The columns to display in the data table.
   */
  public displayedColumns: string[] = ['id', 'name', 'iso2', 'iso3'];

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
   * Lifecycle hook that gets called when the component is initialized.
   */
  public ngOnInit(): void {
    this.countries.sort = this.sort;
    this.countries.paginator = this.paginator;
    this.loadData();
  }

  /**
   * Populates the data.
   */
  public loadData(): void {

    var pageEvent = new PageEvent();
    pageEvent.pageIndex = this.PageIndex;
    pageEvent.pageSize = this.PageSize;
    this.getData(pageEvent);
  }

  /**
   * Retrieves paginated Country data from the API.
   */
  public getData(event: PageEvent) {
    var url = environment.baseUrl + 'api/Countries';

    // Check sort values
    const sortColumn = this.sort && this.sort.active ? this.sort.active : this.SortColumn;
    const sortOrder = this.sort && this.sort.direction ? this.sort.direction : this.SortOrder;

    // Get page index and page size from the url parameters.
    var params = new HttpParams()
      .set("pageIndex", event.pageIndex.toString())
      .set("pageSize", event.pageSize.toString())
      .set("sortColumn", sortColumn)
      .set("sortOrder", sortOrder);

    // Assume we are receiving our custom paginated data type from the API.
    // Retrieve the country data and the additional metadata from the return object.
    this.http.get<any>(url, { params })
      .subscribe({
        next: (result) => {
          this.paginator.length = result.totalCount;
          this.paginator.pageIndex = result.pageIndex;
          this.paginator.pageSize = result.pageSize;
          this.countries.data = result.data;

          // DEBUG
          console.log('API result: ', result);
          console.log('Countries data: ', this.countries.data);
        },
        error: (error) => console.error(error)
      });
  }

  /**
   * Event handler that updates the sort component and re-queries the data
   * when the sort is changed.
   */
  onSortChange(sortState: Sort) {
    this.sort.active = sortState.active;
    this.sort.direction = sortState.direction;
    this.loadData();
  }

  // #endregion
}
