using UnityEngine;

#nullable disable

public class NonActor : GameActor
  {
    public override void Awake()
    {
    }

    public override Gun CurrentGun => (Gun) null;

    public override Transform GunPivot => (Transform) null;

    public override Vector3 SpriteDimensions => Vector3.zero;

    public override bool SpriteFlipped => false;

    public override void Update()
    {
    }
  }

