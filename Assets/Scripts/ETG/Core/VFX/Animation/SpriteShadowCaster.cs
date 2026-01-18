using System.Collections.Generic;

using UnityEngine;

#nullable disable

public class SpriteShadowCaster : MonoBehaviour
    {
        public float radius = 10f;
        public float shadowDepth = -0.05f;
        public Material material;
        private List<SpriteShadow> m_shadows;
        private Camera m_camera;

        private void Start()
        {
            this.m_camera = GameObject.Find("Main Camera").GetComponent<Camera>();
            this.m_shadows = new List<SpriteShadow>();
        }

        public Material GetMaterialInstance() => Object.Instantiate<Material>(this.material);

        private void Update()
        {
            Collider[] colliderArray = Physics.OverlapSphere(this.transform.position, this.radius);
            Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(this.m_camera);
            for (int index1 = 0; index1 < colliderArray.Length; ++index1)
            {
                Collider collider = colliderArray[index1];
                tk2dSprite component = collider.GetComponent<tk2dSprite>();
                if ((!(collider.name != "PlayerSprite") || !((Object) collider.GetComponent<AIActor>() == (Object) null)) && (!((Object) collider.GetComponent<MeshRenderer>() != (Object) null) || collider.GetComponent<MeshRenderer>().enabled) && GeometryUtility.TestPlanesAABB(frustumPlanes, collider.bounds) && (Object) component != (Object) null)
                {
                    bool flag = false;
                    for (int index2 = 0; index2 < this.m_shadows.Count; ++index2)
                    {
                        if ((Object) this.m_shadows[index2].shadowedSprite == (Object) component)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                        this.m_shadows.Add(new SpriteShadow(component, this));
                }
            }
            for (int index = 0; index < this.m_shadows.Count; ++index)
            {
                SpriteShadow shadow = this.m_shadows[index];
                if ((Object) shadow.shadowedSprite == (Object) null || !shadow.shadowedSprite.enabled || !GeometryUtility.TestPlanesAABB(frustumPlanes, shadow.shadowedSprite.GetComponent<Collider>().bounds) || (double) (shadow.shadowedSprite.transform.position - this.transform.position).magnitude > (double) this.radius)
                {
                    this.m_shadows.RemoveAt(index);
                    --index;
                    shadow.Destroy();
                }
                else
                    shadow.UpdateShadow();
            }
        }
    }

