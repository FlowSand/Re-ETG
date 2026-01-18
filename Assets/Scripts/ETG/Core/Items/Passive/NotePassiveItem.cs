#nullable disable

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

