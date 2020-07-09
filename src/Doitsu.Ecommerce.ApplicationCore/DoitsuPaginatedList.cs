using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Doitsu.Ecommerce.ApplicationCore
{
    public class DoitsuPaginatedList<T>
    {
        /// <summary>
        /// Current page
        /// </summary>
        /// <value></value>
        [JsonProperty("pageIndex")]
        public int PageIndex { get; private set; }

        /// <summary>
        /// Limit of items in 1 page
        /// </summary>
        /// <value></value>
        [JsonProperty("pageSize")]
        public int PageSize { get; private set; }

        /// <summary>
        /// Total pages user can reach
        /// </summary>
        /// <value></value>
        [JsonProperty("totalPages")]
        public int TotalPages { get; private set; }

        /// <summary>
        /// Total items user can reach
        /// </summary>
        /// <value></value>
        [JsonProperty("totalSize")]
        public int TotalSize { get; private set; }

        /// <summary>
        /// Item data in one page.
        /// </summary>
        /// <value></value>
        [JsonProperty("result")]
        public ImmutableList<T> Result { get; set; }
        public DoitsuPaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            TotalSize = count;
            Result = ImmutableList<T>.Empty;
            Result = Result.AddRange(items);
        }

        public static DoitsuPaginatedList<T> Empty => new DoitsuPaginatedList<T>(new List<T>(), 0, 0, 0);

        [JsonProperty("hasPreviousPage")]
        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }


        [JsonProperty("hasNextPage")]
        public bool HasNextPage
        {
            get
            {
                return (PageIndex < TotalPages);
            }
        }

        public static async Task<DoitsuPaginatedList<T>> CreateAsync(IOrderedQueryable<T> source, int pageIndex, int pageSize)
        {
            if (pageIndex > 0 && pageSize > 0)
            {
                var count = await source.CountAsync();
                var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                return new DoitsuPaginatedList<T>(items, count, pageIndex, pageSize);
            }
            else
            {
                return DoitsuPaginatedList<T>.Empty;
            }
        }
    }
}
