import { Injectable } from "@angular/core";
import { BaseDataService } from "../base-data.service";
import { City } from "./city";
import { map, Observable } from "rxjs";
import { ApiResult } from "../apiResult";
import { HttpClient, HttpParams } from "@angular/common/http";
import { Country } from "../countries/country";
import { Apollo } from "apollo-angular";
import { gql } from "@apollo/client/core";

/**
 * Data service class for City data.
 */
@Injectable({
  providedIn: 'root',
})
export class CityService extends BaseDataService<City> {

  // #region Constructors

  /**
   * Constructor
   */
  public constructor(http: HttpClient, private apollo: Apollo) { super(http); }

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
    return this.apollo
      .query({
        query: gql`
          query GetCityById($id: Int!) {
            cities(where: { id: { eq: $id } }) {
              nodes {
                id
                name
                latitude
                longitude
                countryId
              }
            }
          }
        `,
        variables: { id }
      })
      .pipe(map((result: any) => result.data.cities.nodes[0]));
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

  /**
   * Gets all countries from the API.
   */ 
  public getCountries(pageIndex: number, pageSize: number, sortColumn: string, sortOrder: string, filterColumn: string | null, filterQuery: string | null): Observable<ApiResult<Country>> {

    // Set URL
    var url = this.getUrl("api/Countries");

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

    // Get country data from API
    return this.http.get<ApiResult<Country>>(url, { params });
  }

  /**
   * Checks to see if the give city object already exists.
   */
  public isDuplicateCity(item: City): Observable<boolean> {
    return this.http.post<boolean>(this.getUrl('api/Cities/IsDuplicateCity'), item);
  }

  // #endregion
}
