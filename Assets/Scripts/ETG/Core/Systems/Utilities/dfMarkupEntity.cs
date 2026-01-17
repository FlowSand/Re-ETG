// Decompiled with JetBrains decompiler
// Type: dfMarkupEntity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using System.Text;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class dfMarkupEntity
    {
      private static List<dfMarkupEntity> HTML_ENTITIES = new List<dfMarkupEntity>()
      {
        new dfMarkupEntity("&nbsp;", " "),
        new dfMarkupEntity("&quot;", "\""),
        new dfMarkupEntity("&amp;", "&"),
        new dfMarkupEntity("&lt;", "<"),
        new dfMarkupEntity("&gt;", ">"),
        new dfMarkupEntity("&#39;", "'"),
        new dfMarkupEntity("&trade;", "™"),
        new dfMarkupEntity("&copy;", "©"),
        new dfMarkupEntity(" ", " ")
      };
      private static StringBuilder buffer = new StringBuilder();
      public string EntityName;
      public string EntityChar;

      public dfMarkupEntity(string entityName, string entityChar)
      {
        this.EntityName = entityName;
        this.EntityChar = entityChar;
      }

      public static string Replace(string text)
      {
        dfMarkupEntity.buffer.EnsureCapacity(text.Length);
        dfMarkupEntity.buffer.Length = 0;
        dfMarkupEntity.buffer.Append(text);
        for (int index = 0; index < dfMarkupEntity.HTML_ENTITIES.Count; ++index)
        {
          dfMarkupEntity dfMarkupEntity = dfMarkupEntity.HTML_ENTITIES[index];
          dfMarkupEntity.buffer.Replace(dfMarkupEntity.EntityName, dfMarkupEntity.EntityChar);
        }
        return dfMarkupEntity.buffer.ToString();
      }
    }

}
