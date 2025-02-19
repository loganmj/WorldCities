﻿using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Web;
using WorldCities2.Server.Security;

namespace WorldCities2.Server.Data
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

        /// <summary>
        /// Filter column name.
        /// </summary>
        public string? FilterColumn { get; private set; }

        /// <summary>
        /// Filter query string.
        /// </summary>
        public string? FilterQuery { get; private set; }

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
        private APIResult(List<T> data,
                          int count,
                          int pageIndex,
                          int pageSize,
                          string? sortColumn = null,
                          string? sortOrder = null,
                          string? filterColumn = null,
                          string? filterQuery = null)
        {
            Data = data;
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = count;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            SortColumn = sortColumn;
            SortOrder = sortOrder;
            FilterColumn = filterColumn;
            FilterQuery = filterQuery;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Checks if the given property name exists in the data.
        /// Helps protect against SQL injection.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="throwExceptionIfNotFound"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns>True, if the property name is a valid property of the given type. False, otherwise.</returns>
        public static bool IsValidProperty(string? propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException(nameof(propertyName), "Property name cannot be null or empty.");
            }

            return typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance) == null
                ? throw new NotSupportedException($"Property: '{propertyName}' does not exist.")
                : true;
        }

        /// <summary>
        /// Sanitizes all string properties of a given data object.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Returns the same data object, but with any string parameters sanitized.</returns>
        public static void SanitizeStringProperties(T? data) 
        {
            // Return if the object is null
            if (data == null) 
            {
                Console.WriteLine($"Data object is null. Nothing to sanitize.");
                return;
            }

            // Get all properties of the object
            var properties = typeof(T).GetProperties();

            // Sanitize each string property in the data object
            foreach (var property in properties) 
            {
                if (property.PropertyType == typeof(string)) 
                {
                    Console.WriteLine($"Sanitizing {property.Name} property of {typeof(T)} object ...");
                    property.SetValue(data, Sanitizer.SanitizeString(property.GetValue(data) as string));
                }
            }
        }

        /// <summary>
        /// Pages, sorts, and/or filters an IQueryable source.
        /// </summary>
        /// <param name="source">An IQueryable source of generic type T.</param>
        /// <param name="pageIndex">The current page index.</param>
        /// <param name="pageSize">The size of each page.</param>
        /// <param name="sortColumn">The sorting column name.</param>
        /// <param name="sortOrder">The sorting order ("ASC" or "DESC").</param>
        /// <param name="filterColumn">The filtering column name.</param>
        /// <param name="filterQuery">The filtering query (value to lookup).</param>
        /// <returns> An APIResult object containing the paged result and all of the relevant paging navigation information.</returns>
        public static async Task<APIResult<T>> CreateAsync(IQueryable<T> source,
                                                           int pageIndex,
                                                           int pageSize,
                                                           string? sortColumn = null,
                                                           string? sortOrder = null,
                                                           string? filterColumn = null,
                                                           string? filterQuery = null)
        {
            // Check for filter data 
            if (!string.IsNullOrEmpty(filterColumn)&& !string.IsNullOrEmpty(filterQuery) && IsValidProperty(filterColumn)) 
            {
                source = source.Where($"{filterColumn}.StartsWith(@0)", filterQuery);
            }

            // Check for sort data
            var formattedSortColumn = "";
            var formattedSortOrder = "";

            if (IsValidProperty(sortColumn))
            {
                formattedSortColumn = sortColumn;

                // Format sort order to either sort ascending or sort descending.
                // Sort ascending by default.
                formattedSortOrder = string.Equals(sortOrder, SORT_DESCENDING, StringComparison.OrdinalIgnoreCase) ? SORT_DESCENDING : SORT_ASCENDING;

                // Order the source
                source = source.OrderBy($"{sortColumn} {sortOrder}");
            }

            // Gets the total number of elements in the source
            var count = await source.CountAsync();

            // Take only the paginated data
            source = source.Skip(pageIndex * pageSize).Take(pageSize);

            // Return the paginated results, and additional data
            return new APIResult<T>(await source.ToListAsync(),
                                    count,
                                    pageIndex,
                                    pageSize,
                                    formattedSortColumn,
                                    formattedSortOrder,
                                    filterColumn,
                                    filterQuery);
        }

        #endregion
    }
}