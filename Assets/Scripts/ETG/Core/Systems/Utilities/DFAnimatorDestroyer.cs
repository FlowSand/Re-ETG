// Decompiled with JetBrains decompiler
// Type: DFAnimatorDestroyer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

public class DFAnimatorDestroyer : MonoBehaviour
  {
    protected dfSpriteAnimation m_animator;

    private void Start() => this.m_animator = this.GetComponent<dfSpriteAnimation>();

    private void Update()
    {
      if (!this.m_animator.IsPlaying && !this.m_animator.AutoRun)
        Object.Destroy((Object) this.gameObject);
      if (!this.m_animator.IsPlaying)
        return;
      this.m_animator.AutoRun = false;
    }
  }

