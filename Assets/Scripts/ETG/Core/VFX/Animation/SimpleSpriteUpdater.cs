// Decompiled with JetBrains decompiler
// Type: SimpleSpriteUpdater
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.VFX.Animation
{
    public class SimpleSpriteUpdater : MonoBehaviour
    {
      private void Start() => this.GetComponent<tk2dBaseSprite>().UpdateZDepth();
    }

}
