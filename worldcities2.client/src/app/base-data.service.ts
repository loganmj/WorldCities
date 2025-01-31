import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiResult } from './ApiResult';
import { environment } from '../environments/environment';

/**
 * A base data service class.
 */ 
@Injectable({
  providedIn: 'root'
})
export abstract class BaseDataService<T> {

  // #region Constructors

  /*
   * Constructor
   */
  constructor(protected http: HttpClient) { }

  // #endregion

  // #region Protected Methods

  /*
   * Gets the current url path.
   */
  protected getUrl(url: string) {
    return `${environment.baseUrl}${url}`;
  }

  // #endregion

  // #region Public Methods

  /*
   * Gets object data from the API result.
   */
  public abstract getData(pageIndex: number, pageSize: number, sortColumn: string, sortOrder: string, filterColumn: string | null, filterQuery: string | null): Observable<ApiResult<T>>;

  /*
   * Sends a get request to the API.
   */
  public abstract get(id: number): Observable<T>;

  /*
   * Sends a put request to the API.
   */
  public abstract put(item: T): Observable<T>;

  /*
   * Sends a post request to the API.
   */
  public abstract post(item: T): Observable<T>;

  // #endregion
}
