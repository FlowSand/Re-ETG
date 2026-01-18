// Decompiled with JetBrains decompiler
// Type: GeneratedEnemyData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable

public struct GeneratedEnemyData
  {
    public string enemyGuid;
    public float percentOfEnemies;
    public bool isSignatureEnemy;

    public GeneratedEnemyData(string id, float percent, bool isSig)
    {
      enemyGuid = id;
      percentOfEnemies = percent;
      isSignatureEnemy = isSig;
    }
  }

