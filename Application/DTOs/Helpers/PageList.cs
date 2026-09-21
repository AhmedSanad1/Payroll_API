using System;
using System.Collections.Generic;

namespace PayRollApi.Application.Helpers
{
    // Kept off PageList<T> so callers can reach these without a T.
    public static class PageList
    {
        public const int DefaultPageSize = 50;
        public const int MaxPageSize = 2000;

        public static int ClampPageSize(int requestedPageSize) =>
            requestedPageSize <= 0 ? DefaultPageSize : Math.Min(requestedPageSize, MaxPageSize);
    }

    // Don't make this inherit List<T> — System.Text.Json would send a bare array and drop TotalCount & co.
    public class PageList<T>
    {
        public IReadOnlyList<T> Items { get; }

        // 1-based: page 1 is the first page.
        public int PageNumber { get; }
        public int PageSize { get; }
        public int TotalCount { get; }

        public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;

        // Up to 5 page numbers around the current one, for the pager buttons.
        public int StartPage { get; }
        public int EndPage { get; }

        public PageList(IReadOnlyList<T> items, int totalCount, int pageNumber, int pageSize)
        {
            Items = items;
            TotalCount = totalCount;
            PageNumber = pageNumber < 1 ? 1 : pageNumber;
            PageSize = pageSize;

            (StartPage, EndPage) = ComputePageWindow(PageNumber, TotalPages);
        }

        private static (int Start, int End) ComputePageWindow(int pageNumber, int totalPages)
        {
            if (totalPages == 0)
                return (0, 0);

            if (totalPages <= 5)
                return (1, totalPages);

            var start = Math.Max(pageNumber - 2, 1);
            var end = Math.Min(start + 4, totalPages);
            start = Math.Max(end - 4, 1);
            return (start, end);
        }
    }
}
