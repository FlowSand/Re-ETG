// Decompiled with JetBrains decompiler
// Type: PotFairyEngageDoer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class PotFairyEngageDoer : CustomEngageDoer
    {
      public static bool InstantSpawn;
      public GameObject[] PotPrefabs;
      private bool m_isFinished;
      private MinorBreakable m_minorBreakable;
      private bool m_hasDonePotCheck;

      public void Awake()
      {
        if (BraveUtility.RandomBool())
        {
          this.aiAnimator.IdleAnimation.Prefix = this.aiAnimator.IdleAnimation.Prefix.Replace("pink", "blue");
          for (int index = 0; index < this.aiAnimator.OtherAnimations.Count; ++index)
            this.aiAnimator.OtherAnimations[index].anim.Prefix = this.aiAnimator.OtherAnimations[index].anim.Prefix.Replace("pink", "blue");
        }
        if (!PotFairyEngageDoer.InstantSpawn)
          return;
        this.StartIntro();
      }

      public void Update()
      {
        if (!this.m_hasDonePotCheck)
        {
          if (!PotFairyEngageDoer.InstantSpawn && !this.aiActor.IsInReinforcementLayer)
          {
            this.specRigidbody.Initialize();
            IntVector2 intVector2 = this.specRigidbody.UnitCenter.ToIntVector2(VectorConversions.Floor);
            RoomHandler roomFromPosition = GameManager.Instance.Dungeon.GetRoomFromPosition(intVector2);
            this.m_minorBreakable = DungeonPlaceableUtility.InstantiateDungeonPlaceable(BraveUtility.RandomElement<GameObject>(this.PotPrefabs), roomFromPosition, intVector2 - roomFromPosition.area.basePosition, true).GetComponent<MinorBreakable>();
          }
          this.m_hasDonePotCheck = true;
        }
        if (!(bool) (Object) this.specRigidbody || !this.specRigidbody.enabled)
          return;
        RoomHandler roomFromPosition1 = GameManager.Instance.Dungeon.GetRoomFromPosition(this.specRigidbody.UnitCenter.ToIntVector2(VectorConversions.Floor));
        foreach (PlayerController allPlayer in GameManager.Instance.AllPlayers)
        {
          if (allPlayer.healthHaver.IsAlive && allPlayer.CurrentRoom != null && allPlayer.CurrentRoom.IsSealed && allPlayer.CurrentRoom != roomFromPosition1)
          {
            this.aiActor.CanDropCurrency = false;
            this.aiActor.CanDropItems = false;
            this.healthHaver.ApplyDamage(10000f, Vector2.zero, "Lonely Suicide", damageCategory: DamageCategory.Unstoppable, ignoreInvulnerabilityFrames: true);
            break;
          }
        }
      }

      public override void StartIntro()
      {
        if (this.m_isFinished)
          return;
        this.StartCoroutine(this.DoIntro());
      }

      [DebuggerHidden]
      private IEnumerator DoIntro()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new PotFairyEngageDoer.<DoIntro>c__Iterator0()
        {
          $this = this
        };
      }

      public override bool IsFinished => this.m_isFinished;
    }

}
