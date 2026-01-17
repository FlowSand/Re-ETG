// Decompiled with JetBrains decompiler
// Type: CharacterCostumeSwapper
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class CharacterCostumeSwapper : MonoBehaviour, IPlayerInteractable
{
  public PlayableCharacters TargetCharacter;
  public tk2dSprite CostumeSprite;
  public tk2dSprite AlternateCostumeSprite;
  public tk2dSpriteAnimation TargetLibrary;
  public bool HasCustomTrigger;
  public bool CustomTriggerIsFlag;
  public GungeonFlags TriggerFlag;
  public bool CustomTriggerIsSpecialReserve;
  private bool m_active;

  private void Start()
  {
    bool flag = GameStatsManager.Instance.GetCharacterSpecificFlag(this.TargetCharacter, CharacterSpecificGungeonFlags.KILLED_PAST);
    if (this.HasCustomTrigger)
    {
      if (this.CustomTriggerIsFlag)
        flag = GameStatsManager.Instance.GetFlag(this.TriggerFlag);
      else if (this.CustomTriggerIsSpecialReserve)
        flag = !GameStatsManager.Instance.GetFlag(GungeonFlags.SECRET_BULLETMAN_SEEN_05) && false;
    }
    if (flag)
    {
      this.m_active = true;
      if (this.TargetCharacter == PlayableCharacters.Guide)
      {
        this.CostumeSprite.HeightOffGround = 0.25f;
        this.AlternateCostumeSprite.HeightOffGround = 0.25f;
        this.CostumeSprite.UpdateZDepth();
        this.AlternateCostumeSprite.UpdateZDepth();
      }
      this.AlternateCostumeSprite.renderer.enabled = true;
      this.CostumeSprite.renderer.enabled = false;
    }
    else
    {
      this.m_active = false;
      this.AlternateCostumeSprite.renderer.enabled = false;
      this.CostumeSprite.renderer.enabled = false;
    }
  }

  private void Update()
  {
    if (!this.m_active || GameManager.IsReturningToBreach || GameManager.Instance.IsSelectingCharacter || GameManager.Instance.IsLoadingLevel || (Object) GameManager.Instance.PrimaryPlayer == (Object) null || this.TargetCharacter == PlayableCharacters.CoopCultist || GameManager.Instance.PrimaryPlayer.characterIdentity == this.TargetCharacter)
      return;
    SpriteOutlineManager.RemoveOutlineFromSprite((tk2dBaseSprite) this.AlternateCostumeSprite);
    SpriteOutlineManager.RemoveOutlineFromSprite((tk2dBaseSprite) this.CostumeSprite);
    this.AlternateCostumeSprite.renderer.enabled = true;
    this.CostumeSprite.renderer.enabled = false;
  }

  public float GetDistanceToPoint(Vector2 point)
  {
    if (!this.m_active)
      return 1000f;
    return this.AlternateCostumeSprite.renderer.enabled ? Vector2.Distance(point, this.AlternateCostumeSprite.WorldCenter) : Vector2.Distance(point, this.CostumeSprite.WorldCenter);
  }

  public void OnEnteredRange(PlayerController interactor)
  {
    if (interactor.characterIdentity != this.TargetCharacter)
      return;
    if (this.AlternateCostumeSprite.renderer.enabled)
      SpriteOutlineManager.AddOutlineToSprite((tk2dBaseSprite) this.AlternateCostumeSprite, Color.white);
    else
      SpriteOutlineManager.AddOutlineToSprite((tk2dBaseSprite) this.CostumeSprite, Color.white);
  }

  public void OnExitRange(PlayerController interactor)
  {
    if (interactor.characterIdentity != this.TargetCharacter)
      return;
    if (this.AlternateCostumeSprite.renderer.enabled)
      SpriteOutlineManager.RemoveOutlineFromSprite((tk2dBaseSprite) this.AlternateCostumeSprite);
    else
      SpriteOutlineManager.RemoveOutlineFromSprite((tk2dBaseSprite) this.CostumeSprite);
  }

  public void Interact(PlayerController interactor)
  {
    if (interactor.characterIdentity != this.TargetCharacter || !this.m_active)
      return;
    if (interactor.IsUsingAlternateCostume)
    {
      interactor.SwapToAlternateCostume();
    }
    else
    {
      if ((bool) (Object) this.TargetLibrary)
        interactor.AlternateCostumeLibrary = this.TargetLibrary;
      interactor.SwapToAlternateCostume();
    }
    SpriteOutlineManager.RemoveOutlineFromSprite((tk2dBaseSprite) this.AlternateCostumeSprite);
    SpriteOutlineManager.RemoveOutlineFromSprite((tk2dBaseSprite) this.CostumeSprite);
    this.AlternateCostumeSprite.renderer.enabled = !this.AlternateCostumeSprite.renderer.enabled;
    this.CostumeSprite.renderer.enabled = !this.CostumeSprite.renderer.enabled;
  }

  public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
  {
    shouldBeFlipped = false;
    return string.Empty;
  }

  public float GetOverrideMaxDistance() => -1f;
}
