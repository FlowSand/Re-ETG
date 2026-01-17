// Decompiled with JetBrains decompiler
// Type: ObjectHeightController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class ObjectHeightController : MonoBehaviour
    {
      public float heightOffGround = 0.5f;
      private Transform m_transform;

      private void Start() => this.m_transform = this.transform;

      private void Update()
      {
        this.m_transform.position = this.m_transform.position.WithZ(this.m_transform.position.y - this.heightOffGround);
      }
    }

}
