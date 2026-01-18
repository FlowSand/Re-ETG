using System;
using System.Collections;
using System.Diagnostics;

using UnityEngine;
using UnityEngine.Rendering;

#nullable disable

public class PortalGunPortalController : BraveBehaviour
    {
        public bool IsAlternatePortal;
        public Vector2 PortalNormal;
        public PortalGunPortalController pairedPortal;
        public Camera FacewallCamera;
        public MeshRenderer PortalRenderer;
        public Texture2D FacewallMaskTexture;
        public int PixelWidth = 16;
        public int PixelHeight = 32;
        private RenderTexture m_renderTarget;
        private bool m_doRender;
        private int cm_bg;
        private int cm_fg;
        private static int m_portalNumber;

        private void Awake()
        {
            this.cm_bg = 1 << LayerMask.NameToLayer("BG_Nonsense") | 1 << LayerMask.NameToLayer("BG_Critical");
            this.cm_fg = 1 << LayerMask.NameToLayer("FG_Nonsense") | 1 << LayerMask.NameToLayer("ShadowCaster") | 1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("FG_Reflection") | 1 << LayerMask.NameToLayer("FG_Critical");
            if ((UnityEngine.Object) this.m_renderTarget == (UnityEngine.Object) null && (bool) (UnityEngine.Object) this.FacewallCamera)
            {
                this.FacewallCamera.orthographicSize = 1f;
                this.FacewallCamera.opaqueSortMode = OpaqueSortMode.FrontToBack;
                this.FacewallCamera.transparencySortMode = TransparencySortMode.Orthographic;
                this.FacewallCamera.enabled = false;
                this.m_renderTarget = RenderTexture.GetTemporary(this.PixelWidth, this.PixelHeight, 24, RenderTextureFormat.Default);
                this.m_renderTarget.filterMode = FilterMode.Point;
                this.FacewallCamera.targetTexture = this.m_renderTarget;
                Material material = UnityEngine.Object.Instantiate<Material>(this.PortalRenderer.material);
                material.shader = Shader.Find("Brave/Effects/CutoutPortalInternalTilted");
                material.SetTexture("_MainTex", (Texture) this.m_renderTarget);
                material.SetTexture("_MaskTex", (Texture) this.FacewallMaskTexture);
                this.PortalRenderer.material = material;
            }
            ++PortalGunPortalController.m_portalNumber;
        }

        private void LateUpdate()
        {
            if (!this.m_doRender)
                return;
            this.FacewallCamera.clearFlags = CameraClearFlags.Color;
            this.FacewallCamera.backgroundColor = Color.black;
            this.FacewallCamera.cullingMask = this.cm_bg;
            this.FacewallCamera.Render();
            this.FacewallCamera.clearFlags = CameraClearFlags.Depth;
            this.FacewallCamera.backgroundColor = Color.clear;
            this.FacewallCamera.cullingMask = this.cm_fg;
            this.FacewallCamera.Render();
        }

        [DebuggerHidden]
        private IEnumerator Start()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new PortalGunPortalController__Startc__Iterator0()
            {
                _this = this
            };
        }

        private void BecomeLinkedTo(PortalGunPortalController otherPortal)
        {
            this.pairedPortal = otherPortal;
            if (!(bool) (UnityEngine.Object) this.FacewallCamera)
                return;
            this.m_doRender = true;
            this.FacewallCamera.transform.position = otherPortal.transform.position + new Vector3(otherPortal.PortalNormal.x, (float) ((double) otherPortal.PortalNormal.y * 2.0 - 0.375), -10f) + CameraController.PLATFORM_CAMERA_OFFSET;
            this.PortalRenderer.enabled = true;
        }

        private void BecomeUnlinked()
        {
            this.pairedPortal = (PortalGunPortalController) null;
            this.m_doRender = false;
            if (!(bool) (UnityEngine.Object) this.FacewallCamera)
                return;
            this.PortalRenderer.enabled = false;
        }

        private void HandleTriggerCollision(
            SpeculativeRigidbody otherRigidbody,
            SpeculativeRigidbody myRigidbody,
            CollisionData collisionData)
        {
            if (!(bool) (UnityEngine.Object) this.pairedPortal)
                return;
            if ((bool) (UnityEngine.Object) otherRigidbody.projectile)
            {
                float z = Mathf.DeltaAngle(BraveMathCollege.Atan2Degrees(-this.PortalNormal), BraveMathCollege.Atan2Degrees(this.pairedPortal.PortalNormal));
                Vector2 unitCenter = this.pairedPortal.specRigidbody.UnitCenter;
                Vector2 vector = (double) this.pairedPortal.PortalNormal.x == 0.0 ? unitCenter + this.pairedPortal.PortalNormal.normalized : unitCenter + this.pairedPortal.PortalNormal.normalized * 0.5f;
                otherRigidbody.transform.position = vector.ToVector3ZisY();
                otherRigidbody.Reinitialize();
                PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(otherRigidbody);
                otherRigidbody.RegisterGhostCollisionException(this.pairedPortal.specRigidbody);
                otherRigidbody.RegisterGhostCollisionException(this.specRigidbody);
                otherRigidbody.projectile.SendInDirection((Quaternion.Euler(0.0f, 0.0f, z) * (Vector3) otherRigidbody.projectile.Direction).XY(), false);
                PhysicsEngine.SkipCollision = true;
                otherRigidbody.projectile.LastPosition = otherRigidbody.transform.position;
            }
            else
            {
                if (!(bool) (UnityEngine.Object) otherRigidbody.gameActor)
                    return;
                Vector2 vector2 = otherRigidbody.gameActor.transform.position.XY() - otherRigidbody.gameActor.specRigidbody.UnitCenter + this.pairedPortal.PortalNormal;
                if ((double) this.pairedPortal.PortalNormal.y < 0.0)
                    vector2 += this.pairedPortal.PortalNormal * 2f;
                if (otherRigidbody.gameActor is PlayerController)
                {
                    PlayerController gameActor = otherRigidbody.gameActor as PlayerController;
                    gameActor.WarpToPoint(this.pairedPortal.specRigidbody.UnitCenter + vector2);
                    gameActor.specRigidbody.RecheckTriggers = false;
                }
                else if (otherRigidbody.gameActor is AIActor)
                {
                    (otherRigidbody.gameActor as AIActor).transform.position = (Vector3) (this.pairedPortal.specRigidbody.UnitCenter + vector2);
                    otherRigidbody.Reinitialize();
                }
                PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(otherRigidbody, includeTriggers: true);
                otherRigidbody.RegisterTemporaryCollisionException(this.pairedPortal.specRigidbody, 0.5f);
                otherRigidbody.RegisterTemporaryCollisionException(this.specRigidbody, 0.5f);
                if (!(bool) (UnityEngine.Object) otherRigidbody.knockbackDoer)
                    return;
                otherRigidbody.knockbackDoer.ApplyKnockback(this.pairedPortal.PortalNormal, 10f);
            }
        }

        protected override void OnDestroy()
        {
            if ((UnityEngine.Object) this.pairedPortal != (UnityEngine.Object) null)
            {
                if ((UnityEngine.Object) this.pairedPortal.pairedPortal == (UnityEngine.Object) this)
                    this.pairedPortal.BecomeUnlinked();
                this.BecomeUnlinked();
            }
            StaticReferenceManager.AllPortals.Remove(this);
            if ((UnityEngine.Object) this.m_renderTarget != (UnityEngine.Object) null)
                RenderTexture.ReleaseTemporary(this.m_renderTarget);
            base.OnDestroy();
        }

        private void HandleRigidbodyCollision(CollisionData rigidbodyCollision)
        {
            throw new NotImplementedException();
        }
    }

