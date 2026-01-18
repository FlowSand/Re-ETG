using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

[InspectorDropdownName("ManfredsRival/Magic1")]
public class ManfredsRivalMagic1 : Script
  {
    private const int NumTimes = 4;
    private const int NumBulletsMainWave = 16 /*0x10*/;

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new ManfredsRivalMagic1__Topc__Iterator0()
      {
        _this = this
      };
    }

    protected void FireCluster(float aim)
    {
      int num = (int) AkSoundEngine.PostEvent("Play_ENM_cannonarmor_blast_01", this.BulletBank.gameObject);
      this.Fire(new Offset(0.5f, rotation: aim, transform: string.Empty), new Brave.BulletScript.Direction(aim), new Brave.BulletScript.Speed(11f));
      this.Fire(new Offset(0.25f, 0.3f, aim, string.Empty), new Brave.BulletScript.Direction(aim), new Brave.BulletScript.Speed(11f));
      this.Fire(new Offset(0.25f, -0.3f, aim, string.Empty), new Brave.BulletScript.Direction(aim), new Brave.BulletScript.Speed(11f));
      this.Fire(new Offset(y: 0.4f, rotation: aim, transform: string.Empty), new Brave.BulletScript.Direction(aim), new Brave.BulletScript.Speed(11f));
      this.Fire(new Offset(y: -0.4f, rotation: aim, transform: string.Empty), new Brave.BulletScript.Direction(aim), new Brave.BulletScript.Speed(11f));
    }
  }

