using System.Collections;
using System.Diagnostics;

using UnityEngine;

#nullable disable

public class FoyerPreloader : MonoBehaviour
    {
        public static bool IsFirstLoadScreen = true;
        public static bool IsRatLoad;
        public dfLabel LoadingLabel;
        public dfLanguageManager LanguageManager;
        public dfSprite Throbber;
        public dfSprite RatThrobber;
        private bool m_wasFirstLoadScreen;
        private bool m_isLoading;

        public void Awake()
        {
            if (FoyerPreloader.IsFirstLoadScreen)
            {
                this.LoadingLabel.gameObject.SetActive(false);
                this.LanguageManager.enabled = false;
                this.m_wasFirstLoadScreen = true;
                FoyerPreloader.IsFirstLoadScreen = false;
            }
            else
            {
                if (!FoyerPreloader.IsRatLoad)
                    return;
                this.Throbber.IsVisible = false;
                this.RatThrobber.IsVisible = true;
                FoyerPreloader.IsRatLoad = false;
            }
        }

        public void Update()
        {
            if (!this.m_wasFirstLoadScreen || UnityEngine.Time.frameCount < 5 || this.m_isLoading)
                return;
            this.StartCoroutine(this.AsyncLoadFoyer());
            this.m_isLoading = true;
        }

        [DebuggerHidden]
        private IEnumerator AsyncLoadFoyer()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new FoyerPreloader__AsyncLoadFoyerc__Iterator0()
            {
                _this = this
            };
        }
    }

