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
        /// The name of the data column to sort by. 
        /// </summary>
        public string? SortColumn { get; private set; }

        /// <summary>
        /// A string representation of the sorting order ("ASC" or "DESC").
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
        /// <param name="sortColumn"></param>
        /// <param name="sortOrder"></param>
        private APIResult(List<T> data, int count, int pageIndex, int pageSize, string? sortColumn = null, string? sortOrder = null)
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
        /// Checks if the given property name exists in the data.
        /// Helps protect against SQL injection.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        private static bool IsValidProperty(string? propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException(nameof(propertyName), "Property name cannot be null or empty.");
            }

            return typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance) == null
                ? throw new NotSupportedException($"Property: '{propertyName}' does not exist.")
                : true;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Pages an IQueryable source.
        /// </summary>
        /// <param name="source">An IQueryable source of generic type T.</param>
        /// <param name="pageIndex">The current page index.</param>
        /// <param name="pageSize">The size of each page.</param>
        /// <returns> An APIResult object containing the paged result and all of the relevant paging navigation information.</returns>
        public static async Task<APIResult<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize, string? sortColumn = null, string? sortOrder = null)
        {
            // Gets the total number of elements in the source
            var count = await source.CountAsync();
            var formattedSortColumn = "";
            var formattedSortOrder = "";

            // Check for sort data
            if (IsValidProperty(sortColumn))
            {
                formattedSortColumn = sortColumn;

                // Format sort order to either sort ascending or sort descending.
                // Sort ascending by default.
                formattedSortOrder = string.Equals(sortOrder, SORT_DESCENDING, StringComparison.OrdinalIgnoreCase) ? SORT_DESCENDING : SORT_ASCENDING;

                // Order the source
                source = source.OrderBy($"{sortColumn} {sortOrder}");
            }

            // Take only the paginated data
            source = source.Skip(pageIndex * pageSize).Take(pageSize);

            // Return the paginated results, and additional data
            return new APIResult<T>(await source.ToListAsync(), count, pageIndex, pageSize, formattedSortColumn, formattedSortOrder);
        }

        #endregion
    }
}