// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.Teleport
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Handles NPC teleportation.")]
[ActionCategory(".NPCs")]
public class Teleport : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("Teleportation type; In and Out handle visibility and effects, Both also handles translation.")]
  public Teleport.Mode mode;
  [HutongGames.PlayMaker.Tooltip("How long the NPC is completely gone (i.e. the delay between the In finishing and the Out starting).")]
  public FsmFloat goneTime;
  [HutongGames.PlayMaker.Tooltip("How far the NPC should move during the teleport, in Unity units (i.e. tiles).")]
  public FsmVector2 positionDelta;
  [HutongGames.PlayMaker.Tooltip("When true, will ignore positionDelta and teleport to the end of the attached path.")]
  public bool useEndOfPath;
  [HutongGames.PlayMaker.Tooltip("If true, lerps any Brent lights on this object while the the teleport animation is playing.")]
  public FsmBool lerpLight = (FsmBool) false;
  [HutongGames.PlayMaker.Tooltip("The new light intensity to set to.")]
  public FsmFloat newLightIntensity;
  private TalkDoerLite m_talkDoer;
  private Teleport.State m_state;
  private Teleport.Mode m_submode;
  private float m_stateTimer;
  private IEnumerator m_coroutine;
  private SetBraveLightIntensity m_lightIntensityAction;
  private float m_cachedLightIntensity;

  public override void Reset() => this.mode = Teleport.Mode.In;

  public override string ErrorCheck()
  {
    string str = string.Empty;
    TalkDoerLite component = this.Owner.GetComponent<TalkDoerLite>();
    if ((Object) component.spriteAnimator == (Object) null && (Object) component.aiAnimator == (Object) null)
      return str + "Requires a 2D Toolkit animator or an AI Animator.\n";
    if ((Object) component.aiAnimator != (Object) null)
    {
      if ((this.mode == Teleport.Mode.In || this.mode == Teleport.Mode.Both) && !component.aiAnimator.HasDirectionalAnimation(component.teleportInSettings.anim))
        str = $"{str}Unknown animation {component.teleportInSettings.anim}.\n";
      if ((this.mode == Teleport.Mode.Out || this.mode == Teleport.Mode.Both) && !component.aiAnimator.HasDirectionalAnimation(component.teleportOutSettings.anim))
        str = $"{str}Unknown animation {component.teleportOutSettings.anim}.\n";
    }
    else if ((Object) component.spriteAnimator != (Object) null)
    {
      if ((this.mode == Teleport.Mode.In || this.mode == Teleport.Mode.Both) && component.spriteAnimator.GetClipByName(component.teleportInSettings.anim) == null)
        str = $"{str}Unknown animation {component.teleportInSettings.anim}.\n";
      if ((this.mode == Teleport.Mode.Out || this.mode == Teleport.Mode.Both) && component.spriteAnimator.GetClipByName(component.teleportOutSettings.anim) == null)
        str = $"{str}Unknown animation {component.teleportOutSettings.anim}.\n";
    }
    return str;
  }

  public override void OnEnter()
  {
    this.m_talkDoer = this.Owner.GetComponent<TalkDoerLite>();
    this.m_coroutine = (IEnumerator) null;
    this.m_lightIntensityAction = (SetBraveLightIntensity) null;
    if (this.mode == Teleport.Mode.In)
    {
      this.m_state = Teleport.State.TeleportIn;
      this.m_coroutine = this.HandleAnimAndVfx(this.m_talkDoer.teleportInSettings);
    }
    else
    {
      if (this.mode != Teleport.Mode.Out && this.mode != Teleport.Mode.Both)
        return;
      this.m_state = Teleport.State.TeleportOut;
      this.m_coroutine = this.HandleAnimAndVfx(this.m_talkDoer.teleportOutSettings);
    }
  }

  public override void OnUpdate()
  {
    if (this.m_state == Teleport.State.TeleportOut)
    {
      if (this.m_coroutine.MoveNext())
        return;
      if (this.mode == Teleport.Mode.Both)
      {
        this.m_state = Teleport.State.MidStep;
        this.m_stateTimer = this.goneTime.Value;
      }
      else
        this.Finish();
    }
    else if (this.m_state == Teleport.State.MidStep)
    {
      this.m_stateTimer -= BraveTime.DeltaTime;
      if ((double) this.m_stateTimer > 0.0)
        return;
      PathMover component = this.m_talkDoer.GetComponent<PathMover>();
      this.m_talkDoer.transform.position = !this.useEndOfPath ? this.m_talkDoer.transform.position + (Vector3) this.positionDelta.Value : component.GetPositionOfNode(component.Path.nodes.Count - 1).ToVector3ZUp();
      this.m_talkDoer.specRigidbody.Reinitialize();
      PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(this.m_talkDoer.specRigidbody, new int?(CollisionMask.LayerToMask(CollisionLayer.PlayerCollider)));
      this.m_state = Teleport.State.TeleportIn;
      this.m_coroutine = this.HandleAnimAndVfx(this.m_talkDoer.teleportInSettings);
    }
    else
    {
      if (this.m_state != Teleport.State.TeleportIn || this.m_coroutine.MoveNext())
        return;
      PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(this.m_talkDoer.specRigidbody, new int?(CollisionMask.LayerToMask(CollisionLayer.PlayerCollider)));
      this.Finish();
    }
  }

  [DebuggerHidden]
  private IEnumerator HandleAnimAndVfx(TalkDoerLite.TeleportSettings teleportSettings)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new Teleport__HandleAnimAndVfxc__Iterator0()
    {
      teleportSettings = teleportSettings,
      _this = this
    };
  }

  private void PlayAnim(string anim)
  {
    if (this.m_state == Teleport.State.TeleportIn)
    {
      SetNpcVisibility.SetVisible(this.m_talkDoer, true);
      this.m_talkDoer.ShowOutlines = true;
    }
    if ((bool) (Object) this.m_talkDoer.aiAnimator)
      this.m_talkDoer.aiAnimator.PlayUntilCancelled(anim);
    else if ((bool) (Object) this.m_talkDoer.spriteAnimator)
      this.m_talkDoer.spriteAnimator.Play(anim);
    if (!this.lerpLight.Value)
      return;
    float num = this.mode != Teleport.Mode.Both ? this.newLightIntensity.Value : (this.m_state != Teleport.State.TeleportOut ? this.m_cachedLightIntensity : 0.0f);
    this.m_lightIntensityAction = new SetBraveLightIntensity();
    this.m_lightIntensityAction.specifyLights = new ShadowSystem[0];
    this.m_lightIntensityAction.intensity = (FsmFloat) num;
    this.m_lightIntensityAction.transitionTime = (FsmFloat) this.m_talkDoer.spriteAnimator.CurrentClip.BaseClipLength;
    this.m_lightIntensityAction.Owner = this.Owner;
    this.m_lightIntensityAction.IsKeptAction = true;
    this.m_lightIntensityAction.OnEnter();
    this.m_cachedLightIntensity = this.m_lightIntensityAction.specifyLights.Length <= 0 ? 0.0f : this.m_lightIntensityAction.specifyLights[0].uLightIntensity;
    this.m_lightIntensityAction.OnUpdate();
  }

  private bool IsPlaying(string anim)
  {
    if ((bool) (Object) this.m_talkDoer.aiAnimator)
      return this.m_talkDoer.aiAnimator.IsPlaying(anim);
    return (bool) (Object) this.m_talkDoer.spriteAnimator && this.m_talkDoer.spriteAnimator.IsPlaying(anim);
  }

  private void FinishAnim()
  {
    if (this.m_state != Teleport.State.TeleportOut)
      return;
    SetNpcVisibility.SetVisible(this.m_talkDoer, false);
    this.m_talkDoer.ShowOutlines = false;
  }

  private GameObject SpawnVfx(GameObject vfxPrefab, GameObject anchor)
  {
    if (!(bool) (Object) vfxPrefab)
      return (GameObject) null;
    GameObject gameObject = Object.Instantiate<GameObject>(vfxPrefab, (Vector3) this.m_talkDoer.specRigidbody.GetUnitCenter(ColliderType.HitBox), Quaternion.identity);
    if ((bool) (Object) anchor)
    {
      gameObject.transform.parent = anchor.transform;
      gameObject.transform.localPosition = Vector3.zero;
    }
    tk2dSprite component = gameObject.GetComponent<tk2dSprite>();
    if ((bool) (Object) component && component.IsPerpendicular == this.m_talkDoer.sprite.IsPerpendicular)
    {
      this.m_talkDoer.sprite.AttachRenderer((tk2dBaseSprite) component);
      component.HeightOffGround = 0.05f;
      component.UpdateZDepth();
    }
    return gameObject;
  }

  public enum Mode
  {
    Out,
    In,
    Both,
  }

  public enum State
  {
    TeleportOut,
    MidStep,
    TeleportIn,
  }

  public enum Timing
  {
    Simultaneous,
    VfxThenAnimation,
    AnimationThenVfx,
    Delays,
  }
}
