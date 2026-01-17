// Decompiled with JetBrains decompiler
// Type: IPlayerOrbital
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public interface IPlayerOrbital
{
  void Reinitialize();

  Transform GetTransform();

  void ToggleRenderer(bool visible);

  int GetOrbitalTier();

  void SetOrbitalTier(int tier);

  int GetOrbitalTierIndex();

  void SetOrbitalTierIndex(int tierIndex);

  float GetOrbitalRadius();

  float GetOrbitalRotationalSpeed();
}
