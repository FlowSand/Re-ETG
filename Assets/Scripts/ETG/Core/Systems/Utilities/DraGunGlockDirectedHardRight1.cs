// Decompiled with JetBrains decompiler
// Type: DraGunGlockDirectedHardRight1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [InspectorDropdownName("Bosses/DraGun/GlockDirectedHardRight1")]
    public class DraGunGlockDirectedHardRight1 : DraGunGlockDirected1
    {
      protected override string BulletName => "glockRight";

      protected override bool IsHard => true;
    }

}
