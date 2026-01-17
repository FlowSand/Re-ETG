// Decompiled with JetBrains decompiler
// Type: Dungeonator.RuntimeInjectionFlags
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace Dungeonator
{
  [Serializable]
  public class RuntimeInjectionFlags
  {
    public bool ShopAnnexed;
    public bool CastleFireplace;

    public void Clear()
    {
      this.ShopAnnexed = false;
      this.CastleFireplace = false;
    }

    public bool Merge(RuntimeInjectionFlags flags)
    {
      bool flag = false;
      if (!this.CastleFireplace && flags.CastleFireplace)
        flag = true;
      this.ShopAnnexed |= flags.ShopAnnexed;
      this.CastleFireplace |= flags.CastleFireplace;
      return flag;
    }

    public bool IsValid(RuntimeInjectionFlags other)
    {
      bool flag = true;
      if (this.ShopAnnexed && other.ShopAnnexed)
        flag = false;
      if (this.CastleFireplace && other.CastleFireplace)
        flag = false;
      return flag;
    }
  }
}
