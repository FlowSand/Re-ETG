using UnityEngine;
using UnityEngine.SceneManagement;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Loads a Level by Name. NOTE: Before you can load a level, you have to add it to the list of levels defined in File->Build Settings...")]
    [ActionCategory(ActionCategory.Level)]
    public class LoadLevel : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("The name of the level to load. NOTE: Must be in the list of levels defined in File->Build Settings... ")]
        [RequiredField]
        public FsmString levelName;
        [HutongGames.PlayMaker.Tooltip("Load the level additively, keeping the current scene.")]
        public bool additive;
        [HutongGames.PlayMaker.Tooltip("Load the level asynchronously in the background.")]
        public bool async;
        [HutongGames.PlayMaker.Tooltip("Event to send when the level has loaded. NOTE: This only makes sense if the FSM is still in the scene!")]
        public FsmEvent loadedEvent;
        [HutongGames.PlayMaker.Tooltip("Keep this GameObject in the new level. NOTE: The GameObject and components is disabled then enabled on load; uncheck Reset On Disable to keep the active state.")]
        public FsmBool dontDestroyOnLoad;
        [HutongGames.PlayMaker.Tooltip("Event to send if the level cannot be loaded.")]
        public FsmEvent failedEvent;
        private AsyncOperation asyncOperation;

        public override void Reset()
        {
            this.levelName = (FsmString) string.Empty;
            this.additive = false;
            this.async = false;
            this.loadedEvent = (FsmEvent) null;
            this.dontDestroyOnLoad = (FsmBool) false;
        }

        public override void OnEnter()
        {
            if (!Application.CanStreamedLevelBeLoaded(this.levelName.Value))
            {
                this.Fsm.Event(this.failedEvent);
                this.Finish();
            }
            else
            {
                if (this.dontDestroyOnLoad.Value)
                    Object.DontDestroyOnLoad((Object) this.Owner.transform.root.gameObject);
                if (this.additive)
                {
                    if (this.async)
                    {
                        this.asyncOperation = SceneManager.LoadSceneAsync(this.levelName.Value, LoadSceneMode.Additive);
                        Debug.Log((object) ("LoadLevelAdditiveAsyc: " + this.levelName.Value));
                        return;
                    }
                    SceneManager.LoadScene(this.levelName.Value, LoadSceneMode.Additive);
                    Debug.Log((object) ("LoadLevelAdditive: " + this.levelName.Value));
                }
                else
                {
                    if (this.async)
                    {
                        this.asyncOperation = SceneManager.LoadSceneAsync(this.levelName.Value, LoadSceneMode.Single);
                        Debug.Log((object) ("LoadLevelAsync: " + this.levelName.Value));
                        return;
                    }
                    SceneManager.LoadScene(this.levelName.Value, LoadSceneMode.Single);
                    Debug.Log((object) ("LoadLevel: " + this.levelName.Value));
                }
                this.Log("LOAD COMPLETE");
                this.Fsm.Event(this.loadedEvent);
                this.Finish();
            }
        }

        public override void OnUpdate()
        {
            if (!this.asyncOperation.isDone)
                return;
            this.Fsm.Event(this.loadedEvent);
            this.Finish();
        }
    }
}
