using System.Collections.Generic;

using UnityEngine;

using Dungeonator;

#nullable disable

public class GenericDecorator : BraveBehaviour, IPlaceConfigurable
    {
        public GenericLootTable tableTable;
        public IntVector2 localPixelSurfaceOrigin;
        public IntVector2 localPixelSurfaceDimensions;
        public float heightOffGround = 0.1f;
        public bool disableRigidbodies = true;
        public DebrisObject rigidbodyTrigger;
        public tk2dSprite parentSprite;
        [HideInInspector]
        public SurfaceDecorator parentSurface;
        private List<SpeculativeRigidbody> m_srbs = new List<SpeculativeRigidbody>();

        public void ConfigureOnPlacement(RoomHandler room)
        {
            this.Decorate();
            if (!this.disableRigidbodies || !((UnityEngine.Object) this.rigidbodyTrigger != (UnityEngine.Object) null))
                return;
            this.rigidbodyTrigger.OnTriggered += new System.Action(this.EnableRigidbodies);
        }

        private GameObject GetSurfaceObject(
            Vector2 availableSpace,
            out Vector2 objectDimensions,
            out Vector2 localOrigin)
        {
            List<GameObject> extant = new List<GameObject>();
            bool flag = false;
            int num = 0;
            while (!flag && num < 1000)
            {
                GameObject surfaceObject = this.tableTable.SelectByWeightWithoutDuplicates(extant);
                if (!((UnityEngine.Object) surfaceObject == (UnityEngine.Object) null))
                {
                    extant.Add(surfaceObject);
                    MinorBreakableGroupManager component = surfaceObject.GetComponent<MinorBreakableGroupManager>();
                    if ((UnityEngine.Object) component != (UnityEngine.Object) null)
                    {
                        objectDimensions = component.GetDimensions();
                        if (!(objectDimensions == Vector2.zero))
                            localOrigin = Vector2.zero;
                        else
                            continue;
                    }
                    else
                    {
                        Bounds bounds = surfaceObject.GetComponent<tk2dSprite>().GetBounds();
                        objectDimensions = new Vector2(bounds.size.x, bounds.size.y);
                        localOrigin = bounds.min.XY();
                    }
                    if ((double) objectDimensions.x <= (double) availableSpace.x && (double) objectDimensions.y <= (double) availableSpace.y)
                        return surfaceObject;
                    ++num;
                }
                else
                    break;
            }
            objectDimensions = new Vector2(float.MaxValue, float.MaxValue);
            localOrigin = Vector2.zero;
            return (GameObject) null;
        }

        public void EnableRigidbodies()
        {
            for (int index = 0; index < this.m_srbs.Count; ++index)
            {
                if ((bool) (UnityEngine.Object) this.m_srbs[index])
                    this.m_srbs[index].enabled = true;
            }
        }

        private void PostProcessObject(GameObject placedObject)
        {
            MinorBreakableGroupManager component1 = placedObject.GetComponent<MinorBreakableGroupManager>();
            if ((UnityEngine.Object) component1 != (UnityEngine.Object) null)
            {
                component1.Initialize();
                tk2dSprite[] componentsInChildren1 = component1.GetComponentsInChildren<tk2dSprite>();
                for (int index = 0; index < componentsInChildren1.Length; ++index)
                {
                    if ((UnityEngine.Object) componentsInChildren1[index].attachParent == (UnityEngine.Object) null)
                    {
                        componentsInChildren1[index].HeightOffGround = this.heightOffGround + 0.1f;
                        this.parentSprite.AttachRenderer((tk2dBaseSprite) componentsInChildren1[index]);
                    }
                }
                MinorBreakable[] componentsInChildren2 = component1.GetComponentsInChildren<MinorBreakable>();
                for (int index = 0; index < componentsInChildren2.Length; ++index)
                {
                    componentsInChildren2[index].heightOffGround = 0.75f;
                    componentsInChildren2[index].isImpermeableToGameActors = true;
                    componentsInChildren2[index].specRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.BulletBreakable;
                }
                DebrisObject[] componentsInChildren3 = component1.GetComponentsInChildren<DebrisObject>();
                for (int index = 0; index < componentsInChildren3.Length; ++index)
                {
                    componentsInChildren3[index].InitializeForCollisions();
                    componentsInChildren3[index].additionalHeightBoost = 0.25f;
                }
                if (this.disableRigidbodies && (UnityEngine.Object) this.rigidbodyTrigger != (UnityEngine.Object) null)
                {
                    SpeculativeRigidbody[] componentsInChildren4 = component1.GetComponentsInChildren<SpeculativeRigidbody>(true);
                    for (int index = 0; index < componentsInChildren4.Length; ++index)
                    {
                        this.m_srbs.Add(componentsInChildren4[index]);
                        componentsInChildren4[index].enabled = false;
                    }
                }
            }
            else
            {
                tk2dSprite component2 = placedObject.GetComponent<tk2dSprite>();
                if ((UnityEngine.Object) component2.attachParent == (UnityEngine.Object) null)
                {
                    component2.HeightOffGround = this.heightOffGround + 0.1f;
                    this.parentSprite.AttachRenderer((tk2dBaseSprite) component2);
                }
                MinorBreakable component3 = placedObject.GetComponent<MinorBreakable>();
                if ((UnityEngine.Object) component3 != (UnityEngine.Object) null)
                {
                    component3.heightOffGround = 0.75f;
                    component3.isImpermeableToGameActors = true;
                    component3.specRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.BulletBreakable;
                }
                DebrisObject component4 = placedObject.GetComponent<DebrisObject>();
                if ((UnityEngine.Object) component4 != (UnityEngine.Object) null)
                {
                    component4.InitializeForCollisions();
                    component4.additionalHeightBoost = 0.25f;
                }
                if (this.disableRigidbodies && (UnityEngine.Object) this.rigidbodyTrigger != (UnityEngine.Object) null)
                {
                    SpeculativeRigidbody component5 = placedObject.GetComponent<SpeculativeRigidbody>();
                    if ((UnityEngine.Object) component5 != (UnityEngine.Object) null)
                    {
                        this.m_srbs.Add(component5);
                        component5.enabled = false;
                    }
                }
            }
            if ((UnityEngine.Object) this.parentSurface != (UnityEngine.Object) null)
            {
                this.parentSurface.RegisterAdditionalObject(placedObject);
                if (!((UnityEngine.Object) this.parentSurface.sprite != (UnityEngine.Object) null))
                    return;
                this.parentSurface.sprite.UpdateZDepth();
            }
            else
                this.parentSprite.UpdateZDepth();
        }

        public void Decorate()
        {
            if ((UnityEngine.Object) this.parentSprite == (UnityEngine.Object) null)
                this.parentSprite = this.GetComponent<tk2dSprite>();
            if ((UnityEngine.Object) this.tableTable == (UnityEngine.Object) null)
            {
                BraveUtility.Log($"Trying to decorate a SurfaceDecorator at: {this.gameObject.name} and failing.", Color.red, BraveUtility.LogVerbosity.CHATTY);
            }
            else
            {
                Vector2 unit1 = PhysicsEngine.PixelToUnit(this.localPixelSurfaceOrigin);
                Vector2 unit2 = PhysicsEngine.PixelToUnit(this.localPixelSurfaceDimensions);
                bool flag = (double) unit2.x >= (double) unit2.y;
                float num1 = 0.0f;
                float num2 = 0.0f;
                float num3 = !flag ? unit2.y : unit2.x;
                float num4 = !flag ? unit2.x : unit2.y;
                float num5 = !flag ? unit1.y : unit1.x;
                float num6 = !flag ? unit1.x : unit1.y;
                float a;
                for (; (double) num1 < (double) num3; num1 += a)
                {
                    float num7 = num3 - num1;
                    a = 0.0f;
                    float num8;
                    for (float num9 = 0.0f; (double) num9 < (double) num4; num9 += num8)
                    {
                        float num10 = num4 - num9;
                        Vector2 objectDimensions = Vector2.zero;
                        Vector2 localOrigin = Vector2.zero;
                        GameObject surfaceObject = this.GetSurfaceObject(!flag ? new Vector2(num10, num7) : new Vector2(num7, num10), out objectDimensions, out localOrigin);
                        if ((UnityEngine.Object) surfaceObject == (UnityEngine.Object) null)
                        {
                            num1 = num3;
                            num2 = num4;
                            break;
                        }
                        float b = !flag ? objectDimensions.y : objectDimensions.x;
                        num8 = !flag ? objectDimensions.x : objectDimensions.y;
                        if ((double) b <= (double) num7 && (double) num8 <= (double) num10)
                        {
                            Vector3 vector3 = !flag ? this.transform.position + new Vector3(num6 + num9, num5 + num1, -0.5f) : this.transform.position + new Vector3(num5 + num1, num6 + num9, -0.5f);
                            this.PostProcessObject(UnityEngine.Object.Instantiate<GameObject>(surfaceObject, vector3 - localOrigin.ToVector3ZUp(), Quaternion.identity));
                        }
                        a = Mathf.Max(a, b);
                    }
                }
            }
        }

        protected override void OnDestroy() => base.OnDestroy();
    }

