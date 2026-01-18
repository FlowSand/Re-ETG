using UnityEngine.SceneManagement;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Restarts current level.")]
  [ActionCategory(ActionCategory.Level)]
  public class RestartLevel : FsmStateAction
  {
    public override void OnEnter()
    {
      SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
      this.Finish();
    }
  }
}
