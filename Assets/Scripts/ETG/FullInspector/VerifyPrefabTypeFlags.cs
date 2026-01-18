using System;

#nullable disable
namespace FullInspector
{
    [Flags]
    public enum VerifyPrefabTypeFlags
    {
        None = 1,
        Prefab = 2,
        ModelPrefab = 4,
        PrefabInstance = 8,
        ModelPrefabInstance = 16, // 0x00000010
        MissingPrefabInstance = 32, // 0x00000020
        DisconnectedPrefabInstance = 64, // 0x00000040
        DisconnectedModelPrefabInstance = 128, // 0x00000080
    }
}
