using System;

#nullable disable

    public static class dfStringExtensions
    {
        public static string MakeRelativePath(this string path)
        {
            return string.IsNullOrEmpty(path) ? string.Empty : path.Substring(path.IndexOf("Assets/", StringComparison.OrdinalIgnoreCase));
        }

        public static bool Contains(this string value, string pattern, bool caseInsensitive)
        {
            return caseInsensitive ? value.IndexOf(pattern, StringComparison.OrdinalIgnoreCase) != -1 : value.IndexOf(pattern) != -1;
        }
    }

