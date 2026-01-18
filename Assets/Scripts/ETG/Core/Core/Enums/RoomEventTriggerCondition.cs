// Decompiled with JetBrains decompiler
// Type: RoomEventTriggerCondition
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable

public enum RoomEventTriggerCondition
  {
    ON_ENTER = 0,
    ON_EXIT = 1,
    ON_ENEMIES_CLEARED = 2,
    ON_ENTER_WITH_ENEMIES = 3,
    ON_ONE_QUARTER_ENEMY_HP_DEPLETED = 8,
    ON_HALF_ENEMY_HP_DEPLETED = 12, // 0x0000000C
    ON_THREE_QUARTERS_ENEMY_HP_DEPLETED = 16, // 0x00000010
    ON_NINETY_PERCENT_ENEMY_HP_DEPLETED = 20, // 0x00000014
    TIMER = 30, // 0x0000001E
    SHRINE_WAVE_A = 40, // 0x00000028
    SHRINE_WAVE_B = 41, // 0x00000029
    SHRINE_WAVE_C = 42, // 0x0000002A
    NPC_TRIGGER_A = 60, // 0x0000003C
    NPC_TRIGGER_B = 61, // 0x0000003D
    NPC_TRIGGER_C = 62, // 0x0000003E
    ENEMY_BEHAVIOR = 80, // 0x00000050
    SEQUENTIAL_WAVE_TRIGGER = 100, // 0x00000064
  }

