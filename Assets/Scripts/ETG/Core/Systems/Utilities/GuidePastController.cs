// Decompiled with JetBrains decompiler
// Type: GuidePastController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class GuidePastController : MonoBehaviour
  {
    public PlayerCommentInteractable TargetInteractable;
    public tk2dSprite ArtifactSprite;
    public TalkDoerLite DrWolfTalkDoer;
    public SpeculativeRigidbody DrWolfEnemyRigidbody;
    public TalkDoerLite PhantomEndTalkDoer;
    public tk2dSprite SpinnyGreenMachinePart;
    private PlayerController m_guide;
    private PlayerController m_coop;
    private AIActor m_dog;
    private Transform m_transform;
    private List<GameObject> m_fakeBullets = new List<GameObject>();
    private bool m_hasTriggeredBoss;
    private bool m_trapActive;
    private bool m_forceSkip;
    private float m_initialTriggerHeight = 8f;
    private bool m_hasTriggeredInitial;
    private float m_antechamberTriggerHeight = 29f;
    private bool m_hasTriggeredAntechamber;

    [DebuggerHidden]
    private IEnumerator Start()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new GuidePastController__Startc__Iterator0()
      {
        _this = this
      };
    }

    private void SetupCutscene()
    {
      PastCameraUtility.LockConversation(this.DrWolfTalkDoer.speakPoint.transform.position.XY() + new Vector2(0.0f, 15.5f));
      this.DrWolfTalkDoer.gameObject.SetActive(true);
    }

    private void HandleBossCutscene()
    {
      PlayerCommentInteractable[] objectsOfType = Object.FindObjectsOfType<PlayerCommentInteractable>();
      if (objectsOfType != null)
      {
        for (int index = 0; index < objectsOfType.Length; ++index)
          objectsOfType[index].ForceDisable();
      }
      this.StartCoroutine(this.BossCutscene_CR());
    }

    [DebuggerHidden]
    private IEnumerator BossCutscene_CR()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new GuidePastController__BossCutscene_CRc__Iterator1()
      {
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator HandleDogGoingNuts()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new GuidePastController__HandleDogGoingNutsc__Iterator2()
      {
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator HandleBulletTrap()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new GuidePastController__HandleBulletTrapc__Iterator3()
      {
        _this = this
      };
    }

    private HealthHaver GetBoss()
    {
      List<HealthHaver> allHealthHavers = StaticReferenceManager.AllHealthHavers;
      for (int index = 0; index < allHealthHavers.Count; ++index)
      {
        if (allHealthHavers[index].IsBoss)
        {
          GenericIntroDoer component = allHealthHavers[index].GetComponent<GenericIntroDoer>();
          if ((bool) (Object) component && component.triggerType == GenericIntroDoer.TriggerType.BossTriggerZone)
            return allHealthHavers[index];
        }
      }
      return (HealthHaver) null;
    }

    private void TriggerBoss()
    {
      if (this.m_hasTriggeredBoss)
        return;
      this.DrWolfEnemyRigidbody.enabled = true;
      List<HealthHaver> allHealthHavers = StaticReferenceManager.AllHealthHavers;
      for (int index = 0; index < allHealthHavers.Count; ++index)
      {
        if (allHealthHavers[index].IsBoss)
        {
          GenericIntroDoer component = allHealthHavers[index].GetComponent<GenericIntroDoer>();
          if ((bool) (Object) component && component.triggerType == GenericIntroDoer.TriggerType.BossTriggerZone)
          {
            component.gameObject.SetActive(true);
            component.GetComponent<ObjectVisibilityManager>().ChangeToVisibility(RoomHandler.VisibilityStatus.CURRENT);
            if (!SpriteOutlineManager.HasOutline(component.aiAnimator.sprite))
              SpriteOutlineManager.AddOutlineToSprite(component.aiAnimator.sprite, Color.black, 0.1f);
            component.aiAnimator.renderer.enabled = false;
            SpriteOutlineManager.ToggleOutlineRenderers(component.aiAnimator.sprite, false);
            component.aiAnimator.ChildAnimator.renderer.enabled = false;
            SpriteOutlineManager.ToggleOutlineRenderers(component.aiAnimator.ChildAnimator.sprite, false);
            component.TriggerSequence(GameManager.Instance.PrimaryPlayer);
            this.m_hasTriggeredBoss = true;
            break;
          }
        }
      }
    }

    public void OnBossKilled() => this.StartCoroutine(this.HandleBossKilled());

    [DebuggerHidden]
    private IEnumerator HandleBossKilled()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new GuidePastController__HandleBossKilledc__Iterator4()
      {
        _this = this
      };
    }

    public void MakeGuideTalkAmbient(string stringKey, float duration = 3f, bool isThoughtBubble = false)
    {
      this.DoAmbientTalk(this.m_guide.transform, new Vector3(0.75f, 1.5f, 0.0f), stringKey, duration, isThoughtBubble, GameManager.Instance.PrimaryPlayer.characterAudioSpeechTag);
    }

    public void MakeDogTalk(string stringKey, float duration = 3f)
    {
      this.DoAmbientTalk(this.m_dog.transform, new Vector3(0.25f, 1f, 0.0f), stringKey, duration, audioTag: string.Empty);
    }

    public void DoAmbientTalk(
      Transform baseTransform,
      Vector3 offset,
      string stringKey,
      float duration,
      bool isThoughtBubble = false,
      string audioTag = "")
    {
      if (isThoughtBubble)
        TextBoxManager.ShowThoughtBubble(baseTransform.position + offset, baseTransform, -1f, StringTableManager.GetString(stringKey), false, overrideAudioTag: audioTag);
      else
        TextBoxManager.ShowTextBox(baseTransform.position + offset, baseTransform, -1f, StringTableManager.GetString(stringKey), audioTag, false);
      this.StartCoroutine(this.HandleManualTalkDuration(baseTransform, duration));
    }

    [DebuggerHidden]
    private IEnumerator HandleManualTalkDuration(Transform source, float duration)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new GuidePastController__HandleManualTalkDurationc__Iterator5()
      {
        duration = duration,
        source = source,
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator HandleIntroConversation()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new GuidePastController__HandleIntroConversationc__Iterator6()
      {
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator HandleAntechamberConversation()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new GuidePastController__HandleAntechamberConversationc__Iterator7()
      {
        _this = this
      };
    }

    private void Update()
    {
      this.m_forceSkip = false;
      bool flag = BraveInput.GetInstanceForPlayer(0).WasAdvanceDialoguePressed();
      if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
        flag |= BraveInput.GetInstanceForPlayer(1).WasAdvanceDialoguePressed();
      if (flag)
        this.m_forceSkip = true;
      float num = !GameManager.IsBossIntro ? BraveTime.DeltaTime : GameManager.INVARIANT_DELTA_TIME;
      if ((bool) (Object) this.SpinnyGreenMachinePart && (double) num > 0.0)
        GlobalSparksDoer.DoRandomParticleBurst(Mathf.CeilToInt(15f * num), (Vector3) (this.SpinnyGreenMachinePart.WorldCenter + new Vector2(-0.5f, -0.5f)), (Vector3) (this.SpinnyGreenMachinePart.WorldCenter + new Vector2(0.5f, 0.5f)), Vector3.up * 3f, 30f, 0.5f, startColor: new Color?(Color.green));
      if (this.ArtifactSprite.renderer.enabled)
        GlobalSparksDoer.DoRandomParticleBurst(Mathf.CeilToInt(Mathf.Max(1f, 80f * num)), this.ArtifactSprite.WorldBottomLeft.ToVector3ZisY(), this.ArtifactSprite.WorldTopRight.ToVector3ZisY(), Vector3.up, 180f, 0.5f, systemType: GlobalSparksDoer.SparksType.BLACK_PHANTOM_SMOKE);
      if ((double) UnityEngine.Time.timeSinceLevelLoad <= 0.5)
        return;
      if (!this.m_hasTriggeredInitial)
      {
        if ((double) this.m_guide.transform.position.y <= (double) this.m_initialTriggerHeight + (double) this.m_transform.position.y)
          return;
        this.m_hasTriggeredInitial = true;
        this.StartCoroutine(this.HandleIntroConversation());
      }
      else
      {
        if (this.m_hasTriggeredAntechamber || (double) this.m_guide.transform.position.y <= (double) this.m_antechamberTriggerHeight + (double) this.m_transform.position.y)
          return;
        this.m_hasTriggeredAntechamber = true;
        this.StartCoroutine(this.HandleAntechamberConversation());
      }
    }
  }

