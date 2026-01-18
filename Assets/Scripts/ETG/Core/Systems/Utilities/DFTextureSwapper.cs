using UnityEngine;

#nullable disable

public class DFTextureSwapper : MonoBehaviour
    {
        public string EnglishSpriteName;
        public string OtherSpriteName;

        private void Start()
        {
            dfControl component = this.GetComponent<dfControl>();
            if (!(bool) (Object) component)
                return;
            component.IsVisibleChanged += new PropertyChangedEventHandler<bool>(this.HandleVisibilityChanged);
            if (!component.IsVisible)
                return;
            this.HandleVisibilityChanged(component, true);
        }

        private void HandleVisibilityChanged(dfControl control, bool value)
        {
            switch (control)
            {
                case dfSlicedSprite _:
                    (control as dfSlicedSprite).SpriteName = GameManager.Options.CurrentLanguage != StringTableManager.GungeonSupportedLanguages.ENGLISH ? this.OtherSpriteName : this.EnglishSpriteName;
                    break;
                case dfSprite _:
                    (control as dfSprite).SpriteName = GameManager.Options.CurrentLanguage != StringTableManager.GungeonSupportedLanguages.ENGLISH ? this.OtherSpriteName : this.EnglishSpriteName;
                    break;
            }
        }
    }

