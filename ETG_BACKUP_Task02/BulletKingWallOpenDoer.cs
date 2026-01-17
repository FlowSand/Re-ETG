// Decompiled with JetBrains decompiler
// Type: BulletKingWallOpenDoer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
public class BulletKingWallOpenDoer : BraveBehaviour
{
  private void Start()
  {
    GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(this.transform.position.IntXY(VectorConversions.Floor)).OnEnemiesCleared += new System.Action(this.OnBossKill);
  }

  private void OnBossKill()
  {
    this.specRigidbody.PixelColliders[4].Enabled = false;
    this.specRigidbody.PixelColliders[5].Enabled = false;
    this.spriteAnimator.Play();
    SpawnManager.Instance.ClearRectOfDecals(this.specRigidbody.PixelColliders[4].UnitBottomLeft, this.specRigidbody.PixelColliders[5].UnitTopRight);
  }

  protected override void OnDestroy() => base.OnDestroy();
}
