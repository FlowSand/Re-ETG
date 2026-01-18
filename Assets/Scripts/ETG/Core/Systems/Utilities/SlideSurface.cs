// Decompiled with JetBrains decompiler
// Type: SlideSurface
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

public class SlideSurface : MonoBehaviour
  {
    private FlippableCover m_table;
    private SurfaceDecorator m_surface;

    public void Awake()
    {
      this.m_table = this.GetComponent<FlippableCover>();
      if (!(bool) (Object) this.m_table && (bool) (Object) this.transform.parent)
        this.m_table = this.transform.parent.GetComponent<FlippableCover>();
      if ((bool) (Object) this.m_table)
        this.m_surface = this.m_table.GetComponent<SurfaceDecorator>();
      this.GetComponent<SpeculativeRigidbody>().OnPreRigidbodyCollision += new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreRigidbodyCollision);
    }

    private void OnPreRigidbodyCollision(
      SpeculativeRigidbody myRigidbody,
      PixelCollider myPixelCollider,
      SpeculativeRigidbody otherRigidbody,
      PixelCollider otherPixelCollider)
    {
      if (!(bool) (Object) otherRigidbody)
        return;
      PlayerController component = otherRigidbody.GetComponent<PlayerController>();
      if (!(bool) (Object) component || component.CurrentRollState != PlayerController.DodgeRollState.InAir || !this.IsAccessible(component))
        return;
      if (!component.IsSlidingOverSurface)
        GameStatsManager.Instance.RegisterStatChange(TrackedStats.TIMES_SLID_OVER_TABLE, 1f);
      component.IsSlidingOverSurface = true;
      PhysicsEngine.SkipCollision = true;
      if (!(bool) (Object) this.m_surface)
        return;
      this.m_surface.Destabilize(component.specRigidbody.Velocity);
    }

    public bool IsAccessible(PlayerController collidingPlayer)
    {
      if (!(bool) (Object) this.m_table)
        return true;
      if (this.m_table.IsFlipped || this.m_table.IsBroken)
        return false;
      return collidingPlayer.IsSlidingOverSurface || true;
    }
  }

