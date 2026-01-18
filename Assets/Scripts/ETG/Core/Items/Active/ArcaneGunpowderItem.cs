#nullable disable

public class ArcaneGunpowderItem : PlayerItem
  {
    public override bool CanBeUsed(PlayerController user) => false;

    protected override void DoEffect(PlayerController user)
    {
    }

    protected override void OnDestroy() => base.OnDestroy();
  }

