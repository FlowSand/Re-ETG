// Decompiled with JetBrains decompiler
// Type: FoyerAlternateGunShrineController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Dungeon.Interactables
{
    public class FoyerAlternateGunShrineController : BraveBehaviour, IPlayerInteractable
    {
      public Transform talkPoint;
      public string displayTextKey;
      public string acceptOptionKey;
      public string declineOptionKey;
      public tk2dSpriteAnimator Flame;
      public tk2dBaseSprite AlternativeOutlineTarget;

      private bool IsCurrentlyActive
      {
        get
        {
          return GameManager.HasInstance && (bool) (Object) GameManager.Instance.PrimaryPlayer && GameManager.Instance.PrimaryPlayer.UsingAlternateStartingGuns;
        }
      }

      [DebuggerHidden]
      private IEnumerator Start()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new FoyerAlternateGunShrineController.\u003CStart\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }

      public void DoEffect(PlayerController interactor)
      {
        if (interactor.characterIdentity == PlayableCharacters.Eevee || interactor.characterIdentity == PlayableCharacters.Gunslinger)
          return;
        interactor.UsingAlternateStartingGuns = !interactor.UsingAlternateStartingGuns;
        interactor.ReinitializeGuns();
        interactor.PlayEffectOnActor((GameObject) ResourceCache.Acquire("Global VFX/VFX_AltGunShrine"), Vector3.zero);
      }

      public void Interact(PlayerController interactor)
      {
        if (TextBoxManager.HasTextBox(this.talkPoint) || interactor.characterIdentity == PlayableCharacters.Eevee || interactor.characterIdentity == PlayableCharacters.Gunslinger)
          return;
        this.StartCoroutine(this.HandleShrineConversation(interactor));
      }

      [DebuggerHidden]
      private IEnumerator HandleShrineConversation(PlayerController interactor)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new FoyerAlternateGunShrineController.\u003CHandleShrineConversation\u003Ec__Iterator1()
        {
          interactor = interactor,
          \u0024this = this
        };
      }

      private void Update()
      {
        if (Foyer.DoIntroSequence || Foyer.DoMainMenu || !GameManager.HasInstance || Dungeon.IsGenerating || GameManager.Instance.IsLoadingLevel || !this.gameObject.activeSelf)
          return;
        this.Flame.sprite.HeightOffGround = -0.5f;
        this.Flame.renderer.enabled = this.IsCurrentlyActive;
      }

      public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
      {
        shouldBeFlipped = false;
        return string.Empty;
      }

      public float GetDistanceToPoint(Vector2 point)
      {
        if ((Object) this.sprite == (Object) null)
          return 100f;
        Vector3 b = (Vector3) BraveMathCollege.ClosestPointOnRectangle(point, this.specRigidbody.UnitBottomLeft, this.specRigidbody.UnitDimensions);
        return Vector2.Distance(point, (Vector2) b) / 1.5f;
      }

      public float GetOverrideMaxDistance() => -1f;

      public void OnEnteredRange(PlayerController interactor)
      {
        if (interactor.characterIdentity == PlayableCharacters.Eevee || interactor.characterIdentity == PlayableCharacters.Gunslinger)
          return;
        if ((Object) this.AlternativeOutlineTarget != (Object) null)
          SpriteOutlineManager.AddOutlineToSprite(this.AlternativeOutlineTarget, Color.white);
        else
          SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.white);
      }

      public void OnExitRange(PlayerController interactor)
      {
        if (interactor.characterIdentity == PlayableCharacters.Eevee || interactor.characterIdentity == PlayableCharacters.Gunslinger)
          return;
        SpriteOutlineManager.RemoveOutlineFromSprite(!((Object) this.AlternativeOutlineTarget != (Object) null) ? this.sprite : this.AlternativeOutlineTarget);
      }
    }

}
