// Decompiled with JetBrains decompiler
// Type: BreakableObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

public class BreakableObject : MonoBehaviour
  {
    public string breakAnimName = string.Empty;
    private SpeculativeRigidbody m_srb;

    private void Start()
    {
      this.m_srb = this.GetComponent<SpeculativeRigidbody>();
      this.m_srb.OnRigidbodyCollision += new SpeculativeRigidbody.OnRigidbodyCollisionDelegate(this.OnRigidbodyCollision);
    }

    private void OnRigidbodyCollision(CollisionData rigidbodyCollision) => this.Break();

    private void Break()
    {
      tk2dSpriteAnimator component = this.GetComponent<tk2dSpriteAnimator>();
      if (this.breakAnimName != string.Empty)
        component.Play(this.breakAnimName);
      else
        component.Play();
      Object.Destroy((Object) this.m_srb);
    }
  }

