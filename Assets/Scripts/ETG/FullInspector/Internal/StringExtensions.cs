#nullable disable
namespace FullInspector.Internal
{
    internal static class StringExtensions
    {
        public static string F(this string format, params object[] args) => string.Format(format, args);
    }
}
