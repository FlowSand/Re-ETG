// Decompiled with JetBrains decompiler
// Type: DungeonFlowSubtypeRestriction
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

[Serializable]
public class DungeonFlowSubtypeRestriction
  {
    public PrototypeDungeonRoom.RoomCategory baseCategoryRestriction = PrototypeDungeonRoom.RoomCategory.NORMAL;
    public PrototypeDungeonRoom.RoomNormalSubCategory normalSubcategoryRestriction;
    public PrototypeDungeonRoom.RoomBossSubCategory bossSubcategoryRestriction;
    public PrototypeDungeonRoom.RoomSpecialSubCategory specialSubcategoryRestriction;
    public PrototypeDungeonRoom.RoomSecretSubCategory secretSubcategoryRestriction;
    public int maximumRoomsOfSubtype = 1;
  }

