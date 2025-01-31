import { Injectable } from "@angular/core";
import { BaseDataService } from "../base-data.service";
import { City } from "./city";
import { Observable } from "rxjs";
import { ApiResult } from "../ApiResult";
import { HttpParams } from "@angular/common/http";

/**
 * Data service class for City data.
 */
@Injectable({
  providedIn: 'root',
})
export class CityService extends BaseDataService<City> {

  // #region Properties
  // #endregion

  // #region Construtors
  // #endregion

  // #region Public Methods

  /**
    * Gets API result data from the API.
    */
  public getData(pageIndex: number, pageSize: number, sortColumn: string, sortOrder: string, filterColumn: string | null, filterQuery: string | null): Observable<ApiResult<City>> {

    // Get URL
    var url = this.getUrl("api/Cities");

    // Set API result parameters
    var params = new HttpParams()
      .set("pageIndex", pageIndex.toString())
      .set("pageSize", pageSize.toString())
      .set("sortColumn", sortColumn)
      .set("sortOrder", sortOrder);

    if (filterColumn && filterQuery) {
      params = params
        .set("filterColumn", filterColumn)
        .set("filterQuery", filterQuery);
    }

    // Get city data from API
    return this.http.get<ApiResult<City>>(url, { params });
  }

  /**
   * Gets a city.
   */
  public get(id: number): Observable<City> {
    return this.http.get<City>(this.getUrl(`api/Cities/${id}`));
  }

  /**
   * Updates a city.
   */
  public put(item: City): Observable<City> {
    return this.http.put<City>(this.getUrl(`api/Cities/${item.id}`), item);
  }

  /**
   * Posts a new city.
   */
  public post(item: City): Observable<City> {
    return this.http.post<City>(this.getUrl(`api/Cities`), item);
  }

  // #endregion
}
