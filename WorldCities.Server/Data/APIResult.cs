using Microsoft.EntityFrameworkCore;

namespace WorldCities.Server.Data
{
    /// <summary>
    /// A custom API result class that allows us to return additional information about our dataset.
    /// </summary>
    public class APIResult<T>
    {
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

        #endregion

        #region Constructors

        /// <summary>
        /// Private constructor called by the CreateAsync method.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="count"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        private APIResult(List<T> data, int count, int pageIndex, int pageSize)
        {
            Data = data;
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = count;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
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
        public static async Task<APIResult<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            // Gets the total number of elements in the source
            var count = await source.CountAsync();

            // Alters source to only contain the paginated data
            source = source.Skip(pageIndex * pageSize).Take(pageSize);
            var data = await source.ToListAsync();

            // Return the paginated results, and additional data
            return new APIResult<T>(data, count, pageIndex, pageSize);
        }

        #endregion
    }
}
