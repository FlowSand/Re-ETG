// Decompiled with JetBrains decompiler
// Type: SimpleMover
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

public class SimpleMover : MonoBehaviour
  {
    public Vector3 velocity;
    public Vector3 acceleration;
    private Transform m_transform;

    private void Start() => this.m_transform = this.transform;

    private void Update()
    {
      this.m_transform.position += this.velocity * BraveTime.DeltaTime;
      this.velocity += this.acceleration * BraveTime.DeltaTime;
    }

    public void OnDespawned() => Object.Destroy((Object) this);
  }

