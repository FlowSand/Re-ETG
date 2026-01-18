// Decompiled with JetBrains decompiler
// Type: IPaydayItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable

public interface IPaydayItem
  {
    void StoreData(string id1, string id2, string id3);

    string GetID(int placement);

    bool HasCachedData();
  }

