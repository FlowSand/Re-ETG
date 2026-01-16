// Decompiled with JetBrains decompiler
// Type: SimpleSpriteRotator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class SimpleSpriteRotator : MonoBehaviour
{
  public float angularVelocity;
  public float acceleration;
  public bool UseWorldCenter = true;
  public bool ForceUpdateZDepth;
  public bool RotateParent;
  public bool RotateDuringBossIntros;
  public bool RandomStartingAngle;
  private Transform m_transform;
  private tk2dSprite m_sprite;

  private void Start()
  {
    this.m_transform = !this.RotateParent ? this.transform : this.transform.parent;
    this.m_sprite = this.GetComponent<tk2dSprite>();
    if (!this.RandomStartingAngle)
      return;
    this.DoRotation((float) Random.Range(0, 360));
  }

  private void Update()
  {
    float num = BraveTime.DeltaTime;
    if (this.RotateDuringBossIntros && GameManager.IsBossIntro)
      num = GameManager.INVARIANT_DELTA_TIME;
    this.angularVelocity += this.acceleration * num;
    this.DoRotation(this.angularVelocity * num);
  }

  private void DoRotation(float degrees)
  {
    if (this.UseWorldCenter)
      this.m_transform.RotateAround((Vector3) this.m_sprite.WorldCenter, Vector3.forward, degrees);
    else
      this.m_transform.Rotate(Vector3.forward, degrees);
    if (!this.ForceUpdateZDepth)
      return;
    this.m_sprite.ForceRotationRebuild();
    this.m_sprite.UpdateZDepth();
  }
}
