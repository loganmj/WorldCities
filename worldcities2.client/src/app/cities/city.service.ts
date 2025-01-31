import { Injectable } from "@angular/core";
import { BaseDataService } from "../base-data.service";
import { City } from "./city";
import { Observable } from "rxjs";
import { ApiResult } from "../ApiResult";

/*
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

  public override getData(pageIndex: number, pageSize: number, sortColumn: string, sortOrder: string, filterColumn: string | null, filterQuery: string | null): Observable<ApiResult<City>> {
    throw new Error("Method not implemented.");
  }
  public override get(id: number): Observable<City> {
    throw new Error("Method not implemented.");
  }
  public override put(item: City): Observable<City> {
    throw new Error("Method not implemented.");
  }
  public override post(item: City): Observable<City> {
    throw new Error("Method not implemented.");
  }

  // #endregion
}
