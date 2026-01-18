using System;

using UnityEngine;

#nullable disable
namespace FullSerializer
{
    public static class fsConfig
    {
        public static System.Type[] SerializeAttributes = new System.Type[2]
        {
            typeof (SerializeField),
            typeof (fsPropertyAttribute)
        };
        public static System.Type[] IgnoreSerializeAttributes = new System.Type[2]
        {
            typeof (NonSerializedAttribute),
            typeof (fsIgnoreAttribute)
        };
        private static fsMemberSerialization _defaultMemberSerialization = fsMemberSerialization.Default;
        public static bool SerializeNonAutoProperties = false;
        public static bool SerializeNonPublicSetProperties = true;
        public static bool IsCaseSensitive = true;
        public static string CustomDateTimeFormatString = (string) null;
        public static bool Serialize64BitIntegerAsString = false;
        public static bool SerializeEnumsAsInteger = false;

        public static fsMemberSerialization DefaultMemberSerialization
        {
            get => fsConfig._defaultMemberSerialization;
            set
            {
                fsConfig._defaultMemberSerialization = value;
                fsMetaType.ClearCache();
            }
        }
    }
}
