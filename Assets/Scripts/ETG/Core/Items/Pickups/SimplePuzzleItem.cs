// Decompiled with JetBrains decompiler
// Type: SimplePuzzleItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

public class SimplePuzzleItem : PickupObject
  {
    private bool m_pickedUp;

    private void Start()
    {
      this.specRigidbody.OnTriggerCollision += new SpeculativeRigidbody.OnTriggerDelegate(this.OnPreCollision);
    }

    private void OnPreCollision(
      SpeculativeRigidbody otherRigidbody,
      SpeculativeRigidbody source,
      CollisionData collisionData)
    {
      if (this.m_pickedUp)
        return;
      PlayerController component = otherRigidbody.GetComponent<PlayerController>();
      if (!((Object) component != (Object) null))
        return;
      this.Pickup(component);
      int num = (int) AkSoundEngine.PostEvent("Play_OBJ_item_pickup_01", this.gameObject);
    }

    public override void Pickup(PlayerController player)
    {
      if (this.m_pickedUp)
        return;
      this.m_pickedUp = true;
      this.specRigidbody.enabled = false;
      this.renderer.enabled = false;
      DebrisObject component = this.GetComponent<DebrisObject>();
      if ((Object) component != (Object) null)
      {
        Object.Destroy((Object) component);
        Object.Destroy((Object) this.specRigidbody);
        player.AcquirePuzzleItem((PickupObject) this);
      }
      else
      {
        Object.Instantiate<GameObject>(this.gameObject);
        player.AcquirePuzzleItem((PickupObject) this);
      }
    }

    protected override void OnDestroy() => base.OnDestroy();
  }

