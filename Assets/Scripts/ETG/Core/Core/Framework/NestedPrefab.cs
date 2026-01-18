// Decompiled with JetBrains decompiler
// Type: NestedPrefab
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

public class NestedPrefab : BraveBehaviour
  {
    public Vector3 localPosition = Vector3.zero;
    public Vector3 localRotation = Vector3.zero;
    public Vector3 localScale = Vector3.one;
    public GameObject prefab;

    public void Awake()
    {
      GameObject gameObject = Object.Instantiate<GameObject>(this.prefab, this.transform.position, Quaternion.identity);
      gameObject.transform.parent = this.transform;
      if (this.localScale != Vector3.zero)
        gameObject.transform.localScale = this.localScale;
      if (this.localRotation != Vector3.zero)
        gameObject.transform.localRotation = Quaternion.Euler(this.localRotation);
      if (!(this.localScale != Vector3.one))
        return;
      gameObject.transform.localScale = this.localScale;
    }

    protected override void OnDestroy() => base.OnDestroy();
  }

