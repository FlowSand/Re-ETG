// Decompiled with JetBrains decompiler
// Type: FoyerFloorController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Dungeon.Rooms
{
    public class FoyerFloorController : MonoBehaviour
    {
      public string FinalFormSpriteName;
      public string IntermediateFormSpriteName;
      public string BaseSpriteName;
      public tk2dSprite PitSprite;
      public string FinalPitName;
      public string IntermediatePitName;
      public string BasePitName;

      [DebuggerHidden]
      private IEnumerator Start()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new FoyerFloorController.<Start>c__Iterator0()
        {
          _this = this
        };
      }
    }

}
