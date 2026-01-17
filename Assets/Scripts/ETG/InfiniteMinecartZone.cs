// Decompiled with JetBrains decompiler
// Type: InfiniteMinecartZone
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class InfiniteMinecartZone : DungeonPlaceableBehaviour
{
  public static bool InInfiniteMinecartZone;
  public int RegionWidth = 10;
  public int RegionHeight = 3;
  private int m_remainingLoops = 10;
  private bool processed;

  public void Start()
  {
  }

  private void Update()
  {
    if (this.IsPlayerInRegion() && this.m_remainingLoops > 0)
    {
      InfiniteMinecartZone.InInfiniteMinecartZone = true;
      PlayerController primaryPlayer = GameManager.Instance.PrimaryPlayer;
      if (!this.processed && primaryPlayer.IsInMinecart)
      {
        ParticleSystem componentInChildren1 = primaryPlayer.currentMineCart.Sparks_A.GetComponentInChildren<ParticleSystem>();
        ParticleSystem componentInChildren2 = primaryPlayer.currentMineCart.Sparks_B.GetComponentInChildren<ParticleSystem>();
        componentInChildren1.simulationSpace = ParticleSystemSimulationSpace.Local;
        componentInChildren2.simulationSpace = ParticleSystemSimulationSpace.Local;
      }
      IntVector2 intVector2 = this.transform.position.IntXY();
      IntRect intRect = new IntRect(intVector2.x, intVector2.y, this.RegionWidth, this.RegionHeight);
      if ((double) primaryPlayer.CenterPosition.x <= (double) intVector2.x + (double) this.RegionWidth * 0.75 || !primaryPlayer.IsInMinecart)
        return;
      --this.m_remainingLoops;
      Vector2 vector2_1 = GameManager.Instance.MainCameraController.transform.position.XY() - primaryPlayer.currentMineCart.transform.position.XY();
      PathMover component = primaryPlayer.currentMineCart.GetComponent<PathMover>();
      Vector2 vector2_2 = component.transform.position.XY();
      component.WarpToNearestPoint(intVector2.ToVector2() + new Vector2(0.0f, (float) this.RegionHeight / 2f));
      Vector2 vector2_3 = component.transform.position.XY() - vector2_2;
      for (int index = 0; index < primaryPlayer.orbitals.Count; ++index)
      {
        primaryPlayer.orbitals[index].GetTransform().position += vector2_3.ToVector3ZisY();
        if (primaryPlayer.orbitals[index] is PlayerOrbital)
          (primaryPlayer.orbitals[index] as PlayerOrbital).ReinitializeWithDelta(vector2_3);
        else
          primaryPlayer.orbitals[index].Reinitialize();
      }
      for (int index = 0; index < primaryPlayer.trailOrbitals.Count; ++index)
      {
        primaryPlayer.trailOrbitals[index].transform.position = primaryPlayer.trailOrbitals[index].transform.position + vector2_3.ToVector3ZisY();
        primaryPlayer.trailOrbitals[index].specRigidbody.Reinitialize();
      }
      primaryPlayer.currentMineCart.ForceUpdatePositions();
      GameManager.Instance.MainCameraController.transform.position = (primaryPlayer.currentMineCart.transform.position.XY() + vector2_1).ToVector3ZUp(GameManager.Instance.MainCameraController.transform.position.z);
    }
    else
      InfiniteMinecartZone.InInfiniteMinecartZone = false;
  }

  private bool IsPlayerInRegion()
  {
    IntVector2 intVector2 = this.transform.position.IntXY();
    return new IntRect(intVector2.x, intVector2.y, this.RegionWidth, this.RegionHeight).Contains(GameManager.Instance.PrimaryPlayer.CenterPosition);
  }
}
