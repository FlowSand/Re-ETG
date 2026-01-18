using System.Collections.Generic;

using UnityEngine;

#nullable disable

    public static class IntToStringSansGarbage
    {
        private static Dictionary<int, string> m_map = new Dictionary<int, string>();

        public static string GetStringForInt(int input)
        {
            if (IntToStringSansGarbage.m_map.ContainsKey(input))
                return IntToStringSansGarbage.m_map[input];
            string stringForInt = input.ToString();
            IntToStringSansGarbage.m_map.Add(input, stringForInt);
            if (IntToStringSansGarbage.m_map.Count > 25000)
            {
                Debug.LogError((object) "Int To String (sans Garbage) map count greater than 25000!");
                IntToStringSansGarbage.m_map.Clear();
            }
            return stringForInt;
        }
    }

