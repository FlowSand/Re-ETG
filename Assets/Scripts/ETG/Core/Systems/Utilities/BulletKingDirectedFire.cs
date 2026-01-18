using Brave.BulletScript;

#nullable disable

  public abstract class BulletKingDirectedFire : Script
  {
    public bool IsHard => this is BulletKingDirectedFireHard;

    protected void DirectedShots(float x, float y, float direction)
    {
      direction -= 90f;
      if (this.IsHard)
        direction += 15f;
      this.Fire(new Offset(x, y, transform: string.Empty), new Brave.BulletScript.Direction(direction), new Brave.BulletScript.Speed(!this.IsHard ? 12f : 16f), new Bullet("directedfire"));
      if (!this.IsHard)
        return;
      direction += 30f;
      this.Fire(new Offset(x, y, transform: string.Empty), new Brave.BulletScript.Direction(direction), new Brave.BulletScript.Speed(!this.IsHard ? 12f : 16f), new Bullet("directedfire"));
    }
  }

