using Brave.BulletScript;
using FullInspector;
using System.Collections;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/Lich/QuickDrawBurstShot1")]
public class LichQuickDrawBurstShot1 : Script
  {
    protected override IEnumerator Top()
    {
      float aimDirection = this.GetAimDirection((float) Random.Range(0, 3), 12f);
      for (int index = -2; index <= 2; ++index)
        this.Fire(new Brave.BulletScript.Direction(aimDirection + (float) (index * 10)), new Brave.BulletScript.Speed(12f), new Bullet("quickHoming"));
      return (IEnumerator) null;
    }
  }

