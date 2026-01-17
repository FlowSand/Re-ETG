// Decompiled with JetBrains decompiler
// Type: AngryBookBlueWave1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [InspectorDropdownName("AngryBook/BlueWave1")]
    public class AngryBookBlueWave1 : Script
    {
      public int NumBullets = 32 /*0x20*/;

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new AngryBookBlueWave1__Topc__Iterator0()
        {
          _this = this
        };
      }

      public class WaveBullet : Bullet
      {
        public WaveBullet()
          : base()
        {
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new AngryBookBlueWave1.WaveBullet__Topc__Iterator0()
          {
            _this = this
          };
        }
      }
    }

}
