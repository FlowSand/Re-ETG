// Decompiled with JetBrains decompiler
// Type: FloorShockwaveChallengeModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Data
{
    public class FloorShockwaveChallengeModifier : ChallengeModifier
    {
      public GameObject EyesVFX;
      public float NearRadius = 5f;
      public float FarRadius = 9f;
      public float StoneDuration = 3.5f;
      public float TimeBetweenGaze = 8f;
      [NonSerialized]
      public bool Preprocessed;
      private RoomHandler m_room;
      private float m_waveTimer = 5f;

      [DebuggerHidden]
      private IEnumerator Start()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new FloorShockwaveChallengeModifier__Startc__Iterator0()
        {
          _this = this
        };
      }

      private void Update()
      {
        this.m_waveTimer -= BraveTime.DeltaTime;
        if ((double) this.m_waveTimer > 0.0)
          return;
        this.m_waveTimer = this.TimeBetweenGaze;
        IntVector2? forChallengeBurst = CircleBurstChallengeModifier.GetAppropriateSpawnPointForChallengeBurst(this.m_room, this.NearRadius, this.FarRadius);
        if (!forChallengeBurst.HasValue)
          return;
        ChallengeManager.Instance.StartCoroutine(this.LaunchWave(forChallengeBurst.Value.ToCenterVector2()));
      }

      [DebuggerHidden]
      private IEnumerator LaunchWave(Vector2 startPoint)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new FloorShockwaveChallengeModifier__LaunchWavec__Iterator1()
        {
          startPoint = startPoint,
          _this = this
        };
      }
    }

}
