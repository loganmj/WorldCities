import { Injectable } from "@angular/core";
import { BaseDataService } from "../base-data.service";
import { Country } from "./country";
import { Observable } from "rxjs";
import { ApiResult } from "../apiResult";
import { HttpClient, HttpParams } from "@angular/common/http";

/**
 * A data service class for Country data.
 */
@Injectable({
  providedIn: 'root'
})
export class CountryService extends BaseDataService<Country> {

  // #region Constructors

  /**
   * Constructor
   */ 
  public constructor(http: HttpClient) { super(http); }

  // #endregion

  // #region Public Methods

  /**
  * Gets API result data from the API.
  */
  public override getData(pageIndex: number, pageSize: number, sortColumn: string, sortOrder: string, filterColumn: string | null, filterQuery: string | null): Observable<ApiResult<Country>> {

    // Get url
    var url = this.getUrl('api/Countries');

    // Create HttpParams
    var params = new HttpParams()
      .set("pageIndex", pageIndex)
      .set("pageSize", pageSize)
      .set("sortColumn", sortColumn)
      .set("sortOrder", sortOrder);

    if (filterColumn && filterQuery) {
      params = params
        .set("filterColumn", filterColumn)
        .set("filterQuery", filterQuery);
    }

    // Get data
    return this.http.get<ApiResult<Country>>(url, { params });
  }
  /**
   * Gets a city with the given ID.
   */ 
  public override get(id: number): Observable<Country> {
    return this.http.get<Country>(this.getUrl(`api/Countries/${id}`));
  }

  /**
   * Updates a country.
   */ 
  public override put(item: Country): Observable<Country> {
    return this.http.put<Country>(this.getUrl(`api/Countries/${item.id}`), item);
  }

  /**
   * Posts a new country.
   */ 
  public override post(item: Country): Observable<Country> {
    return this.http.post<Country>(this.getUrl('api/Countries'), item);
  }

  /**
   * Checks if the field value is a duplicate.
   */ 
  public isDuplicateField(countryId: number, fieldName: string, fieldValue: string): Observable<boolean> {

    // Set params
    var params = new HttpParams()
      .set("countryId", countryId)
      .set("fieldName", fieldName)
      .set("fieldValue", fieldValue);

    // Make post request to IsDuplicateField() in API controller
    return this.http.post<boolean>(this.getUrl("api/Countries/IsDuplicateField"), null, { params });
  }

  // #endregion
}
