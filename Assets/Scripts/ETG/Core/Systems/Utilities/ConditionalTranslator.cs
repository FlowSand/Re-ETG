using UnityEngine;

#nullable disable

public class ConditionalTranslator : MonoBehaviour
  {
    public string EnglishText;
    public string NonEnglishItemsKey;
    public bool useUiTable;
    private dfControl m_control;

    private void Start()
    {
      this.m_control = this.GetComponent<dfControl>();
      if (!(bool) (Object) this.m_control)
        return;
      this.m_control.IsEnabledChanged += new PropertyChangedEventHandler<bool>(this.HandleTranslation);
      this.m_control.IsVisibleChanged += new PropertyChangedEventHandler<bool>(this.HandleTranslation);
    }

    private void SetText(string targetText)
    {
      if (!(this.m_control is dfLabel))
        return;
      (this.m_control as dfLabel).Text = targetText;
    }

    private void HandleTranslation(dfControl control, bool value)
    {
      if (StringTableManager.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.ENGLISH)
      {
        this.m_control.IsLocalized = false;
        this.SetText(this.EnglishText);
      }
      else if (this.useUiTable)
      {
        this.m_control.IsLocalized = true;
        this.SetText(this.NonEnglishItemsKey);
      }
      else
      {
        this.m_control.IsLocalized = false;
        this.SetText(StringTableManager.GetItemsString(this.NonEnglishItemsKey));
      }
    }
  }

