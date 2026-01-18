using UnityEngine;

#nullable disable

[ExecuteInEditMode]
[AddComponentMenu("2D Toolkit/Camera/tk2dCameraAnchor")]
public class tk2dCameraAnchor : MonoBehaviour
    {
        [SerializeField]
        private int anchor = -1;
        [SerializeField]
        private tk2dBaseSprite.Anchor _anchorPoint = tk2dBaseSprite.Anchor.UpperLeft;
        [SerializeField]
        private bool anchorToNativeBounds;
        [SerializeField]
        private Vector2 offset = Vector2.zero;
        [SerializeField]
        private tk2dCamera tk2dCamera;
        [SerializeField]
        private Camera _anchorCamera;
        private Camera _anchorCameraCached;
        private tk2dCamera _anchorTk2dCamera;
        private Transform _myTransform;

        public tk2dBaseSprite.Anchor AnchorPoint
        {
            get
            {
                if (this.anchor != -1)
                {
                    this._anchorPoint = this.anchor < 0 || this.anchor > 2 ? (this.anchor < 6 || this.anchor > 8 ? (tk2dBaseSprite.Anchor) this.anchor : (tk2dBaseSprite.Anchor) (this.anchor - 6)) : (tk2dBaseSprite.Anchor) (this.anchor + 6);
                    this.anchor = -1;
                }
                return this._anchorPoint;
            }
            set => this._anchorPoint = value;
        }

        public Vector2 AnchorOffsetPixels
        {
            get => this.offset;
            set => this.offset = value;
        }

        public bool AnchorToNativeBounds
        {
            get => this.anchorToNativeBounds;
            set => this.anchorToNativeBounds = value;
        }

        public Camera AnchorCamera
        {
            get
            {
                if ((Object) this.tk2dCamera != (Object) null)
                {
                    this._anchorCamera = this.tk2dCamera.GetComponent<Camera>();
                    this.tk2dCamera = (tk2dCamera) null;
                }
                return this._anchorCamera;
            }
            set
            {
                this._anchorCamera = value;
                this._anchorCameraCached = (Camera) null;
            }
        }

        private tk2dCamera AnchorTk2dCamera
        {
            get
            {
                if ((Object) this._anchorCameraCached != (Object) this._anchorCamera)
                {
                    this._anchorTk2dCamera = this._anchorCamera.GetComponent<tk2dCamera>();
                    this._anchorCameraCached = this._anchorCamera;
                }
                return this._anchorTk2dCamera;
            }
        }

        private Transform myTransform
        {
            get
            {
                if ((Object) this._myTransform == (Object) null)
                    this._myTransform = this.transform;
                return this._myTransform;
            }
        }

        private void Start() => this.UpdateTransform();

        private void UpdateTransform()
        {
            if ((Object) this.AnchorCamera == (Object) null)
                return;
            float num = 1f;
            Vector3 localPosition = this.myTransform.localPosition;
            tk2dCamera anchorTk2dCamera = !((Object) this.AnchorTk2dCamera != (Object) null) || this.AnchorTk2dCamera.CameraSettings.projection == tk2dCameraSettings.ProjectionType.Perspective ? (tk2dCamera) null : this.AnchorTk2dCamera;
            Rect rect = new Rect();
            if ((Object) anchorTk2dCamera != (Object) null)
            {
                rect = !this.anchorToNativeBounds ? anchorTk2dCamera.ScreenExtents : anchorTk2dCamera.NativeScreenExtents;
                num = anchorTk2dCamera.GetSizeAtDistance(1f);
            }
            else
                rect.Set(0.0f, 0.0f, (float) this.AnchorCamera.pixelWidth, (float) this.AnchorCamera.pixelHeight);
            float yMin = rect.yMin;
            float yMax = rect.yMax;
            float y = (float) (((double) yMin + (double) yMax) * 0.5);
            float xMin = rect.xMin;
            float xMax = rect.xMax;
            float x = (float) (((double) xMin + (double) xMax) * 0.5);
            Vector3 vector3 = Vector3.zero;
            switch (this.AnchorPoint)
            {
                case tk2dBaseSprite.Anchor.LowerLeft:
                    vector3 = new Vector3(xMin, yMin, localPosition.z);
                    break;
                case tk2dBaseSprite.Anchor.LowerCenter:
                    vector3 = new Vector3(x, yMin, localPosition.z);
                    break;
                case tk2dBaseSprite.Anchor.LowerRight:
                    vector3 = new Vector3(xMax, yMin, localPosition.z);
                    break;
                case tk2dBaseSprite.Anchor.MiddleLeft:
                    vector3 = new Vector3(xMin, y, localPosition.z);
                    break;
                case tk2dBaseSprite.Anchor.MiddleCenter:
                    vector3 = new Vector3(x, y, localPosition.z);
                    break;
                case tk2dBaseSprite.Anchor.MiddleRight:
                    vector3 = new Vector3(xMax, y, localPosition.z);
                    break;
                case tk2dBaseSprite.Anchor.UpperLeft:
                    vector3 = new Vector3(xMin, yMax, localPosition.z);
                    break;
                case tk2dBaseSprite.Anchor.UpperCenter:
                    vector3 = new Vector3(x, yMax, localPosition.z);
                    break;
                case tk2dBaseSprite.Anchor.UpperRight:
                    vector3 = new Vector3(xMax, yMax, localPosition.z);
                    break;
            }
            Vector3 position = vector3 + new Vector3(num * this.offset.x, num * this.offset.y, 0.0f);
            if ((Object) anchorTk2dCamera == (Object) null)
            {
                Vector3 worldPoint = this.AnchorCamera.ScreenToWorldPoint(position);
                if (!(this.myTransform.position != worldPoint))
                    return;
                this.myTransform.position = worldPoint;
            }
            else
            {
                if (!(this.myTransform.localPosition != position))
                    return;
                this.myTransform.localPosition = position;
            }
        }

        public void ForceUpdateTransform() => this.UpdateTransform();

        private void LateUpdate() => this.UpdateTransform();
    }

