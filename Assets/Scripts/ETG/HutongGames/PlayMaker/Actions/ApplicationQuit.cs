using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Quits the player application.")]
    [ActionCategory(ActionCategory.Application)]
    public class ApplicationQuit : FsmStateAction
    {
        public override void Reset()
        {
        }

        public override void OnEnter()
        {
            Application.Quit();
            this.Finish();
        }
    }
}
