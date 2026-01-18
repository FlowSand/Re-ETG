using Dungeonator;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

[RequireComponent(typeof (GenericIntroDoer))]
public class HighPriestIntroDoer : SpecificIntroDoer
  {
    public AIAnimator head;
    private bool m_isMotionRestricted;
    private bool m_finished;
    private int m_minPlayerY;

    public void Start()
    {
      this.aiActor.ParentRoom.Entered += new RoomHandler.OnEnteredEventHandler(this.PlayerEnteredRoom);
      this.aiActor.healthHaver.OnPreDeath += new Action<Vector2>(this.OnPreDeath);
    }

    protected override void OnDestroy()
    {
      this.spriteAnimator.AnimationEventTriggered -= new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.BodyAnimationEventTriggered);
      this.spriteAnimator.AnimationCompleted -= new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.BodyAnimationComplete);
      this.head.spriteAnimator.AnimationCompleted -= new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.HeadAnimationComplete);
      this.RestrictMotion(false);
      if ((bool) (UnityEngine.Object) this.aiActor && this.aiActor.ParentRoom != null)
        this.aiActor.ParentRoom.Entered -= new RoomHandler.OnEnteredEventHandler(this.PlayerEnteredRoom);
      if ((bool) (UnityEngine.Object) this.aiActor && (bool) (UnityEngine.Object) this.aiActor.healthHaver)
        this.aiActor.healthHaver.OnPreDeath -= new Action<Vector2>(this.OnPreDeath);
      base.OnDestroy();
    }

    public override void StartIntro(List<tk2dSpriteAnimator> animators)
    {
      this.aiAnimator.PlayUntilFinished("intro");
      this.spriteAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.BodyAnimationEventTriggered);
      this.spriteAnimator.AnimationCompleted += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.BodyAnimationComplete);
      this.head.spriteAnimator.AnimationCompleted += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.HeadAnimationComplete);
      animators.Add(this.head.spriteAnimator);
      this.head.spriteAnimator.enabled = false;
    }

    public override bool IsIntroFinished => this.m_finished;

    public override void EndIntro() => this.aiAnimator.EndAnimation();

    public override void OnCleanup() => this.head.spriteAnimator.enabled = true;

    private void PlayerEnteredRoom(PlayerController playerController) => this.RestrictMotion(true);

    private void OnPreDeath(Vector2 finalDirection)
    {
      this.RestrictMotion(false);
      this.aiActor.ParentRoom.Entered -= new RoomHandler.OnEnteredEventHandler(this.PlayerEnteredRoom);
    }

    private void PlayerMovementRestrictor(
      SpeculativeRigidbody playerSpecRigidbody,
      IntVector2 prevPixelOffset,
      IntVector2 pixelOffset,
      ref bool validLocation)
    {
      if (!validLocation || pixelOffset.y >= prevPixelOffset.y || playerSpecRigidbody.PixelColliders[0].MinY + pixelOffset.y >= this.m_minPlayerY)
        return;
      validLocation = false;
    }

    private void BodyAnimationEventTriggered(
      tk2dSpriteAnimator animator,
      tk2dSpriteAnimationClip clip,
      int frame)
    {
      if (!(clip.GetFrame(frame).eventInfo == "show_head"))
        return;
      this.head.enabled = false;
      this.head.spriteAnimator.Play("gun_appear_intro");
    }

    private void BodyAnimationComplete(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip)
    {
      if (!(clip.name == "priest_recloak"))
        return;
      this.m_finished = true;
      this.head.enabled = true;
      this.spriteAnimator.Play("priest_idle");
    }

    private void HeadAnimationComplete(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip)
    {
      if (!(clip.name == "gun_appear_intro"))
        return;
      this.aiAnimator.PlayUntilFinished("recloak");
    }

    public void RestrictMotion(bool value)
    {
      if (this.m_isMotionRestricted == value)
        return;
      if (value)
      {
        if (!GameManager.HasInstance || GameManager.Instance.IsLoadingLevel || GameManager.IsReturningToBreach)
          return;
        this.m_minPlayerY = this.aiActor.ParentRoom.area.basePosition.y * 16 /*0x10*/ + 8;
        for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
          GameManager.Instance.AllPlayers[index].specRigidbody.MovementRestrictor += new SpeculativeRigidbody.MovementRestrictorDelegate(this.PlayerMovementRestrictor);
      }
      else
      {
        if (!GameManager.HasInstance || GameManager.IsReturningToBreach)
          return;
        for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
        {
          PlayerController allPlayer = GameManager.Instance.AllPlayers[index];
          if ((bool) (UnityEngine.Object) allPlayer)
            allPlayer.specRigidbody.MovementRestrictor -= new SpeculativeRigidbody.MovementRestrictorDelegate(this.PlayerMovementRestrictor);
        }
      }
      this.m_isMotionRestricted = value;
    }
  }

