// Decompiled with JetBrains decompiler
// Type: CursePotController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class CursePotController : BraveBehaviour
    {
      public string IdleAnim;
      public string AnimToPlayWhenExcited;
      public float TimeToCursePoint = 3f;
      public tk2dSprite CircleSprite;
      public tk2dSprite ShadowSprite;
      private tk2dSpriteAnimationClip m_idleClip;
      private tk2dSpriteAnimationClip m_activeClip;
      private List<PlayerController> m_cursedPlayers = new List<PlayerController>();

      private void Start()
      {
        this.m_idleClip = this.spriteAnimator.GetClipByName(this.IdleAnim);
        this.m_activeClip = this.spriteAnimator.GetClipByName(this.AnimToPlayWhenExcited);
        this.specRigidbody.OnEnterTrigger += new SpeculativeRigidbody.OnTriggerDelegate(this.HandleTriggerEntered);
        this.specRigidbody.OnExitTrigger += new SpeculativeRigidbody.OnTriggerExitDelegate(this.HandleTriggerExited);
        this.GetComponent<MinorBreakable>().OnBreak += new System.Action(this.HandleBreak);
        if (!(bool) (UnityEngine.Object) this.CircleSprite)
          return;
        tk2dSpriteDefinition currentSpriteDef = this.CircleSprite.GetCurrentSpriteDef();
        Vector2 lhs1 = new Vector2(float.MaxValue, float.MaxValue);
        Vector2 lhs2 = new Vector2(float.MinValue, float.MinValue);
        for (int index = 0; index < currentSpriteDef.uvs.Length; ++index)
        {
          lhs1 = Vector2.Min(lhs1, currentSpriteDef.uvs[index]);
          lhs2 = Vector2.Max(lhs2, currentSpriteDef.uvs[index]);
        }
        Vector2 vector2 = (lhs1 + lhs2) / 2f;
        this.CircleSprite.renderer.material.SetVector("_WorldCenter", new Vector4(vector2.x, vector2.y, vector2.x - lhs1.x, vector2.y - lhs1.y));
      }

      private void HandleBreak()
      {
        if ((bool) (UnityEngine.Object) this.CircleSprite)
          this.CircleSprite.transform.parent = (Transform) null;
        if ((bool) (UnityEngine.Object) this.ShadowSprite)
          this.ShadowSprite.transform.parent = (Transform) null;
        GameManager.Instance.Dungeon.StartCoroutine(this.HandleBreakCR());
      }

      [DebuggerHidden]
      private IEnumerator HandleBreakCR()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new CursePotController__HandleBreakCRc__Iterator0()
        {
          _this = this
        };
      }

      private void Update()
      {
        if (this.minorBreakable.IsBroken)
          return;
        if (this.m_cursedPlayers.Count > 0)
        {
          if (!this.spriteAnimator.IsPlaying(this.m_activeClip))
            this.spriteAnimator.Play(this.m_activeClip);
        }
        else if (!this.spriteAnimator.IsPlaying(this.m_idleClip))
          this.spriteAnimator.Play(this.m_idleClip);
        for (int index = 0; index < this.m_cursedPlayers.Count; ++index)
          this.DoCurse(this.m_cursedPlayers[index]);
      }

      private void DoCurse(PlayerController targetPlayer)
      {
        if (targetPlayer.IsGhost)
          return;
        targetPlayer.CurrentCurseMeterValue += BraveTime.DeltaTime / this.TimeToCursePoint;
        targetPlayer.CurseIsDecaying = false;
        if ((double) targetPlayer.CurrentCurseMeterValue <= 1.0)
          return;
        targetPlayer.CurrentCurseMeterValue = 0.0f;
        targetPlayer.ownerlessStatModifiers.Add(new StatModifier()
        {
          amount = 1f,
          modifyType = StatModifier.ModifyMethod.ADDITIVE,
          statToBoost = PlayerStats.StatType.Curse
        });
        targetPlayer.stats.RecalculateStats(targetPlayer);
        this.minorBreakable.Break();
      }

      private void HandleTriggerExited(
        SpeculativeRigidbody exitRigidbody,
        SpeculativeRigidbody sourceSpecRigidbody)
      {
        if (!(bool) (UnityEngine.Object) exitRigidbody || !(bool) (UnityEngine.Object) exitRigidbody.gameActor || !(exitRigidbody.gameActor is PlayerController) || !this.m_cursedPlayers.Contains(exitRigidbody.gameActor as PlayerController))
          return;
        PlayerController gameActor = exitRigidbody.gameActor as PlayerController;
        gameActor.CurseIsDecaying = true;
        this.m_cursedPlayers.Remove(gameActor);
      }

      private void HandleTriggerEntered(
        SpeculativeRigidbody enteredRigidbody,
        SpeculativeRigidbody sourceSpecRigidbody,
        CollisionData collisionData)
      {
        if (GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(this.specRigidbody.UnitCenter.ToIntVector2()) != GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(enteredRigidbody.UnitCenter.ToIntVector2()) || !((UnityEngine.Object) enteredRigidbody.gameActor != (UnityEngine.Object) null) || !(enteredRigidbody.gameActor is PlayerController))
          return;
        PlayerController gameActor = enteredRigidbody.gameActor as PlayerController;
        gameActor.CurseIsDecaying = false;
        this.m_cursedPlayers.Add(gameActor);
      }
    }

}
