// Decompiled with JetBrains decompiler
// Type: DraGunArmController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class DraGunArmController : BraveBehaviour
{
  public GameObject shoulder;
  public List<GameObject> balls;
  public GameObject hand;
  private int[] offsets = new int[6]
  {
    0,
    -3,
    -5,
    -6,
    -5,
    -3
  };
  private DraGunController m_body;
  private tk2dBaseSprite shoulderSprite;
  private TileSpriteClipper handSpriteClipper;
  private List<TileSpriteClipper> armSpriteClippers;

  public void Start()
  {
    this.m_body = this.transform.parent.GetComponent<DraGunController>();
    this.m_body.specRigidbody.Initialize();
    float unitBottom = this.m_body.specRigidbody.PrimaryPixelCollider.UnitBottom;
    this.armSpriteClippers = new List<TileSpriteClipper>(this.balls.Count);
    for (int index = 0; index < this.balls.Count; ++index)
    {
      TileSpriteClipper orAddComponent = this.balls[index].GetComponentInChildren<tk2dBaseSprite>().gameObject.GetOrAddComponent<TileSpriteClipper>();
      orAddComponent.doOptimize = true;
      orAddComponent.updateEveryFrame = true;
      orAddComponent.clipMode = TileSpriteClipper.ClipMode.ClipBelowY;
      orAddComponent.clipY = unitBottom;
      this.armSpriteClippers.Add(orAddComponent);
    }
    this.handSpriteClipper = this.hand.GetComponentInChildren<tk2dBaseSprite>().gameObject.GetOrAddComponent<TileSpriteClipper>();
    this.handSpriteClipper.doOptimize = true;
    this.handSpriteClipper.updateEveryFrame = true;
    this.handSpriteClipper.clipMode = TileSpriteClipper.ClipMode.ClipBelowY;
    this.handSpriteClipper.clipY = unitBottom;
    this.handSpriteClipper.enabled = false;
    this.shoulderSprite = this.shoulder.GetComponentInChildren<tk2dBaseSprite>();
    this.m_body.sprite.SpriteChanged += new Action<tk2dBaseSprite>(this.BodySpriteChanged);
  }

  protected override void OnDestroy() => base.OnDestroy();

  public void ClipArmSprites() => this.SetClipArmSprites(true);

  public void UnclipArmSprites() => this.SetClipArmSprites(false);

  public void SetClipArmSprites(bool clip)
  {
    for (int index = 0; index < this.armSpriteClippers.Count; ++index)
      this.armSpriteClippers[index].enabled = clip;
  }

  public void ClipHandSprite() => this.SetClipHandSprite(true);

  public void UnclipHandSprite() => this.SetClipHandSprite(false);

  public void SetClipHandSprite(bool clip) => this.handSpriteClipper.enabled = clip;

  private void BodySpriteChanged(tk2dBaseSprite obj)
  {
    if (this.m_body.spriteAnimator.CurrentClip == null)
      return;
    float unit = PhysicsEngine.PixelToUnit(this.offsets[Mathf.Min(Mathf.FloorToInt((float) this.m_body.spriteAnimator.CurrentFrame / (float) this.m_body.spriteAnimator.CurrentClip.frames.Length * 6f), 5)]);
    this.shoulderSprite.transform.localPosition = this.shoulderSprite.transform.localPosition.WithY(unit);
    for (int index = 0; index < this.armSpriteClippers.Count; ++index)
      this.armSpriteClippers[index].transform.localPosition = this.shoulderSprite.transform.localPosition.WithY(Mathf.Lerp(unit, 0.0f, (float) (((double) index + 1.0) / ((double) this.armSpriteClippers.Count + 1.0))));
  }
}
