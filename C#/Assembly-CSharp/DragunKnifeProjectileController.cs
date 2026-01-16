// Decompiled with JetBrains decompiler
// Type: DragunKnifeProjectileController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class DragunKnifeProjectileController : BraveBehaviour
{
  [EnemyIdentifier]
  public string knifeGuid;

  public void Start()
  {
    this.specRigidbody.OnTileCollision += new SpeculativeRigidbody.OnTileCollisionDelegate(this.OnTileCollision);
  }

  private void OnTileCollision(CollisionData tileCollision)
  {
    if (!(bool) (Object) this.projectile.Owner || !(this.projectile.Owner is AIActor))
      return;
    AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(this.knifeGuid);
    Vector2 contact = tileCollision.Contact;
    if ((double) tileCollision.Normal.x < 0.0)
      contact.x -= PhysicsEngine.PixelToUnit(orLoadByGuid.specRigidbody.PrimaryPixelCollider.ManualWidth);
    AIActor aiActor = AIActor.Spawn(orLoadByGuid, contact.ToIntVector2() + new IntVector2(0, -1), (this.projectile.Owner as AIActor).ParentRoom);
    aiActor.aiAnimator.LockFacingDirection = true;
    aiActor.aiAnimator.FacingDirection = (double) tileCollision.Normal.x >= 0.0 ? 0.0f : 180f;
    aiActor.aiAnimator.Update();
    if ((double) tileCollision.Normal.x < 0.0)
    {
      PixelCollider primaryPixelCollider = aiActor.specRigidbody.PrimaryPixelCollider;
      int num = primaryPixelCollider.ManualWidth / 2;
      primaryPixelCollider.ManualOffsetX += num;
      primaryPixelCollider.ManualWidth -= num;
      aiActor.specRigidbody.ForceRegenerate();
    }
    else
    {
      PixelCollider primaryPixelCollider = aiActor.specRigidbody.PrimaryPixelCollider;
      int num = primaryPixelCollider.ManualWidth / 2;
      primaryPixelCollider.ManualWidth -= num;
      aiActor.specRigidbody.ForceRegenerate();
    }
  }
}
