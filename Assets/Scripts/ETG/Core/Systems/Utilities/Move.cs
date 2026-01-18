// Decompiled with JetBrains decompiler
// Type: Move
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

public class Move : MonoBehaviour
  {
    private void Update()
    {
      Vector3 zero = Vector3.zero;
      this.transform.Translate(Vector3.right * Input.GetAxis("Horizontal") * BraveTime.DeltaTime * 5f, Space.World);
      this.transform.Translate(Vector3.forward * Input.GetAxis("Vertical") * BraveTime.DeltaTime * 5f, Space.World);
    }
  }

