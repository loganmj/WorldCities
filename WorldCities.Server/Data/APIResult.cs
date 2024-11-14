using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Reflection;

namespace WorldCities.Server.Data
{
    /// <summary>
    /// A custom API result class that allows us to return additional information about our dataset.
    /// </summary>
    public class APIResult<T>
    {
        #region Constants

        private const string SORT_ASCENDING = "ASC";
        private const string SORT_DESCENDING = "DESC";

        #endregion

        #region Properties

        /// <summary>
        /// The data result.
        /// </summary>
        public List<T> Data { get; private set; }

        /// <summary>
        /// The index of the current page.
        /// </summary>
        public int PageIndex { get; private set; }

        /// <summary>
        /// The size of a given page.
        /// </summary>
        public int PageSize { get; private set; }

        /// <summary>
        /// The total number of items in the database table.
        /// </summary>
        public int TotalCount { get; private set; }

        /// <summary>
        /// The total number of pages (using the given size).
        /// </summary>
        public int TotalPages { get; private set; }

        /// <summary>
        /// True if the current page has a previous page, false otherwise.
        /// </summary>
        public bool HasPreviousPage => (PageIndex > 0);

        /// <summary>
        /// True if the current page has a next page, false otherwise.
        /// </summary>
        public bool HasNextPage => (PageIndex + 1) < TotalPages;

        /// <summary>
        /// The sorting column name (or null, if none is set).
        /// </summary>
        public string? SortColumn { get; private set; }

        /// <summary>
        /// The sorting order ("ASC", "DESC" or null, if none is set.
        /// </summary>
        public string? SortOrder { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Private constructor called by the CreateAsync method.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="count"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        private APIResult(List<T> data,
                          int count,
                          int pageIndex,
                          int pageSize,
                          string? sortColumn,
                          string? sortOrder)
        {
            Data = data;
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = count;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            SortColumn = sortColumn;
            SortOrder = sortOrder;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Validates that the object contains a property with the given name.
        /// Helps protect against SQL injection attacks.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private static bool IsValidProperty(string? propertyName, bool throwExceptionIfNotFound = true)
        {
            // Check if property name parameter is null or empty
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new NotSupportedException($"Error: Property name is null or empty.");
            }

            // Retrieve the property with the given name and check that it is not null
            return typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance) == null
                ? throw new NotSupportedException($"Error Property '{propertyName}' does not exist.")
                : true;
        }

        /// <summary>
        /// Sorts the source data based on the specified column.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sortColumn"></param>
        /// <param name="sortOrder"></param>
        /// <returns></returns>
        private static IQueryable<T> SortSource(IQueryable<T> source, string? sortColumn, string? sortOrder)
        {
            // Check that the sort column parameter has valid data
            if (!IsValidProperty(sortColumn))
            {
                return source;
            }

            // Format the sort order property
            sortOrder = !string.IsNullOrEmpty(sortOrder) && sortOrder.Equals(SORT_ASCENDING, StringComparison.OrdinalIgnoreCase) ? SORT_ASCENDING : SORT_DESCENDING;

            // Return the ordered data
            return source.OrderBy($"{sortColumn} {sortOrder}");
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Pages and/or sorts an IQueryable source.
        /// </summary>
        /// <param name="source">An IQueryable source of generic type T.</param>
        /// <param name="pageIndex">The current page index.</param>
        /// <param name="pageSize">The size of each page.</param>
        /// <param name="sortColumn">The data column to sort by.</param>
        /// <param name="sortOrder">Sort by ascending or descending.</param>
        /// <returns> An APIResult object containing the paged result and all of the relevant paging navigation information.</returns>
        public static async Task<APIResult<T>> CreateAsync(IQueryable<T> source,
                                                           int pageIndex,
                                                           int pageSize,
                                                           string? sortColumn = null,
                                                           string? sortOrder = null)
        {
            // Gets the total number of elements in the source
            var count = await source.CountAsync();

            // Sort the data
            source = SortSource(source, sortColumn, sortOrder);

            // Alters source to only contain the paginated data
            source = source.Skip(pageIndex * pageSize).Take(pageSize);
            var data = await source.ToListAsync();

            // Return the paginated results, and additional data
            return new APIResult<T>(data, count, pageIndex, pageSize, sortColumn, sortOrder);
        }

        #endregion
    }
}
