using Dungeonator;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

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
      return (IEnumerator) new SmashTentController__WaitForPlayerPositionc__Iterator0()
      {
        targetPlayer = targetPlayer,
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator HandleSmash(PlayerController targetPlayer)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new SmashTentController__HandleSmashc__Iterator1()
      {
        targetPlayer = targetPlayer,
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator HandleHearts(PlayerController targetPlayer)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new SmashTentController__HandleHeartsc__Iterator2()
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

