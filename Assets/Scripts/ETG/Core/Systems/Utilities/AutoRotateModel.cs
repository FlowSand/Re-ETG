// Decompiled with JetBrains decompiler
// Type: AutoRotateModel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [AddComponentMenu("Daikon Forge/Examples/Color Picker/Auto-rotate Model")]
    public class AutoRotateModel : MonoBehaviour
    {
      private void Update() => this.transform.Rotate(Vector3.up * BraveTime.DeltaTime * 45f);
    }

}
