// Decompiled with JetBrains decompiler
// Type: ShortcutElevatorController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class ShortcutElevatorController : BraveBehaviour
    {
      public ShortcutDefinition[] definedShortcuts;
      public tk2dSprite elevatorFloorSprite;
      public PressurePlate RotateLeftPlate;
      public PressurePlate RotateRightPlate;
      public SpeculativeRigidbody PlayerBlocker;
      public SpeculativeRigidbody BossRushTriggerZone;
      public tk2dSpriteAnimator RotatorBase;
      public tk2dSpriteAnimator RotatorShells;
      public tk2dSpriteAnimator Elevator;
      public MeshRenderer ElevatorFloor;
      [NonSerialized]
      private List<ShortcutDefinition> availableShortcuts = new List<ShortcutDefinition>();
      [NonSerialized]
      private int m_selectedShortcutIndex;
      private bool m_isDeparting;
      private bool m_isRotating;
      private bool m_bossRushValid;
      private bool m_queuedRotationRight;
      private bool m_queuedRotationLeft;

      private void Start()
      {
        this.DetermineAvailableShortcuts();
        if (this.availableShortcuts.Count <= 0)
          return;
        this.RotateLeftPlate.OnPressurePlateDepressed += new Action<PressurePlate>(this.RotateLeft);
        this.RotateRightPlate.OnPressurePlateDepressed += new Action<PressurePlate>(this.RotateRight);
        this.StartCoroutine(this.RotateRight("bullet_elevator_turn", "bullet_elevator_bullet_turn", 1, this.GetCachedAvailableShortcut()));
        this.ElevatorFloor.GetComponent<SpeculativeRigidbody>().OnTriggerCollision += new SpeculativeRigidbody.OnTriggerDelegate(this.OnElevatorTriggerEnter);
        if (!((UnityEngine.Object) this.BossRushTriggerZone != (UnityEngine.Object) null))
          return;
        this.BossRushTriggerZone.OnEnterTrigger += new SpeculativeRigidbody.OnTriggerDelegate(this.HandleRushTriggerEntered);
      }

      private void HandleRushTriggerEntered(
        SpeculativeRigidbody specRigidbody,
        SpeculativeRigidbody sourceSpecRigidbody,
        CollisionData collisionData)
      {
        if (!this.availableShortcuts[this.m_selectedShortcutIndex].IsBossRush || this.m_bossRushValid)
          return;
        for (int index = 0; index < StaticReferenceManager.AllNpcs.Count; ++index)
        {
          TalkDoerLite allNpc = StaticReferenceManager.AllNpcs[index];
          if ((bool) (UnityEngine.Object) allNpc && !Foyer.DoMainMenu && !GameManager.Instance.IsSelectingCharacter && GameManager.Instance.PrimaryPlayer.CurrentRoom == allNpc.ParentRoom)
            allNpc.SendPlaymakerEvent("announceBossRush");
        }
      }

      public void SetBossRushPaymentValid() => this.m_bossRushValid = true;

      private int GetCachedAvailableShortcut()
      {
        string usedShortcutTarget = GameManager.Options.lastUsedShortcutTarget;
        int availableShortcut = -1;
        for (int index = 0; index < this.availableShortcuts.Count; ++index)
        {
          if (this.availableShortcuts[index].targetLevelName == usedShortcutTarget)
          {
            availableShortcut = index;
            break;
          }
        }
        return availableShortcut;
      }

      private bool CheckPlayerPositions()
      {
        if (!GameManager.Instance.IsSelectingCharacter)
        {
          for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
          {
            if (GameManager.Instance.AllPlayers[index].CurrentRoom == this.transform.position.GetAbsoluteRoom() && (double) GameManager.Instance.AllPlayers[index].transform.position.y > (double) this.RotatorBase.Sprite.WorldBottomCenter.y)
              return false;
          }
        }
        return true;
      }

      private void RotateRight(PressurePlate obj)
      {
        bool flag = this.CheckPlayerPositions();
        if (!this.m_isRotating && flag)
        {
          this.StartCoroutine(this.RotateRight("bullet_elevator_turn", "bullet_elevator_bullet_turn", 1));
        }
        else
        {
          if (!flag)
            return;
          this.m_queuedRotationLeft = false;
          this.m_queuedRotationRight = true;
        }
      }

      private void RotateLeft(PressurePlate obj)
      {
        bool flag = this.CheckPlayerPositions();
        if (!this.m_isRotating && flag)
        {
          this.StartCoroutine(this.RotateRight("bullet_elevator_turn_reverse", "bullet_elevator_bullet_turn_reverse", -1));
        }
        else
        {
          if (!flag)
            return;
          this.m_queuedRotationLeft = true;
          this.m_queuedRotationRight = false;
        }
      }

      private void OnElevatorTriggerEnter(
        SpeculativeRigidbody otherSpecRigidbody,
        SpeculativeRigidbody sourceSpecRigidbody,
        CollisionData collisionData)
      {
        if (this.availableShortcuts[this.m_selectedShortcutIndex].IsBossRush && !this.m_bossRushValid || this.availableShortcuts[this.m_selectedShortcutIndex].IsSuperBossRush && !this.m_bossRushValid || !sourceSpecRigidbody.renderer.enabled || this.m_isDeparting || !((UnityEngine.Object) otherSpecRigidbody.GetComponent<PlayerController>() != (UnityEngine.Object) null))
          return;
        if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
        {
          bool flag = true;
          for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
          {
            if (!GameManager.Instance.AllPlayers[index].healthHaver.IsDead && !sourceSpecRigidbody.ContainsPoint(GameManager.Instance.AllPlayers[index].SpriteBottomCenter.XY(), collideWithTriggers: true))
            {
              flag = false;
              break;
            }
          }
          if (!flag)
            return;
          this.StartCoroutine(this.TriggerShortcut());
        }
        else
          this.StartCoroutine(this.TriggerShortcut());
      }

      private void DetermineAvailableShortcuts()
      {
        this.availableShortcuts.Clear();
        for (int index = 0; index < this.definedShortcuts.Length; ++index)
        {
          if (this.definedShortcuts[index].requiredFlag == GungeonFlags.NONE || GameStatsManager.Instance.GetFlag(this.definedShortcuts[index].requiredFlag))
            this.availableShortcuts.Add(this.definedShortcuts[index]);
        }
        if (this.availableShortcuts.Count == 0)
          this.m_selectedShortcutIndex = -1;
        else
          this.m_selectedShortcutIndex = 0;
      }

      protected void SetSherpaText(string key)
      {
        for (int index1 = 0; index1 < StaticReferenceManager.AllNpcs.Count; ++index1)
        {
          TalkDoerLite allNpc = StaticReferenceManager.AllNpcs[index1];
          if ((bool) (UnityEngine.Object) allNpc && !Foyer.DoMainMenu && !GameManager.Instance.IsSelectingCharacter && GameManager.Instance.PrimaryPlayer.CurrentRoom == allNpc.ParentRoom)
          {
            for (int index2 = 0; index2 < allNpc.playmakerFsms.Length; ++index2)
            {
              FsmString fsmString = allNpc.playmakerFsms[index2].FsmVariables.FindFsmString("CurrentShortcutText");
              if (fsmString != null)
                fsmString.Value = key;
            }
            allNpc.SendPlaymakerEvent("rotatoPotato");
          }
        }
      }

      public void UpdateFloorSprite(ShortcutDefinition currentDef)
      {
        if ((bool) (UnityEngine.Object) this.elevatorFloorSprite && !string.IsNullOrEmpty(currentDef.elevatorFloorSpriteName))
          this.elevatorFloorSprite.SetSprite(currentDef.elevatorFloorSpriteName);
        else
          this.elevatorFloorSprite.SetSprite("elevator_bottom_001");
      }

      [DebuggerHidden]
      public IEnumerator RotateRight(
        string rotateAnim,
        string rotateAnimShells,
        int change,
        int specificAvailableToUse = -1)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ShortcutElevatorController.<RotateRight>c__Iterator0()
        {
          change = change,
          specificAvailableToUse = specificAvailableToUse,
          rotateAnim = rotateAnim,
          rotateAnimShells = rotateAnimShells,
          _this = this
        };
      }

      [DebuggerHidden]
      public IEnumerator TriggerShortcut()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ShortcutElevatorController.<TriggerShortcut>c__Iterator1()
        {
          _this = this
        };
      }

      protected override void OnDestroy() => base.OnDestroy();
    }

}
