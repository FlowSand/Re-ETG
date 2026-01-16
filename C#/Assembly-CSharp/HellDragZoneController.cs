// Decompiled with JetBrains decompiler
// Type: HellDragZoneController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using HutongGames.PlayMaker;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable
public class HellDragZoneController : BraveBehaviour
{
  public GameObject HoleObject;
  public GameObject HellDragVFX;
  public GameObject CryoButtonPrefab;
  private Material HoleMaterial;
  private bool m_holeIsActive;
  public string cryoAnimatorName;
  public string cryoArriveAnimation;
  public string cyroDepartAnimation;
  private tk2dSpriteAnimator cryoAnimator;
  private FsmBool m_cryoBool;
  private FsmBool m_normalBool;

  private void Start()
  {
    this.HoleObject.SetActive(false);
    this.HoleMaterial = this.HoleObject.GetComponent<MeshRenderer>().material;
    bool flag = GameManager.Instance.PrimaryPlayer.characterIdentity == PlayableCharacters.Eevee && GameStatsManager.Instance.AllCorePastsBeaten() && !GameStatsManager.Instance.GetFlag(GungeonFlags.GUNSLINGER_UNLOCKED);
    if ((GameManager.Instance.PrimaryPlayer.characterIdentity != PlayableCharacters.Gunslinger || GameStatsManager.Instance.GetFlag(GungeonFlags.GUNSLINGER_UNLOCKED)) && (!GameManager.Instance.PrimaryPlayer.CharacterUsesRandomGuns || GameStatsManager.Instance.GetFlag(GungeonFlags.BOSSKILLED_LICH)) && GameStatsManager.Instance.AllCorePastsBeaten())
    {
      if ((double) GameStatsManager.Instance.GetPlayerStatValue(TrackedStats.TIMES_REACHED_BULLET_HELL) == 0.0 || flag)
      {
        this.specRigidbody.OnEnterTrigger += new SpeculativeRigidbody.OnTriggerDelegate(this.ProcessTriggerEntered);
      }
      else
      {
        this.HoleObject.SetActive(true);
        this.SetHoleSize(0.25f);
        this.m_holeIsActive = true;
        this.specRigidbody.OnTriggerCollision += new SpeculativeRigidbody.OnTriggerDelegate(this.ProcessTriggerFrame);
      }
    }
    if (!this.m_holeIsActive || (double) GameStatsManager.Instance.GetPlayerStatValue(TrackedStats.TIMES_REACHED_BULLET_HELL) < 1.0)
      return;
    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.CryoButtonPrefab);
    gameObject.transform.parent = this.transform;
    gameObject.transform.localPosition = new Vector3(-9f / 16f, -0.875f, 0.0f);
    gameObject.GetComponent<SpeculativeRigidbody>().Reinitialize();
    TalkDoerLite componentInChildren = gameObject.GetComponentInChildren<TalkDoerLite>();
    componentInChildren.GetAbsoluteParentRoom().RegisterInteractable((IPlayerInteractable) componentInChildren);
    componentInChildren.OnGenericFSMActionA += new System.Action(this.SwitchToCryoElevator);
    componentInChildren.OnGenericFSMActionB += new System.Action(this.RescindCryoElevator);
    this.m_cryoBool = componentInChildren.playmakerFsm.FsmVariables.GetFsmBool("IS_CRYO");
    this.m_normalBool = componentInChildren.playmakerFsm.FsmVariables.GetFsmBool("IS_NORMAL");
    this.m_cryoBool.Value = false;
    this.m_normalBool.Value = true;
    Transform transform = gameObject.transform.Find(this.cryoAnimatorName);
    if (!(bool) (UnityEngine.Object) transform)
      return;
    this.cryoAnimator = transform.GetComponent<tk2dSpriteAnimator>();
  }

  private void RescindCryoElevator()
  {
    this.m_cryoBool.Value = false;
    this.m_normalBool.Value = true;
    if (!(bool) (UnityEngine.Object) this.cryoAnimator || string.IsNullOrEmpty(this.cyroDepartAnimation))
      return;
    this.cryoAnimator.Play(this.cyroDepartAnimation);
  }

  private void SwitchToCryoElevator()
  {
    this.m_cryoBool.Value = true;
    this.m_normalBool.Value = false;
    if (!(bool) (UnityEngine.Object) this.cryoAnimator || string.IsNullOrEmpty(this.cryoArriveAnimation))
      return;
    this.cryoAnimator.Play(this.cryoArriveAnimation);
  }

  private void ProcessTriggerFrame(
    SpeculativeRigidbody specRigidbody,
    SpeculativeRigidbody sourceSpecRigidbody,
    CollisionData collisionData)
  {
    if (!this.m_holeIsActive)
      return;
    PlayerController component = specRigidbody.GetComponent<PlayerController>();
    if (!(bool) (UnityEngine.Object) component || (double) Vector2.Distance(component.CenterPosition, this.HoleObject.transform.PositionVector2()) >= 2.5)
      return;
    this.GrabPlayer(component);
    this.m_holeIsActive = false;
  }

  private void SetHoleSize(float size) => this.HoleMaterial.SetFloat("_UVDistCutoff", size);

  [DebuggerHidden]
  private IEnumerator LerpHoleSize(
    float startSize,
    float endSize,
    float duration,
    PlayerController targetPlayer)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new HellDragZoneController.\u003CLerpHoleSize\u003Ec__Iterator0()
    {
      duration = duration,
      targetPlayer = targetPlayer,
      startSize = startSize,
      endSize = endSize,
      \u0024this = this
    };
  }

  [DebuggerHidden]
  private IEnumerator HandleGrabbyGrab(PlayerController grabbedPlayer)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new HellDragZoneController.\u003CHandleGrabbyGrab\u003Ec__Iterator1()
    {
      grabbedPlayer = grabbedPlayer,
      \u0024this = this
    };
  }

  private void GrabPlayer(PlayerController enteredPlayer)
  {
    enteredPlayer.CurrentInputState = PlayerInputState.NoInput;
    tk2dBaseSprite component = enteredPlayer.PlayEffectOnActor(this.HellDragVFX, Vector3.zero).GetComponent<tk2dBaseSprite>();
    component.UpdateZDepth();
    component.attachParent = (tk2dBaseSprite) null;
    component.IsPerpendicular = false;
    component.HeightOffGround = 1f;
    component.UpdateZDepth();
    component.transform.position = component.transform.position.WithX(component.transform.position.x + 0.25f);
    component.transform.position = component.transform.position.WithY((float) enteredPlayer.CurrentRoom.area.basePosition.y + 55f);
    component.usesOverrideMaterial = true;
    component.renderer.material.shader = ShaderCache.Acquire("Brave/Effects/StencilMasked");
    this.StartCoroutine(this.HandleGrabbyGrab(enteredPlayer));
  }

  private void ProcessTriggerEntered(
    SpeculativeRigidbody specRigidbody,
    SpeculativeRigidbody sourceSpecRigidbody,
    CollisionData collisionData)
  {
    UnityEngine.Debug.Log((object) "Hell Hole entered!");
    PlayerController component = specRigidbody.GetComponent<PlayerController>();
    this.HoleObject.SetActive(true);
    if (!(bool) (UnityEngine.Object) component)
      return;
    if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
      this.specRigidbody.OnEnterTrigger -= new SpeculativeRigidbody.OnTriggerDelegate(this.ProcessTriggerEntered);
    this.GrabPlayer(component);
    this.StartCoroutine(this.LerpHoleSize(0.0f, 0.15f, 0.3f, component));
  }
}
