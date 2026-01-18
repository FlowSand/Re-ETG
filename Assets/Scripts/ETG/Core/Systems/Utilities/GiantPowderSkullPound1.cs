using Brave.BulletScript;
using FullInspector;
using System.Collections;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/GiantPowderSkull/Pound1")]
public class GiantPowderSkullPound1 : Script
  {
    private const float WaveSeparation = 12f;
    private const float OffsetDist = 1f;
    private static int s_lastPatternNum;

    protected override IEnumerator Top()
    {
      int num1 = BraveUtility.SequentialRandomRange(0, 4, GiantPowderSkullPound1.s_lastPatternNum, excludeLastValue: true);
      GiantPowderSkullPound1.s_lastPatternNum = num1;
      switch (num1)
      {
        case 0:
          float num2 = this.AimDirection - 48f;
          for (int index = 0; index < 9; ++index)
          {
            float num3 = num2 + (float) index * 12f;
            this.Fire(new Offset(new Vector2(1f, 0.0f), num3, string.Empty), new Brave.BulletScript.Direction(num3), new Brave.BulletScript.Speed(8f), new Bullet("default_ground"));
          }
          break;
        case 1:
          float num4 = this.AimDirection - 48f + Random.Range(-35f, 35f);
          for (int index = 0; index < 9; ++index)
          {
            float num5 = num4 + (float) index * 12f;
            this.Fire(new Offset(new Vector2(1f, 0.0f), num5, string.Empty), new Brave.BulletScript.Direction(num5), new Brave.BulletScript.Speed(8f), new Bullet("default_ground"));
          }
          break;
        case 2:
          float num6 = this.RandomAngle();
          for (int index = 0; index < 12; ++index)
          {
            float num7 = num6 + (float) index * 30f;
            this.Fire(new Offset(new Vector2(1f, 0.0f), num7, string.Empty), new Brave.BulletScript.Direction(num7), new Brave.BulletScript.Speed(8f), new Bullet("default_ground"));
          }
          break;
        case 3:
          float num8 = this.RandomAngle();
          for (int index = 0; index < 36; ++index)
          {
            float num9 = num8 + (float) index * 10f;
            this.Fire(new Offset(new Vector2(1f, 0.0f), num9, string.Empty), new Brave.BulletScript.Direction(num9), new Brave.BulletScript.Speed(8f), new Bullet("default_ground"));
          }
          break;
      }
      return (IEnumerator) null;
    }
  }

