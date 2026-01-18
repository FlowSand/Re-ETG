using System.Collections.Generic;
using System.Text;

#nullable disable
namespace FullInspector.Internal
{
    public static class fiDisplayNameMapper
    {
        private static readonly Dictionary<string, string> _mappedNames = new Dictionary<string, string>();

        public static string Map(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                return string.Empty;
            string str;
            if (!fiDisplayNameMapper._mappedNames.TryGetValue(propertyName, out str))
            {
                str = fiDisplayNameMapper.MapInternal(propertyName);
                fiDisplayNameMapper._mappedNames[propertyName] = str;
            }
            return str;
        }

        private static string MapInternal(string propertyName)
        {
            if (propertyName.StartsWith("m_") && propertyName != "m_")
                propertyName = propertyName.Substring(2);
            int index1 = 0;
            while (index1 < propertyName.Length && propertyName[index1] == '_')
                ++index1;
            if (index1 >= propertyName.Length)
                return propertyName;
            StringBuilder stringBuilder = new StringBuilder();
            bool flag = true;
            for (int index2 = index1; index2 < propertyName.Length; ++index2)
            {
                char upper = propertyName[index2];
                if (upper == '_')
                {
                    flag = true;
                }
                else
                {
                    if (flag)
                    {
                        flag = false;
                        upper = char.ToUpper(upper);
                    }
                    if (index2 != index1 && fiDisplayNameMapper.ShouldInsertSpace(index2, propertyName))
                        stringBuilder.Append(' ');
                    stringBuilder.Append(upper);
                }
            }
            return stringBuilder.ToString();
        }

        private static bool ShouldInsertSpace(int currentIndex, string str)
        {
            return char.IsUpper(str[currentIndex]) && currentIndex + 1 < str.Length && !char.IsUpper(str[currentIndex + 1]);
        }
    }
}
