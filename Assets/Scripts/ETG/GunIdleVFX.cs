// Decompiled with JetBrains decompiler
// Type: GunIdleVFX
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class GunIdleVFX : MonoBehaviour
{
  public tk2dSpriteAnimator idleVFX;
  private Gun m_gun;

  private void Awake() => this.m_gun = this.GetComponent<Gun>();

  private void Update()
  {
    if (!this.idleVFX.gameObject.activeSelf || !(bool) (Object) this.m_gun || !(bool) (Object) this.m_gun.sprite)
      return;
    if (!this.idleVFX.IsPlaying(this.idleVFX.DefaultClip))
      this.idleVFX.Play();
    this.idleVFX.sprite.FlipY = this.m_gun.sprite.FlipY;
    this.idleVFX.transform.localPosition = this.idleVFX.transform.localPosition.WithY(Mathf.Abs(this.idleVFX.transform.localPosition.y) * (!this.idleVFX.sprite.FlipY ? 1f : -1f));
    this.idleVFX.renderer.enabled = this.m_gun.renderer.enabled;
  }
}
