/*
 * An interface for API Result objects.
 */
@Injectable({
  providedIn: 'root'
})
export interface ApiResult<T> {

  // #region Properties

  data: T[];
  pageIndex: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  sortColumn: string;
  sortOrder: string;
  filterColumn: string;
  filterQuery: string;

  // #endregion
}
