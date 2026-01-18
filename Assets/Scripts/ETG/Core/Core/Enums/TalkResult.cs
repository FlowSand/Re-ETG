using System;
using UnityEngine;

#nullable disable

[Serializable]
public class TalkResult
  {
    public TalkResult.TalkResultAction action;
    public GameObject objectData;
    public string actionData;
    [ShowInInspectorIf("action", 8, false)]
    public GenericLootTable lootTableData;
    public string customActionID;

    public enum TalkResultAction
    {
      CHANGE_DEFAULT_MODULE = 0,
      OPEN_TRUTH_CHEST = 1,
      VANISH = 2,
      TOSS_CURRENT_GUN_IN_POT = 3,
      RENDER_SILENT = 4,
      CHANGE_DEFAULT_MODULE_OF_OTHER_TALKDOER = 5,
      SPAWN_ITEM = 6,
      MAKE_TALKDOER_INTERACTABLE = 7,
      SPAWN_ITEM_FROM_TABLE = 8,
      CUSTOM_ACTION = 99, // 0x00000063
    }
  }

