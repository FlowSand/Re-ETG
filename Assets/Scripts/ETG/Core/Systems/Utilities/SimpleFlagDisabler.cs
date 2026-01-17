// Decompiled with JetBrains decompiler
// Type: SimpleFlagDisabler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class SimpleFlagDisabler : MonoBehaviour
    {
      [LongEnum]
      public GungeonFlags FlagToCheckFor;
      public bool DisableOnThisFlagValue;
      public bool UsesStatComparisonInstead;
      public TrackedStats RelevantStat = TrackedStats.NUMBER_ATTEMPTS;
      public int minStatValue = 1;
      public string ChangeSpriteInstead;
      public bool EnableOnGunGameMode;
      public bool DisableIfNotFoyer;

      [DebuggerHidden]
      private IEnumerator Start()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new SimpleFlagDisabler__Startc__Iterator0()
        {
          _this = this
        };
      }

      private void Update()
      {
        if (!this.EnableOnGunGameMode || GameManager.Instance.IsSelectingCharacter || !((Object) GameManager.Instance.PrimaryPlayer != (Object) null) || !GameManager.Instance.PrimaryPlayer.CharacterUsesRandomGuns && !ChallengeManager.CHALLENGE_MODE_ACTIVE)
          return;
        SpeculativeRigidbody component = this.GetComponent<SpeculativeRigidbody>();
        if (component.enabled)
          return;
        component.enabled = true;
        component.Reinitialize();
        this.GetComponent<MeshRenderer>().enabled = true;
      }

      private void Disable()
      {
        SpeculativeRigidbody component = this.GetComponent<SpeculativeRigidbody>();
        if (!string.IsNullOrEmpty(this.ChangeSpriteInstead))
        {
          this.GetComponent<tk2dBaseSprite>().SetSprite(this.ChangeSpriteInstead);
          if (!(bool) (Object) component)
            return;
          component.Reinitialize();
        }
        else
        {
          if ((bool) (Object) component)
            component.enabled = false;
          if (!this.EnableOnGunGameMode)
            this.gameObject.SetActive(false);
          else
            this.GetComponent<MeshRenderer>().enabled = false;
        }
      }
    }

}
