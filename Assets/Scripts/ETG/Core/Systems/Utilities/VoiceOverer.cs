// Decompiled with JetBrains decompiler
// Type: VoiceOverer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class VoiceOverer : MonoBehaviour
    {
      public string[] IntroEvents;
      public string[] IntroKeys;
      public string[] DefeatedPlayerEvents;
      public string[] DefeatedPlayerKeys;
      public string[] KilledByPlayerEvents;
      public string[] KilledByPlayerKeys;
      public string[] DamageBarkEvents;
      public string[] AttackBarkEvents;
      private float m_sinceAnyBark = 100f;
      private float m_sinceBark = 100f;
      private int m_lastBark = -1;
      private bool m_lastFrameHadContinueAttack;
      private float m_sinceAttackBark = 100f;
      private int m_lastAttackBark = -1;
      private BehaviorSpeculator m_bs;

      private void Start()
      {
        this.m_bs = this.GetComponent<BehaviorSpeculator>();
        this.GetComponent<HealthHaver>().OnDamaged += new HealthHaver.OnDamagedEvent(this.HandleDamaged);
        for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
          GameManager.Instance.AllPlayers[index].OnRealPlayerDeath += new Action<PlayerController>(this.HandlePlayerGameOver);
      }

      private void HandlePlayerGameOver(PlayerController player)
      {
        AIActor component = this.GetComponent<AIActor>();
        if (component.HasBeenEngaged && player.CurrentRoom == component.ParentRoom)
          this.StartCoroutine(this.HandlePlayerLostVO());
        this.DisconnectCallback();
      }

      private void DisconnectCallback()
      {
        for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
          GameManager.Instance.AllPlayers[index].OnRealPlayerDeath -= new Action<PlayerController>(this.HandlePlayerGameOver);
      }

      private void OnDestroy() => this.DisconnectCallback();

      private void Update()
      {
        this.m_sinceBark += BraveTime.DeltaTime;
        this.m_sinceAttackBark += BraveTime.DeltaTime;
        this.m_sinceAnyBark += BraveTime.DeltaTime;
        bool flag = this.m_bs.ActiveContinuousAttackBehavior != null;
        if (flag && !this.m_lastFrameHadContinueAttack)
          this.HandleAttackBark();
        this.m_lastFrameHadContinueAttack = flag;
      }

      private void HandleDamaged(
        float resultValue,
        float maxValue,
        CoreDamageTypes damageTypes,
        DamageCategory damageCategory,
        Vector2 damageDirection)
      {
        if ((double) this.m_sinceBark <= 10.0 || (double) this.m_sinceAnyBark <= 3.0 || (double) resultValue <= 0.0)
          return;
        this.HandleDamageBark();
        this.m_sinceBark = 0.0f;
        this.m_sinceAnyBark = 0.0f;
      }

      [DebuggerHidden]
      public IEnumerator HandleIntroVO()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new VoiceOverer.<HandleIntroVO>c__Iterator0()
        {
          _this = this
        };
      }

      [DebuggerHidden]
      public IEnumerator HandlePlayerLostVO()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new VoiceOverer.<HandlePlayerLostVO>c__Iterator1()
        {
          _this = this
        };
      }

      [DebuggerHidden]
      public IEnumerator HandlePlayerWonVO(float maxDuration)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new VoiceOverer.<HandlePlayerWonVO>c__Iterator2()
        {
          maxDuration = maxDuration,
          _this = this
        };
      }

      public void HandleDamageBark()
      {
        int index = UnityEngine.Random.Range(0, this.DamageBarkEvents.Length);
        if (index == this.m_lastBark && this.m_lastBark == 2)
          index = 0;
        this.m_lastBark = index;
        int num = (int) AkSoundEngine.PostEvent(this.DamageBarkEvents[index], this.gameObject);
      }

      public void HandleAttackBark()
      {
        if ((double) this.m_sinceAttackBark <= 10.0 || (double) this.m_sinceAnyBark <= 3.0)
          return;
        this.m_sinceAttackBark = 0.0f;
        this.m_sinceAnyBark = 0.0f;
        int index = UnityEngine.Random.Range(0, this.AttackBarkEvents.Length);
        while (index == this.m_lastBark)
          index = UnityEngine.Random.Range(0, this.AttackBarkEvents.Length);
        int num = (int) AkSoundEngine.PostEvent(this.AttackBarkEvents[index], this.gameObject);
      }

      [DebuggerHidden]
      public IEnumerator HandleTalk(string stringKey, float maxDuration = -1f)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new VoiceOverer.<HandleTalk>c__Iterator3()
        {
          stringKey = stringKey,
          maxDuration = maxDuration,
          _this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator TalkRaw(string plaintext, float maxDuration = -1f)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new VoiceOverer.<TalkRaw>c__Iterator4()
        {
          plaintext = plaintext,
          maxDuration = maxDuration,
          _this = this
        };
      }
    }

}
