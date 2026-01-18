using UnityEngine;

#nullable disable

public interface IDFControlHost
    {
        T AddControl<T>() where T : dfControl;

        dfControl AddControl(System.Type controlType);

        void AddControl(dfControl child);

        dfControl AddPrefab(GameObject prefab);
    }

