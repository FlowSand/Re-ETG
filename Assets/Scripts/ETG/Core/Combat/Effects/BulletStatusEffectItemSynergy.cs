// Decompiled with JetBrains decompiler
// Type: BulletStatusEffectItemSynergy
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

namespace ETG.Core.Combat.Effects
{
    [Serializable]
    public class BulletStatusEffectItemSynergy
    {
      [LongNumericEnum]
      public CustomSynergyType RequiredSynergy;
      public float ChanceMultiplier = 1f;
    }

}
