// Decompiled with JetBrains decompiler
// Type: BreakableSprite
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class BreakableSprite : BraveBehaviour
    {
      public bool animations = true;
      public BreakFrame[] breakFrames;

      public void Start()
      {
        if (!(bool) (Object) this.healthHaver)
          return;
        this.healthHaver.OnDamaged += new HealthHaver.OnDamagedEvent(this.OnHealthHaverDamaged);
      }

      protected override void OnDestroy() => base.OnDestroy();

      private void OnHealthHaverDamaged(
        float resultValue,
        float maxValue,
        CoreDamageTypes damageTypes,
        DamageCategory damageCategory,
        Vector2 damageDirection)
      {
        for (int index = this.breakFrames.Length - 1; index >= 0; --index)
        {
          if ((double) resultValue / (double) maxValue <= (double) this.breakFrames[index].healthPercentage / 100.0)
          {
            string sprite = this.breakFrames[index].sprite;
            if (this.animations)
            {
              this.spriteAnimator.Play(sprite);
              break;
            }
            this.sprite.SetSprite(this.breakFrames[index].sprite);
            break;
          }
        }
      }
    }

}
