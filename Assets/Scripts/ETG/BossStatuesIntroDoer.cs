// Decompiled with JetBrains decompiler
// Type: BossStatuesIntroDoer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable
[RequireComponent(typeof (GenericIntroDoer))]
public class BossStatuesIntroDoer : SpecificIntroDoer
{
  public float ghostDelay;
  public float[] ghostMidDelay;
  public List<tk2dSpriteAnimator> ghostAnimators;
  public VFXPool eyeVfx;
  public float dustDelay;
  public List<tk2dSpriteAnimator> dustAnimators;
  public float floatDelay = 0.3f;
  public float floatTime = 2f;
  public float hangTime = 0.5f;
  public float slamTime = 1f;
  public ScreenShakeSettings floatScreenShake;
  public ScreenShakeSettings slamScreenShake;
  private BossStatuesIntroDoer.State m_state;
  private BossStatuesController m_statuesController;
  private List<BossStatueController> m_allStatues;
  private List<tk2dSpriteAnimator> m_animators;
  private Vector3[] m_startingPositions;
  private Vector3[] m_startingShadowPositions;

  private void Start()
  {
    this.m_statuesController = this.GetComponent<BossStatuesController>();
    for (int index = 0; index < this.m_statuesController.allStatues.Count; ++index)
    {
      BossStatueController allStatue = this.m_statuesController.allStatues[index];
      allStatue.specRigidbody.CollideWithOthers = false;
      allStatue.aiActor.IsGone = true;
    }
  }

  private void Update()
  {
  }

  protected override void OnDestroy() => base.OnDestroy();

  public Vector2? BossCenter
  {
    get
    {
      return new Vector2?(this.transform.position.XY() + new Vector2((float) this.dungeonPlaceable.placeableWidth / 2f, (float) this.dungeonPlaceable.placeableHeight / 2f) + new Vector2(0.0f, 2f));
    }
  }

  public override void StartIntro(List<tk2dSpriteAnimator> animators)
  {
    this.m_animators = animators;
    for (int index = 0; index < this.dustAnimators.Count; ++index)
      this.m_animators.Add(this.dustAnimators[index]);
    for (int index = 0; index < this.ghostAnimators.Count; ++index)
    {
      this.m_animators.Add(this.ghostAnimators[index]);
      this.ghostAnimators[index].renderer.enabled = false;
    }
    this.m_allStatues = this.m_statuesController.allStatues;
    this.m_startingPositions = new Vector3[this.m_allStatues.Count];
    this.m_startingShadowPositions = new Vector3[this.m_allStatues.Count];
    for (int index = 0; index < this.m_allStatues.Count; ++index)
    {
      this.m_animators.Add(this.m_allStatues[index].landVfx);
      this.m_startingPositions[index] = this.m_allStatues[index].transform.position;
      this.m_startingShadowPositions[index] = this.m_allStatues[index].shadowSprite.transform.position;
    }
    this.StartCoroutine(this.PlayIntro());
  }

  public override void EndIntro()
  {
    this.StopAllCoroutines();
    GameUIRoot.Instance.bossController.SetBossName(StringTableManager.GetEnemiesString(this.GetComponent<GenericIntroDoer>().portraitSlideSettings.bossNameString));
    for (int index = 0; index < this.m_statuesController.allStatues.Count; ++index)
    {
      BossStatueController allStatue = this.m_statuesController.allStatues[index];
      allStatue.aiActor.SkipOnEngaged();
      GameUIRoot.Instance.bossController.RegisterBossHealthHaver(allStatue.healthHaver);
      allStatue.specRigidbody.CollideWithOthers = true;
      allStatue.aiActor.IsGone = false;
      allStatue.aiActor.State = AIActor.ActorState.Normal;
    }
    for (int index = 0; index < this.ghostAnimators.Count; ++index)
    {
      if ((Object) this.ghostAnimators[index] != (Object) null)
        this.ghostAnimators[index].renderer.enabled = false;
    }
    for (int index = 0; index < this.dustAnimators.Count; ++index)
      this.dustAnimators[index].renderer.enabled = false;
    if (this.m_allStatues != null)
    {
      for (int index = 0; index < this.m_allStatues.Count; ++index)
      {
        this.m_allStatues[index].transform.position = this.m_startingPositions[index];
        this.m_allStatues[index].shadowSprite.transform.position = this.m_startingShadowPositions[index];
      }
    }
    this.eyeVfx.DestroyAll();
  }

  public override bool IsIntroFinished => this.m_state == BossStatuesIntroDoer.State.Finished;

  public override void OnCameraIntro()
  {
    for (int index = 0; index < this.ghostAnimators.Count; ++index)
      this.ghostAnimators[index].renderer.enabled = false;
  }

  public override void OnBossCard() => this.eyeVfx.DestroyAll();

  public override void OnCleanup() => this.behaviorSpeculator.enabled = true;

  [DebuggerHidden]
  private IEnumerator PlayIntro()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new BossStatuesIntroDoer.\u003CPlayIntro\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  [DebuggerHidden]
  private IEnumerator WaitForSecondsInvariant(float time)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new BossStatuesIntroDoer.\u003CWaitForSecondsInvariant\u003Ec__Iterator1()
    {
      time = time
    };
  }

  private enum State
  {
    Idle,
    Playing,
    Finished,
  }
}
