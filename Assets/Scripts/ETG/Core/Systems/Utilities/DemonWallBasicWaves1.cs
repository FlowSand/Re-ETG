using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/DemonWall/BasicWaves1")]
public class DemonWallBasicWaves1 : Script
  {
    public static string[][] shootPoints = new string[3][]
    {
      new string[3]{ "sad bullet", "blobulon", "dopey bullet" },
      new string[3]{ "left eye", "right eye", "crashed bullet" },
      new string[4]
      {
        "sideways bullet",
        "shotgun bullet",
        "cultist",
        "angry bullet"
      }
    };
    public const int NumBursts = 10;

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new DemonWallBasicWaves1__Topc__Iterator0()
      {
        _this = this
      };
    }

    private void FireWave(string transform)
    {
      for (int i = 0; i < 7; ++i)
        this.Fire(new Offset(transform), new Brave.BulletScript.Direction(this.SubdivideArc(-125f, 70f, 7, i)), new Brave.BulletScript.Speed(7f), new Bullet("wave", i != 3));
      float aimDirection = this.GetAimDirection(transform);
      if ((double) Random.value >= 0.33300000429153442 || (double) BraveMathCollege.AbsAngleBetween(-90f, aimDirection) >= 45.0)
        return;
      this.Fire(new Offset(transform), new Brave.BulletScript.Direction(aimDirection), new Brave.BulletScript.Speed(7f), new Bullet("wave", true));
    }
  }

