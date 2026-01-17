// Decompiled with JetBrains decompiler
// Type: SpriteAnimatorChanger
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.VFX.Animation
{
    public class SpriteAnimatorChanger : MonoBehaviour
    {
      public float time;
      public string newAnimation;
      private tk2dSpriteAnimator m_animator;
      private float m_timer;

      public void Awake() => this.m_animator = this.GetComponent<tk2dSpriteAnimator>();

      public void Update()
      {
        this.m_timer += BraveTime.DeltaTime;
        if ((double) this.m_timer <= (double) this.time)
          return;
        this.m_animator.Play(this.newAnimation);
        Object.Destroy((Object) this);
      }
    }

}
