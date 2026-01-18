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
      return (IEnumerator) new ConvictPastController__Startc__Iterator0()
      {
        _this = this
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
      return (IEnumerator) new ConvictPastController__HandleBossStartc__Iterator1()
      {
        initialDelay = initialDelay,
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator DoPath(TalkDoerLite source, bool doDestroy)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new ConvictPastController__DoPathc__Iterator2()
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
      return (IEnumerator) new ConvictPastController__EnableHeadlightsc__Iterator3()
      {
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator HandleBossKilled(Transform bossTransform)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new ConvictPastController__HandleBossKilledc__Iterator4()
      {
        bossTransform = bossTransform,
        _this = this
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
      return (IEnumerator) new ConvictPastController__DoAmbientTalkc__Iterator5()
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

