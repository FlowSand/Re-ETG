using UnityEngine;
using UnityEngine.SceneManagement;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Loads a Level by Index number. Before you can load a level, you have to add it to the list of levels defined in File->Build Settings...")]
    [ActionCategory(ActionCategory.Level)]
    public class LoadLevelNum : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("The level index in File->Build Settings")]
        [RequiredField]
        public FsmInt levelIndex;
        [HutongGames.PlayMaker.Tooltip("Load the level additively, keeping the current scene.")]
        public bool additive;
        [HutongGames.PlayMaker.Tooltip("Event to send after the level is loaded.")]
        public FsmEvent loadedEvent;
        [HutongGames.PlayMaker.Tooltip("Keep this GameObject in the new level. NOTE: The GameObject and components is disabled then enabled on load; uncheck Reset On Disable to keep the active state.")]
        public FsmBool dontDestroyOnLoad;
        [HutongGames.PlayMaker.Tooltip("Event to send if the level cannot be loaded.")]
        public FsmEvent failedEvent;

        public override void Reset()
        {
            this.levelIndex = (FsmInt) null;
            this.additive = false;
            this.loadedEvent = (FsmEvent) null;
            this.dontDestroyOnLoad = (FsmBool) false;
        }

        public override void OnEnter()
        {
            if (!Application.CanStreamedLevelBeLoaded(this.levelIndex.Value))
            {
                this.Fsm.Event(this.failedEvent);
                this.Finish();
            }
            else
            {
                if (this.dontDestroyOnLoad.Value)
                    Object.DontDestroyOnLoad((Object) this.Owner.transform.root.gameObject);
                if (this.additive)
                    SceneManager.LoadScene(this.levelIndex.Value, LoadSceneMode.Additive);
                else
                    SceneManager.LoadScene(this.levelIndex.Value, LoadSceneMode.Single);
                this.Fsm.Event(this.loadedEvent);
                this.Finish();
            }
        }
    }
}
