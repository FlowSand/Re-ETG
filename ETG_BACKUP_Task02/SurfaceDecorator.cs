// Decompiled with JetBrains decompiler
// Type: SurfaceDecorator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable
public class SurfaceDecorator : BraveBehaviour
{
  public GenericLootTable tableTable;
  public float chanceToDecorate = 1f;
  public IntVector2 localSurfaceOrigin;
  public IntVector2 localSurfaceDimensions;
  public float heightOffGround = 0.5f;
  public tk2dSprite parentSprite;
  private List<GameObject> m_surfaceObjects;
  private List<tk2dSprite> m_attachedSprites;
  private RoomHandler m_parentRoom;
  private bool m_destabilized;
  private const int PRIMARY_PIXEL_BUFFER = 1;

  public bool IsDestabilized => this.m_destabilized;

  private SurfaceDecorator.ObjectPlacementData GetSurfaceObject(
    Vector2 availableSpace,
    out Vector2 objectDimensions,
    out Vector2 localOrigin)
  {
    List<GameObject> extant = new List<GameObject>();
    bool flag1 = false;
    int num = 0;
    GenericLootTable genericLootTable = this.tableTable;
    if ((Object) this.m_parentRoom.RoomMaterial.overrideTableTable != (Object) null)
      genericLootTable = this.m_parentRoom.RoomMaterial.overrideTableTable;
    while (!flag1 && num < 1000)
    {
      GameObject gameObject = genericLootTable.SelectByWeightWithoutDuplicates(extant);
      if (!((Object) gameObject == (Object) null))
      {
        extant.Add(gameObject);
        DebrisObject component1 = gameObject.GetComponent<DebrisObject>();
        MinorBreakableGroupManager component2 = gameObject.GetComponent<MinorBreakableGroupManager>();
        if ((Object) component2 != (Object) null)
        {
          objectDimensions = component2.GetDimensions();
          if (!(objectDimensions == Vector2.zero))
            localOrigin = Vector2.zero;
          else
            continue;
        }
        else
        {
          Bounds bounds = gameObject.GetComponent<tk2dSprite>().GetBounds();
          objectDimensions = new Vector2(bounds.size.x, bounds.size.y);
          localOrigin = bounds.min.XY();
        }
        bool flag2 = (double) objectDimensions.x <= (double) availableSpace.x && (double) objectDimensions.y <= (double) availableSpace.y;
        bool flag3 = (Object) component1 != (Object) null && component1.placementOptions.canBeRotated && (double) objectDimensions.x <= (double) availableSpace.y && (double) objectDimensions.y <= (double) availableSpace.x;
        if (flag2 || flag3)
        {
          SurfaceDecorator.ObjectPlacementData surfaceObject = new SurfaceDecorator.ObjectPlacementData(gameObject);
          surfaceObject.rotated = !flag2 || !flag3 ? !flag2 : (double) Random.value > 0.5;
          if (surfaceObject.rotated)
          {
            objectDimensions = new Vector2(objectDimensions.y, objectDimensions.x);
            localOrigin = new Vector2(localOrigin.y, localOrigin.x);
          }
          if ((Object) component1 != (Object) null && component1.placementOptions.canBeFlippedHorizontally)
            surfaceObject.horizontalFlip = (double) Random.value > 0.5;
          if ((Object) component1 != (Object) null && component1.placementOptions.canBeFlippedVertically)
            surfaceObject.verticalFlip = (double) Random.value > 0.5;
          return surfaceObject;
        }
        ++num;
      }
      else
        break;
    }
    objectDimensions = new Vector2(float.MaxValue, float.MaxValue);
    localOrigin = Vector2.zero;
    return (SurfaceDecorator.ObjectPlacementData) null;
  }

  public void RegisterAdditionalObject(GameObject o)
  {
    if (this.m_surfaceObjects.Contains(o))
      return;
    tk2dSprite[] componentsInChildren = o.GetComponentsInChildren<tk2dSprite>();
    for (int index = 0; index < componentsInChildren.Length; ++index)
    {
      if (!this.m_attachedSprites.Contains(componentsInChildren[index]) && (Object) componentsInChildren[index].attachParent == (Object) null)
      {
        componentsInChildren[index].HeightOffGround = 0.1f;
        this.m_attachedSprites.Add(componentsInChildren[index]);
      }
    }
    this.m_surfaceObjects.Add(o);
  }

  private void PostProcessObject(
    GameObject placedObject,
    SurfaceDecorator.ObjectPlacementData placementData)
  {
    MinorBreakableGroupManager component1 = placedObject.GetComponent<MinorBreakableGroupManager>();
    if ((Object) component1 != (Object) null)
    {
      component1.Initialize();
      tk2dSprite[] componentsInChildren1 = component1.GetComponentsInChildren<tk2dSprite>();
      for (int index = 0; index < componentsInChildren1.Length; ++index)
      {
        if ((Object) componentsInChildren1[index].attachParent == (Object) null)
        {
          componentsInChildren1[index].HeightOffGround = 0.1f;
          this.parentSprite.AttachRenderer((tk2dBaseSprite) componentsInChildren1[index]);
          this.m_attachedSprites.Add(componentsInChildren1[index]);
        }
      }
      MinorBreakable[] componentsInChildren2 = component1.GetComponentsInChildren<MinorBreakable>();
      for (int index = 0; index < componentsInChildren2.Length; ++index)
      {
        componentsInChildren2[index].IgnoredForPotShotsModifier = true;
        componentsInChildren2[index].heightOffGround = 0.75f;
        componentsInChildren2[index].isImpermeableToGameActors = true;
        componentsInChildren2[index].parentSurface = this;
        componentsInChildren2[index].specRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.BulletBreakable;
      }
      DebrisObject[] componentsInChildren3 = component1.GetComponentsInChildren<DebrisObject>();
      for (int index = 0; index < componentsInChildren3.Length; ++index)
      {
        componentsInChildren3[index].InitializeForCollisions();
        componentsInChildren3[index].additionalHeightBoost = 0.25f;
      }
    }
    else
    {
      tk2dSprite component2 = placedObject.GetComponent<tk2dSprite>();
      component2.HeightOffGround = 0.1f;
      if (!placementData.rotated)
      {
        if (placementData.horizontalFlip)
        {
          Vector2 vector2 = component2.GetBounds().min.XY();
          component2.FlipX = true;
          Vector2 vector = component2.GetBounds().min.XY() - vector2;
          component2.transform.position = component2.transform.position - vector.ToVector3ZUp();
        }
        if (placementData.verticalFlip)
        {
          Vector2 vector2 = component2.GetBounds().min.XY();
          component2.FlipY = true;
          Vector2 vector = component2.GetBounds().min.XY() - vector2;
          component2.transform.position = component2.transform.position - vector.ToVector3ZUp();
        }
      }
      this.parentSprite.AttachRenderer((tk2dBaseSprite) component2);
      this.m_attachedSprites.Add(component2);
      MinorBreakable component3 = placedObject.GetComponent<MinorBreakable>();
      if ((Object) component3 != (Object) null)
      {
        component3.IgnoredForPotShotsModifier = true;
        component3.heightOffGround = 0.75f;
        component3.isImpermeableToGameActors = true;
        component3.parentSurface = this;
        component3.specRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.BulletBreakable;
      }
      DebrisObject component4 = placedObject.GetComponent<DebrisObject>();
      if ((Object) component4 != (Object) null)
      {
        component4.InitializeForCollisions();
        component4.additionalHeightBoost = 0.25f;
      }
    }
    GenericDecorator[] componentsInChildren = placedObject.GetComponentsInChildren<GenericDecorator>();
    for (int index = 0; index < componentsInChildren.Length; ++index)
    {
      componentsInChildren[index].parentSurface = this;
      componentsInChildren[index].ConfigureOnPlacement(this.m_parentRoom);
    }
  }

  protected override void OnDestroy() => base.OnDestroy();

  public void Decorate(RoomHandler parentRoom)
  {
    if (GameManager.Options.DebrisQuantity == GameOptions.GenericHighMedLowOption.VERY_LOW)
      return;
    IntVector2 intVector2 = this.transform.position.IntXY();
    if (GameManager.Instance.Dungeon.data.CheckInBounds(intVector2) && GameManager.Instance.Dungeon.data[intVector2].cellVisualData.containsObjectSpaceStamp && GameManager.Instance.Dungeon.data[intVector2].cellVisualData.containsWallSpaceStamp || (double) Random.value >= (double) this.chanceToDecorate)
      return;
    this.m_parentRoom = parentRoom;
    if ((Object) this.parentSprite == (Object) null)
      this.parentSprite = this.GetComponent<tk2dSprite>();
    if ((Object) this.tableTable == (Object) null)
    {
      BraveUtility.Log($"Trying to decorate a SurfaceDecorator at: {this.gameObject.name} and failing.", Color.red, BraveUtility.LogVerbosity.CHATTY);
    }
    else
    {
      this.m_surfaceObjects = new List<GameObject>();
      this.m_attachedSprites = new List<tk2dSprite>();
      bool flag = this.localSurfaceDimensions.x >= this.localSurfaceDimensions.y;
      float num1 = 0.0f;
      float num2 = 0.0f;
      float unit1 = PhysicsEngine.PixelToUnit(!flag ? this.localSurfaceDimensions.y : this.localSurfaceDimensions.x);
      float unit2 = PhysicsEngine.PixelToUnit(!flag ? this.localSurfaceDimensions.x : this.localSurfaceDimensions.y);
      float unit3 = PhysicsEngine.PixelToUnit(!flag ? this.localSurfaceOrigin.y : this.localSurfaceOrigin.x);
      float unit4 = PhysicsEngine.PixelToUnit(!flag ? this.localSurfaceOrigin.x : this.localSurfaceOrigin.y);
      float a;
      for (; (double) num1 < (double) unit1; num1 += a)
      {
        float num3 = unit1 - num1;
        a = 0.0f;
        float num4 = 0.0f;
        int p = 0;
        float num5 = 0.0f;
        List<GameObject> gameObjectList = new List<GameObject>();
        while ((double) num4 < (double) unit2)
        {
          float num6 = (float) (1.0 - 1.0 / (double) Mathf.Pow(2f, (float) p));
          float num7 = unit2 - num4;
          Vector2 objectDimensions = Vector2.zero;
          Vector2 localOrigin = Vector2.zero;
          Vector2 availableSpace = !flag ? new Vector2(num7, num3) : new Vector2(num3, num7);
          SurfaceDecorator.ObjectPlacementData placementData = (SurfaceDecorator.ObjectPlacementData) null;
          if ((double) Random.value > (double) num6)
            placementData = this.GetSurfaceObject(availableSpace, out objectDimensions, out localOrigin);
          if (placementData == null)
          {
            num5 = unit2 - num4;
            num2 = unit2;
            break;
          }
          GameObject o = placementData.o;
          float b = !flag ? objectDimensions.y : objectDimensions.x;
          float num8 = !flag ? objectDimensions.x : objectDimensions.y;
          if ((double) b <= (double) num3 && (double) num8 <= (double) num7)
          {
            Vector3 position = (!flag ? this.transform.position + new Vector3(unit4 + num4, unit3 + num1, -0.5f) : this.transform.position + new Vector3(unit3 + num1, unit4 + num4, -0.5f)) - localOrigin.ToVector3ZUp();
            if (placementData.rotated)
              position += new Vector3(objectDimensions.x, 0.0f, 0.0f);
            GameObject placedObject = SpawnManager.SpawnDebris(o, position, !placementData.rotated ? Quaternion.identity : Quaternion.Euler(0.0f, 0.0f, 90f));
            this.PostProcessObject(placedObject, placementData);
            this.m_surfaceObjects.Add(placedObject);
            gameObjectList.Add(placedObject);
            ++p;
          }
          a = Mathf.Max(a, b);
          num4 += num8;
          num5 = unit2 - num4;
        }
        if ((double) a == 0.0)
          num1 = unit1;
        else
          a += 1f / (float) PhysicsEngine.Instance.PixelsPerUnit;
        int num9 = Mathf.FloorToInt(num5 / (1f / 16f));
        if (num9 >= 2)
        {
          int num10 = Mathf.FloorToInt((float) num9 / 2f);
          int num11 = Mathf.FloorToInt((float) num9 / 3f);
          int num12 = Random.Range(-1 * num11, num11 + 1);
          IntVector2 offset = !flag ? new IntVector2(num10 + num12, 0) : new IntVector2(0, num10 + num12);
          for (int index = 0; index < gameObjectList.Count; ++index)
            gameObjectList[index].transform.MovePixelsLocal(offset);
        }
      }
      this.parentSprite.UpdateZDepth();
    }
  }

  private float GetForceMultiplier(Vector3 objectPosition, Vector2 forceDirection)
  {
    bool flag = this.localSurfaceDimensions.x >= this.localSurfaceDimensions.y;
    if ((double) Mathf.Abs(forceDirection.x) > (double) Mathf.Abs(forceDirection.y) != flag)
      return 1f;
    Vector2 unit1 = PhysicsEngine.PixelToUnit(this.localSurfaceOrigin);
    Vector2 unit2 = PhysicsEngine.PixelToUnit(this.localSurfaceDimensions);
    Vector2 vector2 = new Vector2((objectPosition.x - (this.transform.position.x + unit1.x)) / unit2.x, (objectPosition.y - (this.transform.position.y + unit1.y)) / unit2.y);
    if ((double) forceDirection.x > 0.0)
      vector2.x = 1f - vector2.x;
    if ((double) forceDirection.y > 0.0)
      vector2.y = 1f - vector2.y;
    return Mathf.Lerp(1f, 1.5f, !flag ? Mathf.Clamp01(vector2.y) : Mathf.Clamp01(vector2.x));
  }

  public void Destabilize(Vector2 direction)
  {
    if (this.m_destabilized)
      return;
    this.m_destabilized = true;
    if ((Object) this.parentSprite == (Object) null)
      this.parentSprite = this.GetComponent<tk2dSprite>();
    Vector2 zero = Vector2.zero;
    float num = 0.0f;
    if ((double) direction.x > 0.0)
    {
      zero += new Vector2(5f, 0.0f);
      num = 0.5f;
    }
    if ((double) direction.x < 0.0)
    {
      zero += new Vector2(-5f, 0.0f);
      num = 0.5f;
    }
    if ((double) direction.y > 0.0)
    {
      zero += new Vector2(0.0f, 9f);
      num = -0.25f;
    }
    if ((double) direction.y < 0.0)
      zero += new Vector2(0.0f, -5f);
    if (this.m_attachedSprites != null)
    {
      for (int index = 0; index < this.m_attachedSprites.Count; ++index)
      {
        if ((bool) (Object) this.m_attachedSprites[index])
        {
          this.parentSprite.DetachRenderer((tk2dBaseSprite) this.m_attachedSprites[index]);
          this.m_attachedSprites[index].attachParent = (tk2dBaseSprite) null;
          this.m_attachedSprites[index].IsPerpendicular = true;
        }
      }
    }
    if (this.m_surfaceObjects == null)
      return;
    for (int index = 0; index < this.m_surfaceObjects.Count; ++index)
    {
      if ((bool) (Object) this.m_surfaceObjects[index])
      {
        float forceMultiplier = this.GetForceMultiplier(this.m_surfaceObjects[index].transform.position, zero);
        MinorBreakableGroupManager component1 = this.m_surfaceObjects[index].GetComponent<MinorBreakableGroupManager>();
        if ((Object) component1 != (Object) null)
        {
          component1.Destabilize(zero.ToVector3ZUp(0.5f) * forceMultiplier, 0.5f + num);
        }
        else
        {
          DebrisObject component2 = this.m_surfaceObjects[index].gameObject.GetComponent<DebrisObject>();
          if ((Object) component2 != (Object) null)
          {
            Vector3 startingForce = Quaternion.Euler(0.0f, 0.0f, Mathf.Lerp(-30f, 30f, Random.value)) * zero.ToVector3ZUp(0.25f + num) * forceMultiplier;
            component2.Trigger(startingForce, 0.5f);
          }
          else
          {
            MinorBreakable component3 = this.m_surfaceObjects[index].GetComponent<MinorBreakable>();
            if ((Object) component3 != (Object) null)
            {
              Vector3 vector = Quaternion.Euler(0.0f, 0.0f, Mathf.Lerp(-30f, 30f, Random.value)) * zero.ToVector3ZUp(0.25f + num) * forceMultiplier;
              component3.destroyOnBreak = true;
              component3.Break(vector.XY());
            }
          }
        }
      }
    }
  }

  [DebuggerHidden]
  private IEnumerator DetachRenderersMomentarily()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new SurfaceDecorator.\u003CDetachRenderersMomentarily\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  internal class ObjectPlacementData
  {
    public GameObject o;
    public bool rotated;
    public bool horizontalFlip;
    public bool verticalFlip;

    public ObjectPlacementData(GameObject obj) => this.o = obj;
  }
}
