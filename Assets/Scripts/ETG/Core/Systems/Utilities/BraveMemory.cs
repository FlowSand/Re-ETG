// Decompiled with JetBrains decompiler
// Type: BraveMemory
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public static class BraveMemory
    {
      private static float LastGcTime;

      public static void EnsureHeapSize(int kilobytes)
      {
        object[] objArray = new object[kilobytes];
        for (int index = 0; index < kilobytes; ++index)
          objArray[index] = (object) new byte[1024 /*0x0400*/];
        GC.Collect();
      }

      public static void DoCollect()
      {
        BraveMemory.LastGcTime = UnityEngine.Time.realtimeSinceStartup;
        GC.Collect();
      }

      private static void MaybeDoCollect() => BraveMemory.LastGcTime = UnityEngine.Time.realtimeSinceStartup;

      private static bool CheckTime(float threshold)
      {
        return (double) UnityEngine.Time.realtimeSinceStartup - (double) BraveMemory.LastGcTime > (double) threshold;
      }

      private static void TestPC()
      {
        for (int index = 0; index < 5; ++index)
          GC.Collect();
      }

      public static void HandleBossCardFlashAnticipation()
      {
        if (!BraveMemory.CheckTime(20f))
          return;
        BraveMemory.MaybeDoCollect();
        GenericIntroDoer.SkipFrame = true;
      }

      public static void HandleBossCardSkip()
      {
        if (!BraveMemory.CheckTime(20f))
          return;
        BraveMemory.MaybeDoCollect();
        GenericIntroDoer.SkipFrame = true;
      }

      public static void HandleRoomEntered(int numOfEnemies)
      {
        float threshold = 360f;
        if (numOfEnemies >= 8)
          threshold = 240f;
        if (!BraveMemory.CheckTime(threshold))
          return;
        BraveMemory.MaybeDoCollect();
      }

      public static void HandleGamePaused()
      {
        if (GameManager.Instance.CurrentGameType != GameManager.GameType.COOP_2_PLAYER || !BraveMemory.CheckTime(360f))
          return;
        BraveMemory.MaybeDoCollect();
      }

      public static void HandleTeleportation()
      {
        if (!BraveMemory.CheckTime(300f))
          return;
        BraveMemory.MaybeDoCollect();
      }
    }

}
