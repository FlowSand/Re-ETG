// Decompiled with JetBrains decompiler
// Type: DungeonPlaceableUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using UnityEngine;

#nullable disable
public static class DungeonPlaceableUtility
{
  public static GameObject InstantiateDungeonPlaceable(
    GameObject objectToInstantiate,
    RoomHandler targetRoom,
    IntVector2 location,
    bool deferConfiguration,
    AIActor.AwakenAnimationType awakenAnimType = AIActor.AwakenAnimationType.Default,
    bool autoEngage = false)
  {
    if (!((Object) objectToInstantiate != (Object) null))
      return (GameObject) null;
    Vector3 position = location.ToVector3(0.0f) + targetRoom.area.basePosition.ToVector3();
    position.z = position.y + position.z;
    AIActor component1 = objectToInstantiate.GetComponent<AIActor>();
    if (component1 is AIActorDummy)
    {
      objectToInstantiate = (component1 as AIActorDummy).realPrefab;
      component1 = objectToInstantiate.GetComponent<AIActor>();
    }
    SpeculativeRigidbody component2 = objectToInstantiate.GetComponent<SpeculativeRigidbody>();
    if ((bool) (Object) component1 && (bool) (Object) component2)
    {
      PixelCollider pixelCollider = component2.GetPixelCollider(ColliderType.Ground);
      if (pixelCollider.ColliderGenerationMode != PixelCollider.PixelColliderGeneration.Manual)
        Debug.LogErrorFormat("Trying to spawn an AIActor who doesn't have a manual ground collider... do we still do this? Name: {0}", (object) objectToInstantiate.name);
      Vector2 unit1 = PhysicsEngine.PixelToUnit(new IntVector2(pixelCollider.ManualOffsetX, pixelCollider.ManualOffsetY));
      Vector2 unit2 = PhysicsEngine.PixelToUnit(new IntVector2(pixelCollider.ManualWidth, pixelCollider.ManualHeight));
      Vector2 vector2 = new Vector2((float) (((double) new Vector2((float) Mathf.CeilToInt(unit2.x), (float) Mathf.CeilToInt(unit2.y)).x - (double) unit2.x) / 2.0), 0.0f).Quantize(1f / 16f);
      position -= (Vector3) (unit1 - vector2);
    }
    if ((bool) (Object) component1)
      component1.AwakenAnimType = awakenAnimType;
    GameObject gameObject = Object.Instantiate<GameObject>(objectToInstantiate, position, Quaternion.identity);
    if (!deferConfiguration)
    {
      foreach (Component componentsInChild in gameObject.GetComponentsInChildren(typeof (IPlaceConfigurable)))
      {
        if (componentsInChild is IPlaceConfigurable placeConfigurable)
          placeConfigurable.ConfigureOnPlacement(targetRoom);
      }
    }
    ObjectVisibilityManager component3 = gameObject.GetComponent<ObjectVisibilityManager>();
    if ((Object) component3 != (Object) null)
      component3.Initialize(targetRoom, autoEngage);
    if ((Object) gameObject.GetComponentInChildren<MinorBreakable>() != (Object) null)
    {
      CellData cellData = GameManager.Instance.Dungeon.data[location + targetRoom.area.basePosition];
      if (cellData != null)
        cellData.cellVisualData.containsObjectSpaceStamp = true;
    }
    PlayerItem component4 = gameObject.GetComponent<PlayerItem>();
    if ((Object) component4 != (Object) null)
      component4.ForceAsExtant = true;
    return gameObject;
  }

  public static GameObject InstantiateDungeonPlaceableOnlyActors(
    GameObject objectToInstantiate,
    RoomHandler targetRoom,
    IntVector2 location,
    bool deferConfiguration)
  {
    bool component1 = (bool) (Object) objectToInstantiate.GetComponent<AIActor>();
    bool flag = GameManager.Instance.InTutorial && (bool) (Object) objectToInstantiate.GetComponent<TalkDoerLite>();
    bool component2 = (bool) (Object) objectToInstantiate.GetComponent<GenericIntroDoer>();
    if (!component1 && !flag && !component2)
      return (GameObject) null;
    GameObject gameObject = DungeonPlaceableUtility.InstantiateDungeonPlaceable(objectToInstantiate, targetRoom, location, deferConfiguration);
    AIActor component3 = gameObject.GetComponent<AIActor>();
    if ((bool) (Object) component3)
    {
      component3.CanDropCurrency = false;
      component3.CanDropItems = false;
    }
    return gameObject;
  }
}
