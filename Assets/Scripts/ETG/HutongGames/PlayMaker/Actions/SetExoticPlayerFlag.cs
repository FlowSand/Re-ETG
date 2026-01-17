// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetExoticPlayerFlag
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(".NPCs")]
[HutongGames.PlayMaker.Tooltip("Sets various unusual player flags.")]
public class SetExoticPlayerFlag : FsmStateAction
{
  public FsmBool SetGunGameTrue;
  public FsmBool SetChallengeModTrue;
  public FsmBool SetMegaChallengeModeTrue;
  public FsmBool ToggleTurboMode;
  public FsmBool ToggleRainbowRun;

  public override void OnEnter()
  {
    TalkDoerLite component = this.Owner.GetComponent<TalkDoerLite>();
    if ((bool) (Object) component && (bool) (Object) component.TalkingPlayer)
    {
      if (this.SetGunGameTrue.Value)
        SetExoticPlayerFlag.SetGunGame(true);
      if (this.SetChallengeModTrue.Value)
      {
        ChallengeManager.ChallengeModeType = ChallengeModeType.ChallengeMode;
        component.TalkingPlayer.PlayEffectOnActor((GameObject) BraveResources.Load("Global VFX/VFX_DaisukeFavor"), new Vector3(0.0f, -0.625f, 0.0f));
      }
      else if (this.SetMegaChallengeModeTrue.Value)
      {
        ChallengeManager.ChallengeModeType = ChallengeModeType.ChallengeMegaMode;
        component.TalkingPlayer.PlayEffectOnActor((GameObject) BraveResources.Load("Global VFX/VFX_DaisukeFavor"), new Vector3(0.0f, -0.625f, 0.0f));
      }
      else if (this.ToggleRainbowRun.Value)
      {
        if (!GameStatsManager.Instance.rainbowRunToggled)
        {
          GameStatsManager.Instance.rainbowRunToggled = true;
          int num = (int) AkSoundEngine.PostEvent("Play_NPC_Blessing_Rainbow_Get_01", this.Owner.gameObject);
          GameUIRoot.Instance.notificationController.DoCustomNotification(GameUIRoot.Instance.FoyerAmmonomiconLabel.ForceGetLocalizedValue("#RAINBOW_POPUP_ACTIVE"), string.Empty, (tk2dSpriteCollectionData) null, -1, forceSingleLine: true);
          component.TalkingPlayer.PlayEffectOnActor((GameObject) BraveResources.Load("Global VFX/VFX_BowlerFavor"), new Vector3(0.0f, -0.625f, 0.0f));
        }
        else
        {
          GameStatsManager.Instance.rainbowRunToggled = false;
          int num = (int) AkSoundEngine.PostEvent("Play_NPC_Blessing_Rainbow_Remove_01", this.Owner.gameObject);
          GameUIRoot.Instance.notificationController.DoCustomNotification(GameUIRoot.Instance.FoyerAmmonomiconLabel.ForceGetLocalizedValue("#RAINBOW_POPUP_INACTIVE"), string.Empty, (tk2dSpriteCollectionData) null, -1, forceSingleLine: true);
        }
        GameOptions.Save();
      }
      else if (this.ToggleTurboMode.Value)
      {
        if (!GameStatsManager.Instance.isTurboMode)
        {
          GameStatsManager.Instance.isTurboMode = true;
          int num = (int) AkSoundEngine.PostEvent("Play_NPC_Blessing_Speed_Tonic_01", this.Owner.gameObject);
          if (GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.ENGLISH)
            GameUIRoot.Instance.notificationController.DoCustomNotification("Game Speed: Turbo", string.Empty, (tk2dSpriteCollectionData) null, -1, forceSingleLine: true);
          else
            GameUIRoot.Instance.notificationController.DoCustomNotification(GameUIRoot.Instance.FoyerAmmonomiconLabel.ForceGetLocalizedValue("#OPTIONS_GAMESPEED"), GameUIRoot.Instance.FoyerAmmonomiconLabel.ForceGetLocalizedValue("#OPTIONS_GAMESPEED_TURBO"), (tk2dSpriteCollectionData) null, -1);
        }
        else
        {
          GameStatsManager.Instance.isTurboMode = false;
          int num = (int) AkSoundEngine.PostEvent("Play_NPC_Blessing_Slow_Tonic_01", this.Owner.gameObject);
          if (GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.ENGLISH)
            GameUIRoot.Instance.notificationController.DoCustomNotification("Game Speed: Classic", string.Empty, (tk2dSpriteCollectionData) null, -1, forceSingleLine: true);
          else
            GameUIRoot.Instance.notificationController.DoCustomNotification(GameUIRoot.Instance.FoyerAmmonomiconLabel.ForceGetLocalizedValue("#OPTIONS_GAMESPEED"), GameUIRoot.Instance.FoyerAmmonomiconLabel.ForceGetLocalizedValue("#OPTIONS_GAMESPEED_NORMAL"), (tk2dSpriteCollectionData) null, -1);
        }
        GameOptions.Save();
      }
    }
    this.Finish();
  }

  public static void SetGunGame(bool doEffects)
  {
    for (int index1 = 0; index1 < GameManager.Instance.AllPlayers.Length; ++index1)
    {
      PlayerController allPlayer = GameManager.Instance.AllPlayers[index1];
      allPlayer.CharacterUsesRandomGuns = true;
      for (int index2 = 1; index2 < allPlayer.inventory.AllGuns.Count; index2 = index2 - 1 + 1)
      {
        Gun allGun = allPlayer.inventory.AllGuns[index2];
        allPlayer.inventory.RemoveGunFromInventory(allGun);
        Object.Destroy((Object) allGun.gameObject);
      }
      if (doEffects)
        allPlayer.PlayEffectOnActor((GameObject) BraveResources.Load("Global VFX/VFX_MagicFavor_Light"), new Vector3(0.0f, -0.625f, 0.0f), alreadyMiddleCenter: true);
    }
  }
}
