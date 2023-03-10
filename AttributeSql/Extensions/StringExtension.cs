namespace AttributeSql.Demo.Extensions
{
    public static class StringExtension
    {
        public static string ToStr(this object source)
        {
            return source?.ToString().Trim() ?? string.Empty;
        }
    }
}
