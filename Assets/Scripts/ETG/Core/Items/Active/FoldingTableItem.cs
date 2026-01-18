// Decompiled with JetBrains decompiler
// Type: FoldingTableItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using UnityEngine;

#nullable disable

public class FoldingTableItem : PlayerItem
  {
    public FlippableCover TableToSpawn;

    public override bool CanBeUsed(PlayerController user)
    {
      if (!(bool) (Object) user || user.InExitCell || user.CurrentRoom == null)
        return false;
      Vector2 nearbyPoint = user.CenterPosition + (user.unadjustedAimPoint.XY() - user.CenterPosition).normalized;
      return user.CurrentRoom.GetNearestAvailableCell(nearbyPoint, new IntVector2?(IntVector2.One), new CellTypes?(CellTypes.FLOOR)).HasValue && base.CanBeUsed(user);
    }

    protected override void DoEffect(PlayerController user)
    {
      base.DoEffect(user);
      int num = (int) AkSoundEngine.PostEvent("Play_ITM_Folding_Table_Use_01", this.gameObject);
      Vector2 nearbyPoint = user.CenterPosition + (user.unadjustedAimPoint.XY() - user.CenterPosition).normalized;
      GameObject gameObject = Object.Instantiate<GameObject>(this.TableToSpawn.gameObject, (Vector3) user.CurrentRoom.GetNearestAvailableCell(nearbyPoint, new IntVector2?(IntVector2.One), new CellTypes?(CellTypes.FLOOR)).Value.ToVector2(), Quaternion.identity);
      SpeculativeRigidbody componentInChildren = gameObject.GetComponentInChildren<SpeculativeRigidbody>();
      FlippableCover component = gameObject.GetComponent<FlippableCover>();
      component.transform.position.XY().GetAbsoluteRoom().RegisterInteractable((IPlayerInteractable) component);
      component.ConfigureOnPlacement(component.transform.position.XY().GetAbsoluteRoom());
      componentInChildren.Initialize();
      PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(componentInChildren);
    }
  }

