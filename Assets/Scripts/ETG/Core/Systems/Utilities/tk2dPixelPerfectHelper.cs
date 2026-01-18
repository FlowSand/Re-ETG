using System;

using UnityEngine;

#nullable disable

[AddComponentMenu("2D Toolkit/Deprecated/Extra/tk2dPixelPerfectHelper")]
public class tk2dPixelPerfectHelper : MonoBehaviour
    {
        private static tk2dPixelPerfectHelper _inst;
        [NonSerialized]
        public Camera cam;
        public int collectionTargetHeight = 640;
        public float collectionOrthoSize = 1f;
        public float targetResolutionHeight = 640f;
        [NonSerialized]
        public float scaleD;
        [NonSerialized]
        public float scaleK;

        public static tk2dPixelPerfectHelper inst
        {
            get
            {
                if ((UnityEngine.Object) tk2dPixelPerfectHelper._inst == (UnityEngine.Object) null)
                {
                    tk2dPixelPerfectHelper._inst = UnityEngine.Object.FindObjectOfType(typeof (tk2dPixelPerfectHelper)) as tk2dPixelPerfectHelper;
                    if ((UnityEngine.Object) tk2dPixelPerfectHelper._inst == (UnityEngine.Object) null)
                        return (tk2dPixelPerfectHelper) null;
                    tk2dPixelPerfectHelper.inst.Setup();
                }
                return tk2dPixelPerfectHelper._inst;
            }
        }

        private void Awake()
        {
            this.Setup();
            tk2dPixelPerfectHelper._inst = this;
        }

        public virtual void Setup()
        {
            float num1 = (float) this.collectionTargetHeight / this.targetResolutionHeight;
            if ((UnityEngine.Object) this.GetComponent<Camera>() != (UnityEngine.Object) null)
                this.cam = this.GetComponent<Camera>();
            if ((UnityEngine.Object) this.cam == (UnityEngine.Object) null)
                this.cam = Camera.main;
            if (this.cam.orthographic)
            {
                this.scaleK = num1 * this.cam.orthographicSize / this.collectionOrthoSize;
                this.scaleD = 0.0f;
            }
            else
            {
                float num2 = num1 * Mathf.Tan((float) (Math.PI / 180.0 * (double) this.cam.fieldOfView * 0.5)) / this.collectionOrthoSize;
                this.scaleK = num2 * -this.cam.transform.position.z;
                this.scaleD = num2;
            }
        }

        public static float CalculateScaleForPerspectiveCamera(float fov, float zdist)
        {
            return Mathf.Abs(Mathf.Tan((float) (Math.PI / 180.0 * (double) fov * 0.5)) * zdist);
        }

        public bool CameraIsOrtho => this.cam.orthographic;
    }

