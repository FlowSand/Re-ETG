using System;
using UnityEngine;

#nullable disable

public class DungeonDoorSubsidiaryBlocker : BraveBehaviour
  {
    public bool hideSealAnimators = true;
    public tk2dSpriteAnimator sealAnimator;
    public tk2dSpriteAnimator chainAnimator;
    public tk2dSpriteAnimator sealVFX;
    public float unsealDistanceMaximum = -1f;
    public GameObject unsealedVFXOverride;
    public string sealAnimationName;
    public string sealChainAnimationName;
    public string unsealAnimationName;
    public string unsealChainAnimationName;
    public string playerNearSealedAnimationName;
    public string playerNearChainAnimationName;
    [NonSerialized]
    public bool isSealed;
    public bool northSouth;
    public bool usesUnsealScreenShake;
    public ScreenShakeSettings unsealScreenShake;
    [HideInInspector]
    public DungeonDoorController parentDoor;

    public void ToggleRenderers(bool visible)
    {
      if ((UnityEngine.Object) this.sealAnimator != (UnityEngine.Object) null)
        this.sealAnimator.GetComponent<Renderer>().enabled = visible;
      if (!((UnityEngine.Object) this.chainAnimator != (UnityEngine.Object) null))
        return;
      this.chainAnimator.GetComponent<Renderer>().enabled = visible;
    }

    private void Update()
    {
      if (!((UnityEngine.Object) this.parentDoor != (UnityEngine.Object) null) || !this.parentDoor.northSouth || !this.isSealed || string.IsNullOrEmpty(this.playerNearSealedAnimationName))
        return;
      if ((double) Vector2.Distance(this.sealAnimator.GetComponent<SpeculativeRigidbody>().UnitCenter, GameManager.Instance.PrimaryPlayer.specRigidbody.UnitCenter) < 4.0)
      {
        if (this.sealAnimator.IsPlaying(this.playerNearSealedAnimationName) || this.sealAnimator.IsPlaying(this.unsealAnimationName) || this.sealAnimator.IsPlaying(this.sealAnimationName))
          return;
        this.sealAnimator.Play(this.playerNearSealedAnimationName);
      }
      else
      {
        if (!this.sealAnimator.IsPlaying(this.playerNearSealedAnimationName))
          return;
        this.sealAnimator.Stop();
        tk2dSpriteAnimationClip clipByName = this.sealAnimator.GetClipByName(this.sealAnimationName);
        this.sealAnimator.Sprite.SetSprite(clipByName.frames[clipByName.frames.Length - 1].spriteId);
      }
    }

    public void OnSealAnimationEvent(
      tk2dSpriteAnimator animator,
      tk2dSpriteAnimationClip clip,
      int frameNo)
    {
      if (!(clip.GetFrame(frameNo).eventInfo == "SealVFX") || !((UnityEngine.Object) this.sealVFX != (UnityEngine.Object) null))
        return;
      this.sealVFX.gameObject.SetActive(true);
      this.sealVFX.Play();
    }

    public void OnUnsealAnimationCompleted(tk2dSpriteAnimator a, tk2dSpriteAnimationClip c)
    {
      if (this.hideSealAnimators)
        a.gameObject.SetActive(false);
      if ((UnityEngine.Object) a.GetComponent<SpeculativeRigidbody>() != (UnityEngine.Object) null)
        a.GetComponent<SpeculativeRigidbody>().enabled = false;
      if (!((UnityEngine.Object) this.unsealedVFXOverride != (UnityEngine.Object) null))
        return;
      this.unsealedVFXOverride.SetActive(true);
    }

    public void Seal()
    {
      if (!string.IsNullOrEmpty(this.sealAnimationName))
      {
        this.sealAnimator.alwaysUpdateOffscreen = true;
        this.sealAnimator.AnimationCompleted = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>) null;
        this.sealAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.OnSealAnimationEvent);
        this.sealAnimator.gameObject.SetActive(true);
        this.sealAnimator.Play(this.sealAnimationName);
        int num = (int) AkSoundEngine.PostEvent("Play_OBJ_gate_slam_01", this.gameObject);
      }
      if (!string.IsNullOrEmpty(this.sealChainAnimationName))
        this.chainAnimator.Play(this.sealChainAnimationName);
      if ((UnityEngine.Object) this.sealAnimator.GetComponent<SpeculativeRigidbody>() != (UnityEngine.Object) null)
        this.sealAnimator.GetComponent<SpeculativeRigidbody>().enabled = true;
      this.isSealed = true;
    }

    public void Unseal()
    {
      if (!this.isSealed)
        return;
      if (!string.IsNullOrEmpty(this.unsealAnimationName))
      {
        this.sealAnimator.alwaysUpdateOffscreen = true;
        this.sealAnimator.Play(this.unsealAnimationName);
        this.sealAnimator.AnimationCompleted += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.OnUnsealAnimationCompleted);
        this.sealAnimator.AnimationEventTriggered = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>) null;
        int num = (int) AkSoundEngine.PostEvent("Play_OBJ_gate_open_01", this.gameObject);
      }
      if (!string.IsNullOrEmpty(this.unsealChainAnimationName))
        this.chainAnimator.Play(this.unsealChainAnimationName);
      if (this.usesUnsealScreenShake)
        GameManager.Instance.MainCameraController.DoScreenShake(this.unsealScreenShake, new Vector2?((Vector2) this.transform.position));
      this.isSealed = false;
    }

    protected override void OnDestroy() => base.OnDestroy();
  }

