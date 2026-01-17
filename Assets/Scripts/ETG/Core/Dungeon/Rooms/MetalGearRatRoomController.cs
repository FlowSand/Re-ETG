// Decompiled with JetBrains decompiler
// Type: MetalGearRatRoomController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Dungeon.Rooms
{
    public class MetalGearRatRoomController : MonoBehaviour
    {
      public GameObject brokenMetalGear;
      public GameObject floorCover;

      [DebuggerHidden]
      public IEnumerator Start()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new MetalGearRatRoomController.<Start>c__Iterator0()
        {
          $this = this
        };
      }

      private void HandlePitfallIntoReward()
      {
        GameManager.Instance.Dungeon.StartCoroutine(this.HandlePitfallIntoRewardCR());
      }

      [DebuggerHidden]
      private IEnumerator HandlePitfallIntoRewardCR()
      {
        // ISSUE: object of a compiler-generated type is created
        // ISSUE: variable of a compiler-generated type
        MetalGearRatRoomController.<HandlePitfallIntoRewardCR>c__Iterator1 rewardCrCIterator1 = new MetalGearRatRoomController.<HandlePitfallIntoRewardCR>c__Iterator1();
        return (IEnumerator) rewardCrCIterator1;
      }

      public void EnablePitfalls(bool value)
      {
        this.floorCover.SetActive(!value);
        IntVector2 intVector2 = this.transform.position.GetAbsoluteRoom().area.basePosition + new IntVector2(19, 12);
        for (int x = 0; x < 8; ++x)
        {
          for (int y = 0; y < 5; ++y)
            GameManager.Instance.Dungeon.data[intVector2 + new IntVector2(x, y)].fallingPrevented = !value;
        }
      }

      public void TransformToDestroyedRoom()
      {
        this.brokenMetalGear.SetActive(true);
        this.EnablePitfalls(true);
        RoomHandler absoluteRoom = this.transform.position.GetAbsoluteRoom();
        if (absoluteRoom == null || absoluteRoom.DarkSoulsRoomResetDependencies == null)
          return;
        absoluteRoom.DarkSoulsRoomResetDependencies.Clear();
      }
    }

}
