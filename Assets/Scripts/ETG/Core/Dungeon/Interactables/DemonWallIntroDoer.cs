using UnityEngine;

#nullable disable

[RequireComponent(typeof (GenericIntroDoer))]
public class DemonWallIntroDoer : SpecificIntroDoer
  {
    public string preIntro;

    protected override void OnDestroy() => base.OnDestroy();

    public override Vector2? OverrideOutroPosition
    {
      get => new Vector2?(this.GetComponent<DemonWallController>().CameraPos);
    }

    public override void OnCameraIntro() => this.aiAnimator.PlayUntilCancelled(this.preIntro);

    public override void OnCleanup() => this.aiAnimator.EndAnimation();

    public override void EndIntro() => this.GetComponent<DemonWallController>().ModifyCamera(true);
  }

