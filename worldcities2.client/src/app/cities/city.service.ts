import { Injectable } from "@angular/core";
import { BaseDataService } from "../base-data.service";
import { City } from "./city";
import { catchError, map, Observable, throwError } from "rxjs";
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
  public getData(pageIndex: number,
                 pageSize: number,
                 sortColumn: string,
                 sortOrder: string,
                 filterColumn: string | null,
                 filterQuery: string | null): Observable<ApiResult<City>> {

    return this.apollo
      .query({
        query: gql`
          query GetCitiesApiResult(
            $pageIndex: Int!,
            $pageSize: Int!,
            $sortColumn: String,
            $sortOrder: String,
            $filterColumn: String,
            $filterQuery: String) {
            citiesApiResult(
              pageIndex: $pageIndex
              pageSize: $pageSize
              sortColumn: $sortColumn
              sortOrder: $sortOrder
              filterColumn: $filterColumn
              filterQuery: $filterQuery
            ) {
              data {
                id
                name
                latitude
                longitude
                countryId
                country
              }
              pageIndex
              pageSize
              totalCount
              totalPages
              sortColumn
              sortOrder
              filterColumn
              filterQuery
            }
          }  
        `,
        variables: {
          pageIndex,
          pageSize,
          sortColumn,
          sortOrder,
          filterColumn,
          filterQuery
        }
      })
      .pipe(map((result: any) => result.data.citiesApiResult));
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
    return this.apollo
      .mutate({
        mutation: gql`
          mutation UpdateCity($city: CityDTOInput!) {
            updateCity(cityDTO: $city) {
            id
            name
            latitude
            longitude
            countryID
            }
          }
        `,
        variables: {
          city: item
        }
      })
      .pipe(
        map((result: any) => result.data.updateCity),
        catchError((error) => {
          return throwError(() => new Error(error));
        })
      );
  }

  /**
   * Posts a new city.
   */
  public post(item: City): Observable<City> {
    return this.apollo
      .mutate({
        mutation: gql`
          mutation AddCity($city: CityDTOInput!) {
            addCity(cityDTO: $city) {
              id
              name
              latitude
              longitude
              countryID
            }
          }
        `,
        variables: {
          city: item
        }
      })
      .pipe(
        map((result: any) => result.data.addCity),
        catchError((error) => {
          return throwError(() => new Error(error));
        }));
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
