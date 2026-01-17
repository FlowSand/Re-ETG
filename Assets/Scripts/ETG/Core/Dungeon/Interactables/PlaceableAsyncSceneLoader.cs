// Decompiled with JetBrains decompiler
// Type: PlaceableAsyncSceneLoader
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

#nullable disable

namespace ETG.Core.Dungeon.Interactables
{
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
        return (IEnumerator) new PlaceableAsyncSceneLoader.<WaitForChunkLoaded>c__Iterator0()
        {
          loader = loader,
          $this = this
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

}
