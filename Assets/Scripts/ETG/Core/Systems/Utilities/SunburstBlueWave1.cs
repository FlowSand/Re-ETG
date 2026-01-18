using Brave.BulletScript;
using FullInspector;
using System.Collections;

#nullable disable

[InspectorDropdownName("Sunburst/BlueWave1")]
public class SunburstBlueWave1 : Script
  {
    protected override IEnumerator Top()
    {
      float aimDirection = this.AimDirection;
      this.Fire(new Offset(y: 0.66f, rotation: aimDirection, transform: string.Empty), new Brave.BulletScript.Direction(aimDirection), new Brave.BulletScript.Speed(9f));
      this.Fire(new Offset(0.66f, rotation: aimDirection, transform: string.Empty), new Brave.BulletScript.Direction(aimDirection), new Brave.BulletScript.Speed(9f));
      this.Fire(new Offset(y: -0.66f, rotation: aimDirection, transform: string.Empty), new Brave.BulletScript.Direction(aimDirection), new Brave.BulletScript.Speed(9f));
      return (IEnumerator) null;
    }
  }

