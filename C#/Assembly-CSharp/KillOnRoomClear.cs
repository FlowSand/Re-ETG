// Decompiled with JetBrains decompiler
// Type: KillOnRoomClear
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class KillOnRoomClear : BraveBehaviour
{
  [CheckAnimation(null)]
  public string overrideDeathAnim;
  public bool preventExplodeOnDeath;

  public void Start() => this.aiActor.ParentRoom.OnEnemiesCleared += new System.Action(this.RoomCleared);

  protected override void OnDestroy()
  {
    if ((bool) (UnityEngine.Object) this.aiActor && this.aiActor.ParentRoom != null)
      this.aiActor.ParentRoom.OnEnemiesCleared -= new System.Action(this.RoomCleared);
    base.OnDestroy();
  }

  private void RoomCleared()
  {
    if (!string.IsNullOrEmpty(this.overrideDeathAnim) && (bool) (UnityEngine.Object) this.aiAnimator)
    {
      bool flag = false;
      for (int index = 0; index < this.aiAnimator.OtherAnimations.Count; ++index)
      {
        if (this.aiAnimator.OtherAnimations[index].name == "death")
        {
          this.aiAnimator.OtherAnimations[index].anim.Type = DirectionalAnimation.DirectionType.Single;
          this.aiAnimator.OtherAnimations[index].anim.Prefix = this.overrideDeathAnim;
          flag = true;
        }
      }
      if (!flag)
      {
        AIAnimator.NamedDirectionalAnimation directionalAnimation = new AIAnimator.NamedDirectionalAnimation()
        {
          name = "death",
          anim = new DirectionalAnimation()
        };
        directionalAnimation.anim.Type = DirectionalAnimation.DirectionType.Single;
        directionalAnimation.anim.Prefix = this.overrideDeathAnim;
        directionalAnimation.anim.Flipped = new DirectionalAnimation.FlipType[1];
        this.aiAnimator.OtherAnimations.Add(directionalAnimation);
      }
    }
    if (this.preventExplodeOnDeath)
    {
      ExplodeOnDeath component = this.GetComponent<ExplodeOnDeath>();
      if ((bool) (UnityEngine.Object) component)
        component.enabled = false;
    }
    this.healthHaver.PreventAllDamage = false;
    this.healthHaver.ApplyDamage(100000f, Vector2.zero, "Death on Room Claer", damageCategory: DamageCategory.Unstoppable, ignoreInvulnerabilityFrames: true);
  }
}
