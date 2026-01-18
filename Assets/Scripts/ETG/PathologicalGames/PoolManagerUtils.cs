using UnityEngine;

#nullable disable
namespace PathologicalGames
{
    public static class PoolManagerUtils
    {
        internal static void SetActive(GameObject obj, bool state) => obj.SetActive(state);

        internal static bool activeInHierarchy(GameObject obj) => obj.activeInHierarchy;
    }
}
