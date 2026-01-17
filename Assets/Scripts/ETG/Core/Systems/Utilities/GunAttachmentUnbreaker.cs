// Decompiled with JetBrains decompiler
// Type: GunAttachmentUnbreaker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class GunAttachmentUnbreaker : MonoBehaviour
    {
      private void Start()
      {
      }

      private void Update()
      {
        if ((double) this.gameObject.transform.position.y >= 0.0)
          return;
        this.gameObject.transform.position = new Vector3(this.transform.position.x, Mathf.Abs(this.gameObject.transform.position.y), this.transform.position.z);
      }
    }

}
