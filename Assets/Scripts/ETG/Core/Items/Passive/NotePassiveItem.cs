// Decompiled with JetBrains decompiler
// Type: NotePassiveItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable

namespace ETG.Core.Items.Passive
{
    public class NotePassiveItem : PassiveItem
    {
      public int ResourcefulRatNoteIdentifier = -1;

      private void Awake()
      {
        if (this.ResourcefulRatNoteIdentifier < 0)
          return;
        string appropriateSpriteName = this.GetAppropriateSpriteName(false);
        if (string.IsNullOrEmpty(appropriateSpriteName))
          return;
        this.sprite.SetSprite(appropriateSpriteName);
      }

      public string GetAppropriateSpriteName(bool isAmmonomicon)
      {
        return isAmmonomicon ? "resourcefulrat_note_base_001" : "resourcefulrat_note_base";
      }

      protected override void OnDestroy() => base.OnDestroy();
    }

}
