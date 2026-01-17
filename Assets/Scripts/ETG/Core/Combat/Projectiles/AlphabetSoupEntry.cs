// Decompiled with JetBrains decompiler
// Type: AlphabetSoupEntry
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

namespace ETG.Core.Combat.Projectiles
{
    [Serializable]
    public class AlphabetSoupEntry
    {
      [LongNumericEnum]
      public CustomSynergyType RequiredSynergy;
      public string[] Words;
      public string[] AudioEvents;
      public Projectile BaseProjectile;
    }

}
