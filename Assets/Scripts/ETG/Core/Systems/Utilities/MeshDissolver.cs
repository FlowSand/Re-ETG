// Decompiled with JetBrains decompiler
// Type: MeshDissolver
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class MeshDissolver : MonoBehaviour
    {
      public void DissolveMesh(Vector2 startPosition, float duration)
      {
        this.StartCoroutine(this.Dissolve(startPosition, duration));
      }

      [DebuggerHidden]
      private IEnumerator Dissolve(Vector2 startPosition, float duration)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new MeshDissolver.\u003CDissolve\u003Ec__Iterator0()
        {
          startPosition = startPosition,
          duration = duration,
          \u0024this = this
        };
      }
    }

}
