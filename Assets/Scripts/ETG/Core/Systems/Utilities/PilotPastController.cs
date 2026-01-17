// Decompiled with JetBrains decompiler
// Type: PilotPastController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class PilotPastController : MonoBehaviour
    {
      public bool InstantBossFight;
      public TalkDoerLite FriendTalker;
      public TalkDoerLite HegemonyShip;
      public tk2dSprite[] AdditionalHegemonyShips;
      public GameObject FloatingCrap;
      public tk2dSprite TheRock;
      public Renderer Quad;
      public Vector2 BackgroundScrollSpeed;
      private PlayerController m_pilot;
      private PlayerController m_coop;
      private bool m_hasTriggeredBoss;
      private Vector2 m_backgroundOffset;
      private int m_scrollPositionXId = -1;
      private int m_scrollPositionYId = -1;

      [DebuggerHidden]
      private IEnumerator Start()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new PilotPastController__Startc__Iterator0()
        {
          _this = this
        };
      }

      public void Update()
      {
        Material material = this.Quad.material;
        if (this.m_scrollPositionXId < 0)
          this.m_scrollPositionXId = Shader.PropertyToID("_PositionX");
        if (this.m_scrollPositionYId < 0)
          this.m_scrollPositionYId = Shader.PropertyToID("_PositionY");
        this.m_backgroundOffset += this.BackgroundScrollSpeed * BraveTime.DeltaTime;
        material.SetFloat(this.m_scrollPositionXId, this.m_backgroundOffset.x);
        material.SetFloat(this.m_scrollPositionYId, this.m_backgroundOffset.y);
      }

      public void ToggleFriendAndJunk(bool state)
      {
        this.StartCoroutine(this.HandleFriendAndJunkToggle(state));
      }

      [DebuggerHidden]
      private IEnumerator HandleFriendAndJunkToggle(bool state)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new PilotPastController__HandleFriendAndJunkTogglec__Iterator1()
        {
          state = state,
          _this = this
        };
      }

      [DebuggerHidden]
      public IEnumerator EndPastSuccess()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new PilotPastController__EndPastSuccessc__Iterator2()
        {
          _this = this
        };
      }

      private void SetupCutscene() => PastCameraUtility.LockConversation(this.m_pilot.CenterPosition);

      private void HandleBossCutscene() => this.StartCoroutine(this.BossCutscene_CR());

      [DebuggerHidden]
      private IEnumerator BossCutscene_CR()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new PilotPastController__BossCutscene_CRc__Iterator3()
        {
          _this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator ArriveFromWarp(tk2dBaseSprite targetSprite, float duration)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new PilotPastController__ArriveFromWarpc__Iterator4()
        {
          targetSprite = targetSprite,
          duration = duration,
          _this = this
        };
      }

      public void OnBossKilled()
      {
        if (!this.m_pilot.gameObject.activeSelf)
          this.m_pilot.ResurrectFromBossKill();
        if ((bool) (Object) this.m_coop && !this.m_coop.gameObject.activeSelf)
          this.m_coop.ResurrectFromBossKill();
        this.StartCoroutine(this.HandleBossKilled());
      }

      [DebuggerHidden]
      private IEnumerator HandleBossKilled()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new PilotPastController__HandleBossKilledc__Iterator5()
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
    }

}
