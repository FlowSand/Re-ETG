// Decompiled with JetBrains decompiler
// Type: SimpleSecretRoomTrigger
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using UnityEngine;

#nullable disable

public class SimpleSecretRoomTrigger : BraveBehaviour, IPlayerInteractable
  {
    public SecretRoomManager referencedSecretRoom;
    public RoomHandler parentRoom;

    public void Initialize() => this.parentRoom.RegisterInteractable((IPlayerInteractable) this);

    private void HandleTrigger(
      SpeculativeRigidbody specRigidbody,
      SpeculativeRigidbody sourceSpecRigidbody)
    {
      if (this.referencedSecretRoom.IsOpen || !((Object) specRigidbody.projectile != (Object) null))
        return;
      this.parentRoom.DeregisterInteractable((IPlayerInteractable) this);
      if ((Object) this.spriteAnimator != (Object) null)
        this.spriteAnimator.Play();
      this.referencedSecretRoom.OpenDoor();
    }

    public float GetDistanceToPoint(Vector2 point)
    {
      return Vector2.Distance(point, this.sprite.WorldCenter);
    }

    public float GetOverrideMaxDistance() => -1f;

    public void OnEnteredRange(PlayerController interactor)
    {
      if (this.referencedSecretRoom.IsOpen || !(bool) (Object) this)
        return;
      SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.white);
      this.sprite.UpdateZDepth();
    }

    public void OnExitRange(PlayerController interactor)
    {
      if (!(bool) (Object) this)
        return;
      SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite);
    }

    public void Disable() => this.parentRoom.DeregisterInteractable((IPlayerInteractable) this);

    public void Interact(PlayerController interactor)
    {
      this.parentRoom.DeregisterInteractable((IPlayerInteractable) this);
      if (this.referencedSecretRoom.IsOpen)
        return;
      if ((Object) this.spriteAnimator != (Object) null)
        this.spriteAnimator.Play();
      this.referencedSecretRoom.OpenDoor();
    }

    public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
    {
      shouldBeFlipped = false;
      return string.Empty;
    }

    protected override void OnDestroy() => base.OnDestroy();
  }

