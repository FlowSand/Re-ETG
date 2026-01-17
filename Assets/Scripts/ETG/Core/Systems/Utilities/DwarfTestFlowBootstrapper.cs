// Decompiled with JetBrains decompiler
// Type: DwarfTestFlowBootstrapper
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class DwarfTestFlowBootstrapper : MonoBehaviour
    {
      public static bool IsBootstrapping;
      public static bool ShouldConvertToCoopMode;
      public bool ConvertToCoopMode;

      private void Start()
      {
        foreach (UnityEngine.Object @object in UnityEngine.Object.FindObjectsOfType<GameManager>())
          UnityEngine.Object.Destroy(@object);
        if (this.ConvertToCoopMode)
          DwarfTestFlowBootstrapper.ShouldConvertToCoopMode = true;
        UnityEngine.Random.InitState(new System.Random().Next(1, 1000));
      }
    }

}
