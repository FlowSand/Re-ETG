using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class FinalIntroSequenceManager : MonoBehaviour
  {
    public tk2dSpriteAnimator PT1_DodgeRoll_Guy;
    public tk2dSpriteAnimator PT1_DodgeRoll_Logo;
    public tk2dSpriteAnimator PT2_Devolver_Logo;
    public Material FadeMaterial;
    public GameObject QuickStartObject;
    public tk2dTextMesh QuickStartKeyboard;
    public tk2dSprite QuickStartController;
    public tk2dTextMesh IntroTextMesh;
    public FinalIntroSequenceCard[] IntroCards;
    public bool IsDoingIntro;
    private bool m_inFoyer;
    private bool m_isDoingQuickStart;
    private bool m_skipCycle;
    private float customTextFadeInTime = -1f;
    private float customTextFadeOutTime = -1f;
    private string[] introKeys = new string[11]
    {
      "#INTRO_VIDEO_01",
      "#INTRO_VIDEO_02a",
      "#INTRO_VIDEO_02b",
      "#INTRO_VIDEO_03",
      "#INTRO_VIDEO_04a",
      "#INTRO_VIDEO_04b",
      "#INTRO_VIDEO_05",
      "#INTRO_VIDEO_06a",
      "#INTRO_VIDEO_06b",
      "#INTRO_VIDEO_07",
      "#INTRO_VIDEO_08"
    };
    private string m_cachedLastFirstString;
    private string[] m_lastAssignedStrings;
    public float FirstTextFadeInTime = 3f;
    public float FirstTextHoldTime = 7f;
    public float LastTextHoldTime = 7f;
    public float LastTextFadeOutTime = 3f;
    public float LastTextSecondStringTriggerTime = 5f;
    private bool m_skipLegend;
    private Vector3 m_currentIntroTextMeshLocalPosition;

    private void Awake()
    {
      if (!Foyer.DoIntroSequence)
        return;
      GameManager.Instance.IsSelectingCharacter = true;
      this.m_inFoyer = true;
      if (!this.m_inFoyer)
        GameManager.PreventGameManagerExistence = true;
      if (GameManager.Options == null)
        GameOptions.Load();
      Pixelator.DEBUG_LogSystemRenderingData();
    }

    public void TriggerSequence()
    {
      if (!Foyer.DoIntroSequence)
        return;
      GameManager.Instance.StartCoroutine(this.CoreSequence());
      this.StartCoroutine(this.HandleBackgroundSkipChecks());
    }

    private void OnDestroy() => GameManager.PreventGameManagerExistence = false;

    private bool QuickStartAvailable()
    {
      return GameStatsManager.Instance != null && (double) GameStatsManager.Instance.GetPlayerStatValue(TrackedStats.NUMBER_ATTEMPTS) >= 1.0;
    }

    [DebuggerHidden]
    private IEnumerator MoveQuickstartOffscreen()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new FinalIntroSequenceManager__MoveQuickstartOffscreenc__Iterator0()
      {
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator MoveQuickstartOnScreen()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new FinalIntroSequenceManager__MoveQuickstartOnScreenc__Iterator1()
      {
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator Start()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new FinalIntroSequenceManager__Startc__Iterator2()
      {
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator HandleBackgroundSkipChecks()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new FinalIntroSequenceManager__HandleBackgroundSkipChecksc__Iterator3()
      {
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator SkippableWait(float duration)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new FinalIntroSequenceManager__SkippableWaitc__Iterator4()
      {
        duration = duration,
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator CoreSequence()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new FinalIntroSequenceManager__CoreSequencec__Iterator5()
      {
        _this = this
      };
    }

    private void SetIntroString(bool fadePrevious, bool resetToCenter, params string[] keys)
    {
      this.StartCoroutine(this.SetIntroStringCR(fadePrevious, resetToCenter, -1f, keys));
    }

    private void SetIntroString(
      bool fadePrevious,
      bool resetToCenter,
      float customDura,
      params string[] keys)
    {
      this.StartCoroutine(this.SetIntroStringCR(fadePrevious, resetToCenter, customDura, keys));
    }

    [DebuggerHidden]
    private IEnumerator SetIntroStringCR(
      bool fadePrevious,
      bool resetToCenter,
      float customDura,
      params string[] keys)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new FinalIntroSequenceManager__SetIntroStringCRc__Iterator6()
      {
        customDura = customDura,
        fadePrevious = fadePrevious,
        resetToCenter = resetToCenter,
        keys = keys,
        _this = this
      };
    }

    private void UpdateText(
      float totalElapsed,
      float cardElapsed,
      int currentCardIndex,
      ref int currentIndex)
    {
      if (currentCardIndex < 0 || currentCardIndex >= this.IntroCards.Length)
        return;
      string[] targetKeys = this.IntroCards[currentCardIndex].GetTargetKeys(cardElapsed);
      bool flag = false;
      if (this.m_lastAssignedStrings == null || targetKeys.Length != this.m_lastAssignedStrings.Length)
      {
        flag = true;
      }
      else
      {
        for (int index = 0; index < targetKeys.Length; ++index)
        {
          if (targetKeys[index] != this.m_lastAssignedStrings[index])
            flag = true;
        }
      }
      if (!flag)
        return;
      this.customTextFadeInTime = -1f;
      this.customTextFadeOutTime = -1f;
      bool fadePrevious = this.m_cachedLastFirstString != targetKeys[0];
      this.m_cachedLastFirstString = targetKeys[0];
      this.m_lastAssignedStrings = targetKeys;
      this.SetIntroString(fadePrevious, false, targetKeys);
    }

    [DebuggerHidden]
    private IEnumerator LegendSkippableWait(float dura)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new FinalIntroSequenceManager__LegendSkippableWaitc__Iterator7()
      {
        dura = dura,
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator ContinueMovingPreviousCard(FinalIntroSequenceCard previousCard)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new FinalIntroSequenceManager__ContinueMovingPreviousCardc__Iterator8()
      {
        previousCard = previousCard
      };
    }

    private void UpdateSkipLegend()
    {
      if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E))
        this.m_skipLegend = true;
      if (!BraveInput.PlayerlessInstance.ActiveActions.IntroSkipActionPressed())
        return;
      this.m_skipLegend = true;
    }

    private void HandleOffsetUpdate()
    {
      this.IntroTextMesh.transform.parent = (Transform) null;
      this.IntroTextMesh.transform.position = GameManager.Instance.MainCameraController.transform.position + this.m_currentIntroTextMeshLocalPosition;
    }

    [DebuggerHidden]
    private IEnumerator LegendCore()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new FinalIntroSequenceManager__LegendCorec__Iterator9()
      {
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator FadeToBlack(float duration, bool startAtCurrent = false, bool force = false)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new FinalIntroSequenceManager__FadeToBlackc__IteratorA()
      {
        startAtCurrent = startAtCurrent,
        duration = duration,
        force = force,
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator HandleDevolverLogo()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new FinalIntroSequenceManager__HandleDevolverLogoc__IteratorB()
      {
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator HandleDodgeRollLogo()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new FinalIntroSequenceManager__HandleDodgeRollLogoc__IteratorC()
      {
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator DoQuickStart()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new FinalIntroSequenceManager__DoQuickStartc__IteratorD()
      {
        _this = this
      };
    }
  }

