// Decompiled with JetBrains decompiler
// Type: EvolutionaryGunController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class EvolutionaryGunController : MonoBehaviour
    {
      [PickupIdentifier]
      public int EvoStage01ID = -1;
      [PickupIdentifier]
      public int EvoStage02ID = -1;
      [PickupIdentifier]
      public int EvoStage03ID = -1;
      [PickupIdentifier]
      public int EvoStage04ID = -1;
      [PickupIdentifier]
      public int EvoStage05ID = -1;
      [PickupIdentifier]
      public int EvoStage06ID = -1;
      private Gun m_gun;
    }

}
