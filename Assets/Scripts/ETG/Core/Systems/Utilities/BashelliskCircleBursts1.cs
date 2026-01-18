using Brave.BulletScript;
using FullInspector;
using System.Collections;

#nullable disable

[InspectorDropdownName("Bosses/Bashellisk/CircleBursts1")]
public class BashelliskCircleBursts1 : Script
  {
    private const int NumBullets = 17;

    protected override IEnumerator Top()
    {
      float num1 = this.RandomAngle();
      float num2 = 21.17647f;
      for (int index = 0; index < 17; ++index)
        this.Fire(new Brave.BulletScript.Direction(num1 + (float) index * num2), new Brave.BulletScript.Speed(9f), new Bullet("CircleBurst"));
      return (IEnumerator) null;
    }
  }

