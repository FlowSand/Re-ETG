// Decompiled with JetBrains decompiler
// Type: WaftModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    [RequireComponent(typeof (AIActor))]
    public class WaftModifier : BraveBehaviour
    {
      public bool modifierEnabled = true;
      public float waftMagnitude = 1f;
      public float waftFrequency = 3f;
      public bool fleeWalls = true;

      private void Start()
      {
        this.gameActor.MovementModifiers += new GameActor.MovementModifier(this.ModifyVelocity);
      }

      protected override void OnDestroy()
      {
        this.gameActor.MovementModifiers -= new GameActor.MovementModifier(this.ModifyVelocity);
      }

      public void ModifyVelocity(ref Vector2 volundaryVel, ref Vector2 involuntaryVel)
      {
        if (!this.modifierEnabled)
          return;
        Vector2 vector2_1 = new Vector2(-volundaryVel.y, volundaryVel.x).normalized;
        if (volundaryVel == Vector2.zero)
          vector2_1 = Vector2.right;
        float num = Mathf.Sin(UnityEngine.Time.timeSinceLevelLoad * this.waftFrequency) * this.waftMagnitude;
        Vector2 vector2_2 = vector2_1 * num;
        Vector2 vector2_3 = Vector2.zero;
        if (this.fleeWalls && GameManager.Instance.Dungeon.data[this.specRigidbody.UnitBottomCenter.ToIntVector2(VectorConversions.Floor)].isOccludedByTopWall)
          vector2_3 = Vector2.up * this.waftMagnitude;
        volundaryVel += vector2_2 + vector2_3;
      }
    }

}
