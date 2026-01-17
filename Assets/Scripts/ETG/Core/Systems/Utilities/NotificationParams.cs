// Decompiled with JetBrains decompiler
// Type: NotificationParams
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class NotificationParams
    {
      public bool isSingleLine;
      public string EncounterGuid;
      public int pickupId = -1;
      public string PrimaryTitleString;
      public string SecondaryDescriptionString;
      public tk2dSpriteCollectionData SpriteCollection;
      public int SpriteID;
      public UINotificationController.NotificationColor forcedColor;
      public bool OnlyIfSynergy;
      public bool HasAttachedSynergy;
      public AdvancedSynergyEntry AttachedSynergy;
    }

}
