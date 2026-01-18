using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

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

