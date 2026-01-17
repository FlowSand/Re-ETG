// Decompiled with JetBrains decompiler
// Type: AutoAimTarget
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class AutoAimTarget : BraveBehaviour, IAutoAimTarget
    {
      public bool ForceUseTransform;
      public bool IgnoreForSuperAutoAim;
      public float MinDistForSuperAutoAim;
      private RoomHandler parentRoom;

      public void Start()
      {
        this.parentRoom = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(this.AimCenter.ToIntVector2(VectorConversions.Floor));
        this.parentRoom.RegisterAutoAimTarget((IAutoAimTarget) this);
      }

      protected override void OnDestroy()
      {
        if (this.parentRoom != null)
          this.parentRoom.DeregisterAutoAimTarget((IAutoAimTarget) this);
        base.OnDestroy();
      }

      public bool IsValid
      {
        get
        {
          if (!(bool) (Object) this)
            return false;
          return (bool) (Object) this.specRigidbody && !this.ForceUseTransform ? this.specRigidbody.enabled && this.specRigidbody.GetPixelCollider(ColliderType.HitBox) != null : this.enabled && this.gameObject.activeSelf;
        }
      }

      public Vector2 AimCenter
      {
        get
        {
          return (bool) (Object) this.specRigidbody && !this.ForceUseTransform ? this.specRigidbody.GetUnitCenter(ColliderType.HitBox) : this.transform.position.XY();
        }
      }

      public Vector2 Velocity
      {
        get => (bool) (Object) this.specRigidbody ? this.specRigidbody.Velocity : Vector2.zero;
      }

      public bool IgnoreForSuperDuperAutoAim => this.IgnoreForSuperAutoAim;

      public float MinDistForSuperDuperAutoAim => this.MinDistForSuperAutoAim;
    }

}
