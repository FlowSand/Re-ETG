// Decompiled with JetBrains decompiler
// Type: BossChallengeData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

namespace ETG.Core.Systems.Data
{
    [Serializable]
    public class BossChallengeData
    {
      public string Annotation;
      [EnemyIdentifier]
      public string[] BossGuids;
      public int NumToSelect;
      public ChallengeModifier[] Modifiers;
    }

}
