// Decompiled with JetBrains decompiler
// Type: ConvictPastController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable
public class ConvictPastController : MonoBehaviour
{
  public bool InstantBossFight;
  public TalkDoerLite InitialTalkDoer;
  public TalkDoerLite BaldoTalkDoer;
  public TalkDoerLite BaldoBossTalkDoer;
  public NightclubCrowdController crowdController;
  public TalkDoerLite[] HmonSoldiers;
  public tk2dSpriteAnimator DeskAnimator;
  public GameObject DeskAnimatorPoof;
  public TalkDoerLite PhantomEndTalkDoer;
  public tk2dSpriteAnimator Car;
  public Renderer CarHeadlightsRenderer;
  public SpeculativeRigidbody ExitDoorRigidbody;
  public SpeculativeRigidbody MainDoorBlocker;
  private bool m_hasStartedBossSequence;
  public static float FREEZE_FRAME_DURATION = 2f;

  [DebuggerHidden]
  private IEnumerator Start()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new ConvictPastController.\u003CStart\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  private void HandlePrematurePanic()
  {
    if (this.m_hasStartedBossSequence)
      return;
    this.StartCoroutine(this.HandleBossStart(3f));
  }

  [DebuggerHidden]
  private IEnumerator HandleBossStart(float initialDelay = 0.0f)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new ConvictPastController.\u003CHandleBossStart\u003Ec__Iterator1()
    {
      initialDelay = initialDelay,
      \u0024this = this
    };
  }

  [DebuggerHidden]
  private IEnumerator DoPath(TalkDoerLite source, bool doDestroy)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new ConvictPastController.\u003CDoPath\u003Ec__Iterator2()
    {
      source = source,
      doDestroy = doDestroy
    };
  }

  public void OnBossKilled(Transform bossTransform)
  {
    this.StartCoroutine(this.HandleBossKilled(bossTransform));
  }

  [DebuggerHidden]
  private IEnumerator EnableHeadlights()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new ConvictPastController.\u003CEnableHeadlights\u003Ec__Iterator3()
    {
      \u0024this = this
    };
  }

  [DebuggerHidden]
  private IEnumerator HandleBossKilled(Transform bossTransform)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new ConvictPastController.\u003CHandleBossKilled\u003Ec__Iterator4()
    {
      bossTransform = bossTransform,
      \u0024this = this
    };
  }

  [DebuggerHidden]
  public IEnumerator DoAmbientTalk(
    Transform baseTransform,
    Vector3 offset,
    string stringKey,
    float duration,
    bool isThoughtBubble)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new ConvictPastController.\u003CDoAmbientTalk\u003Ec__Iterator5()
    {
      isThoughtBubble = isThoughtBubble,
      baseTransform = baseTransform,
      offset = offset,
      duration = duration,
      stringKey = stringKey
    };
  }

  private void Update()
  {
  }
}
