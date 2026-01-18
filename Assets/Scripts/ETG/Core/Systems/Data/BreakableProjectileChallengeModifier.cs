using Dungeonator;
using System;
using UnityEngine;

#nullable disable

public class BreakableProjectileChallengeModifier : ChallengeModifier
  {
    public bool AimAtPlayer = true;
    private AIBulletBank m_bulletBank;

    private void Start()
    {
      this.m_bulletBank = this.GetComponent<AIBulletBank>();
      RoomHandler currentRoom = GameManager.Instance.PrimaryPlayer.CurrentRoom;
      for (int index = 0; index < StaticReferenceManager.AllMinorBreakables.Count; ++index)
      {
        MinorBreakable allMinorBreakable = StaticReferenceManager.AllMinorBreakables[index];
        if ((bool) (UnityEngine.Object) allMinorBreakable && !allMinorBreakable.IsBroken && allMinorBreakable.CenterPoint.GetAbsoluteRoom() == currentRoom && !allMinorBreakable.IgnoredForPotShotsModifier)
          allMinorBreakable.OnBreakContext += new Action<MinorBreakable>(this.HandleBroken);
      }
    }

    private void HandleBroken(MinorBreakable mb)
    {
      if (!(bool) (UnityEngine.Object) this || (double) UnityEngine.Time.realtimeSinceStartup - (double) GameManager.Instance.BestActivePlayer.RealtimeEnteredCurrentRoom < 3.0 || !(bool) (UnityEngine.Object) mb)
        return;
      if (this.AimAtPlayer)
      {
        PlayerController playerClosestToPoint = GameManager.Instance.GetActivePlayerClosestToPoint(mb.CenterPoint);
        if (!(bool) (UnityEngine.Object) playerClosestToPoint || (double) (playerClosestToPoint.CenterPosition - mb.CenterPoint).magnitude <= 1.0)
          return;
        this.FireBullet((Vector3) mb.CenterPoint, playerClosestToPoint.CenterPosition - mb.CenterPoint);
      }
      else
        this.FireBullet((Vector3) mb.CenterPoint, UnityEngine.Random.insideUnitCircle.normalized);
    }

    private void FireBullet(Vector3 shootPoint, Vector2 direction)
    {
      this.m_bulletBank.CreateProjectileFromBank((Vector2) shootPoint, BraveMathCollege.Atan2Degrees(direction), "default");
    }

    private void OnDestroy()
    {
      if (Dungeon.IsGenerating || !GameManager.HasInstance || GameManager.Instance.IsLoadingLevel || !(bool) (UnityEngine.Object) GameManager.Instance.PrimaryPlayer || GameManager.Instance.PrimaryPlayer.CurrentRoom == null)
        return;
      RoomHandler currentRoom = GameManager.Instance.PrimaryPlayer.CurrentRoom;
      for (int index = 0; index < StaticReferenceManager.AllMinorBreakables.Count; ++index)
      {
        MinorBreakable allMinorBreakable = StaticReferenceManager.AllMinorBreakables[index];
        if ((bool) (UnityEngine.Object) allMinorBreakable && allMinorBreakable.CenterPoint.GetAbsoluteRoom() == currentRoom)
          allMinorBreakable.OnBreakContext -= new Action<MinorBreakable>(this.HandleBroken);
      }
    }
  }

