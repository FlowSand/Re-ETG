#nullable disable

public class MimicController : BraveBehaviour
  {
    public void Awake()
    {
      this.aiActor.enabled = false;
      this.aiShooter.enabled = false;
      this.behaviorSpeculator.enabled = false;
    }

    protected override void OnDestroy() => base.OnDestroy();
  }

