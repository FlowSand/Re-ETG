// Decompiled with JetBrains decompiler
// Type: dfMarkupTokenAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class dfMarkupTokenAttribute : IPoolable
    {
      public dfMarkupToken Key;
      public dfMarkupToken Value;
      private static dfList<dfMarkupTokenAttribute> pool = new dfList<dfMarkupTokenAttribute>();

      private dfMarkupTokenAttribute()
      {
      }

      public static dfMarkupTokenAttribute Obtain(dfMarkupToken key, dfMarkupToken value)
      {
        dfMarkupTokenAttribute markupTokenAttribute = dfMarkupTokenAttribute.pool.Count <= 0 ? new dfMarkupTokenAttribute() : dfMarkupTokenAttribute.pool.Pop();
        markupTokenAttribute.Key = key;
        markupTokenAttribute.Value = value;
        return markupTokenAttribute;
      }

      public void Release()
      {
        if (this.Key != null)
        {
          this.Key.Release();
          this.Key = (dfMarkupToken) null;
        }
        if (this.Value != null)
        {
          this.Value.Release();
          this.Value = (dfMarkupToken) null;
        }
        if (dfMarkupTokenAttribute.pool.Contains(this))
          return;
        dfMarkupTokenAttribute.pool.Add(this);
      }
    }

}
