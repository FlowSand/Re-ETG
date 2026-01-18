using UnityEngine;

#nullable disable

public class LoadingScreenController : MonoBehaviour
  {
    public tk2dBaseSprite DEBUG_SPRITE;
    public dfSprite ItemBowlSprite;
    public dfSprite ItemDescriptionBox;
    public dfLabel ItemNameLabel;
    public dfLabel ItemDescriptionLabel;
    public tk2dBaseSprite ItemSprite;

    private void Start()
    {
      this.ItemDescriptionLabel.SizeChanged += new PropertyChangedEventHandler<Vector2>(this.OnDescriptionLabelSizeChanged);
      this.ItemSprite.ignoresTiltworldDepth = true;
    }

    private void Update()
    {
    }

    private void OnDescriptionLabelSizeChanged(dfControl control, Vector2 value)
    {
      if (!((Object) control == (Object) this.ItemDescriptionLabel))
        return;
      Vector2 vector2 = this.ItemDescriptionLabel.Font.ObtainRenderer().MeasureString(this.ItemDescriptionLabel.Text);
      this.ItemDescriptionBox.Size = new Vector2(this.ItemDescriptionBox.Size.x, (float) Mathf.CeilToInt(vector2.x / this.ItemDescriptionLabel.Size.x) * vector2.y + 66f);
      this.ItemDescriptionBox.Size = new Vector2(Mathf.Max(this.ItemNameLabel.Font.ObtainRenderer().MeasureString(this.ItemNameLabel.Text).x, this.ItemDescriptionBox.Size.x), this.ItemDescriptionBox.Size.y);
    }

    public void ChangeToNewItem(tk2dBaseSprite sourceSprite, JournalEntry entry)
    {
    }
  }

