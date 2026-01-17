// Decompiled with JetBrains decompiler
// Type: GaseousFormPlayerItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Items.Active
{
    public class GaseousFormPlayerItem : PlayerItem
    {
      public float Duration = 5f;

      protected override void DoEffect(PlayerController user)
      {
        if (!(bool) (Object) user)
          return;
        user.StartCoroutine(this.HandleDuration(user));
        int num = (int) AkSoundEngine.PostEvent("Play_OBJ_metalskin_activate_01", this.gameObject);
      }

      private void ChangeRendering(PlayerController user, bool val)
      {
        if (!(bool) (Object) user)
          return;
        if (val)
        {
          user.ChangeSpecialShaderFlag(0, 1f);
          user.FlatColorOverridden = true;
          user.ChangeFlatColorOverride(new Color(0.4f, 0.31f, 0.49f, 1f));
          user.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox, CollisionLayer.EnemyCollider));
          user.ToggleShadowVisiblity(false);
          SpriteOutlineManager.RemoveOutlineFromSprite(user.sprite, true);
        }
        else
        {
          user.ChangeSpecialShaderFlag(0, 0.0f);
          user.FlatColorOverridden = false;
          user.ChangeFlatColorOverride(Color.clear);
          user.specRigidbody.RemoveCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox, CollisionLayer.EnemyCollider));
          user.ToggleShadowVisiblity(true);
          if (SpriteOutlineManager.HasOutline(user.sprite))
            return;
          SpriteOutlineManager.AddOutlineToSprite(user.sprite, user.outlineColor, 0.1f);
        }
      }

      [DebuggerHidden]
      private IEnumerator HandleDuration(PlayerController user)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new GaseousFormPlayerItem__HandleDurationc__Iterator0()
        {
          user = user,
          _this = this
        };
      }

      protected override void OnPreDrop(PlayerController user)
      {
        if (!this.IsCurrentlyActive)
          return;
        this.StopAllCoroutines();
        if ((bool) (Object) user)
        {
          this.ChangeRendering(user, false);
          user.SetIsFlying(false, "gaseousform");
          user.IsEthereal = false;
          if ((bool) (Object) user.healthHaver)
            user.healthHaver.IsVulnerable = true;
          user.SetCapableOfStealing(false, nameof (GaseousFormPlayerItem));
          if ((bool) (Object) user.specRigidbody)
          {
            user.specRigidbody.RemoveCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox, CollisionLayer.EnemyCollider, CollisionLayer.Projectile));
            PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(user.specRigidbody);
          }
          this.IsCurrentlyActive = false;
        }
        if (!(bool) (Object) this)
          return;
        int num = (int) AkSoundEngine.PostEvent("Play_OBJ_metalskin_end_01", this.gameObject);
      }

      protected override void OnDestroy() => base.OnDestroy();
    }

}
