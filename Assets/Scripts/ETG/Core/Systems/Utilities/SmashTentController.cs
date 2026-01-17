// Decompiled with JetBrains decompiler
// Type: SmashTentController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class SmashTentController : MonoBehaviour, IPlaceConfigurable
    {
      public TalkDoerLite DrGreet;
      public TalkDoerLite DrSmash;
      public tk2dBaseSprite TableBottleSprite;
      public Transform PlayerStandPoint;
      public GameObject BottleSmashVFX;
      [NonSerialized]
      public bool IsProcessing;
      [NonSerialized]
      private bool HasSmashed;
      private PlayerController m_targetPlayer;

      public void DoSmash()
      {
        PlayerController talkingPlayer = this.DrGreet.TalkingPlayer;
        this.DrGreet.GetDungeonFSM().Fsm.SuppressGlobalTransitions = true;
        this.DrSmash.GetDungeonFSM().Fsm.SuppressGlobalTransitions = true;
        this.StartCoroutine(this.HandleSmash(talkingPlayer));
      }

      [DebuggerHidden]
      private IEnumerator WaitForPlayerPosition(PlayerController targetPlayer)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new SmashTentController.<WaitForPlayerPosition>c__Iterator0()
        {
          targetPlayer = targetPlayer,
          $this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator HandleSmash(PlayerController targetPlayer)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new SmashTentController.<HandleSmash>c__Iterator1()
        {
          targetPlayer = targetPlayer,
          $this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator HandleHearts(PlayerController targetPlayer)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new SmashTentController.<HandleHearts>c__Iterator2()
        {
          targetPlayer = targetPlayer
        };
      }

      private void HandleSmashEvent(tk2dSpriteAnimator arg1, tk2dSpriteAnimationClip arg2, int arg3)
      {
        tk2dSpriteAnimationFrame frame = arg2.GetFrame(arg3);
        if (frame.eventInfo == "grabbottle")
        {
          this.TableBottleSprite.gameObject.SetActive(false);
        }
        else
        {
          if (!(frame.eventInfo == "smash"))
            return;
          UnityEngine.Object.Instantiate<GameObject>(this.BottleSmashVFX, this.DrSmash.transform.position, Quaternion.identity);
          this.m_targetPlayer.PlayFairyEffectOnActor(ResourceCache.Acquire("Global VFX/VFX_Fairy_Fly") as GameObject, Vector3.zero, 4.5f, true);
          this.StartCoroutine(this.HandleHearts(this.m_targetPlayer));
          this.HasSmashed = true;
        }
      }

      public void ConfigureOnPlacement(RoomHandler room)
      {
        if (room.connectedRooms.Count != 1)
          return;
        room.ShouldAttemptProceduralLock = true;
        room.AttemptProceduralLockChance = Mathf.Max(room.AttemptProceduralLockChance, UnityEngine.Random.Range(0.3f, 0.5f));
      }
    }

}
