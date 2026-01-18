#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Starts location service updates. Last location coordinates can be retrieved with GetLocationInfo.")]
  [ActionCategory(ActionCategory.Device)]
  public class StartLocationServiceUpdates : FsmStateAction
  {
    [Tooltip("Maximum time to wait in seconds before failing.")]
    public FsmFloat maxWait;
    public FsmFloat desiredAccuracy;
    public FsmFloat updateDistance;
    [Tooltip("Event to send when the location services have started.")]
    public FsmEvent successEvent;
    [Tooltip("Event to send if the location services fail to start.")]
    public FsmEvent failedEvent;

    public override void Reset()
    {
      this.maxWait = (FsmFloat) 20f;
      this.desiredAccuracy = (FsmFloat) 10f;
      this.updateDistance = (FsmFloat) 10f;
      this.successEvent = (FsmEvent) null;
      this.failedEvent = (FsmEvent) null;
    }

    public override void OnEnter() => this.Finish();

    public override void OnUpdate()
    {
    }
  }
}
