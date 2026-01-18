using System.Collections;
using System.Diagnostics;

using UnityEngine;
using UnityEngine.SceneManagement;

using Dungeonator;

#nullable disable

public class PlaceableAsyncSceneLoader : DungeonPlaceableBehaviour, IPlaceConfigurable
    {
        public string asyncSceneName;
        public string asyncChunkIdentifier;
        public bool DoNoncombatSetup;

        public void ConfigureOnPlacement(RoomHandler room)
        {
            if (this.DoNoncombatSetup)
                this.NoncombatSetup();
            DebugTime.Log("PlaceableAsyncSceneLoader.LoadScene({0})", (object) this.asyncSceneName);
            if (this.asyncSceneName == "Foyer")
                this.LoadBundledScene("Foyer", "foyer_002");
            else if (this.asyncSceneName == "Foyer_Coop")
                this.LoadBundledScene("Foyer_Coop", "foyer_003");
            else
                SceneManager.LoadScene(this.asyncSceneName, LoadSceneMode.Additive);
        }

        [DebuggerHidden]
        private IEnumerator WaitForChunkLoaded(AsyncOperation loader)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new PlaceableAsyncSceneLoader__WaitForChunkLoadedc__Iterator0()
            {
                loader = loader,
                _this = this
            };
        }

        private void NoncombatSetup()
        {
            GameUIRoot.Instance.ForceHideGunPanel = true;
            GameUIRoot.Instance.ForceHideItemPanel = true;
        }

        private void LoadBundledScene(string sceneName, string bundleName)
        {
            AssetBundle assetBundle = ResourceManager.LoadAssetBundle(bundleName);
            DebugTime.RecordStartTime();
            ResourceManager.LoadSceneFromBundle(assetBundle, LoadSceneMode.Additive);
            DebugTime.Log("Application.LoadLevel(foyer)");
        }

        protected override void OnDestroy() => base.OnDestroy();
    }

