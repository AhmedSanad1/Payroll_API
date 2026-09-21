namespace PayRollApi.Application.Common
{
    public static class CacheKeys
    {
        public static string EntityList<T>() => $"cache:{typeof(T).Name}:all";

        // Raw entities and mapped DTOs must never share a key — casting one to the other blows up.
        public static string RawEntityList<T>() => $"cache:{typeof(T).Name}:all:raw";
        public static string Singleton<T>() => $"cache:{typeof(T).Name}:singleton";
        public const string DepartmentsLookup = "cache:departments:lookup";

        // Bump this to drop every cached page for T in one go.
        public static string PagedListVersion<T>() => $"cache:{typeof(T).Name}:paged:version";
    }
}
