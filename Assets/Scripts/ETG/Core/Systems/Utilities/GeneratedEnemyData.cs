// Decompiled with JetBrains decompiler
// Type: GeneratedEnemyData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public struct GeneratedEnemyData(string id, float percent, bool isSig)
    {
      public string enemyGuid = id;
      public float percentOfEnemies = percent;
      public bool isSignatureEnemy = isSig;
    }

}
