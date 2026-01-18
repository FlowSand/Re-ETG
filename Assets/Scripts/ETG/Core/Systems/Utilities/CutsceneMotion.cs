using UnityEngine;

#nullable disable

public class CutsceneMotion
    {
        public Transform transform;
        public CameraController camera;
        public Vector2 lerpStart;
        public Vector2? lerpEnd;
        public float lerpProgress;
        public float speed;
        public float zOffset;
        public bool isSmoothStepped = true;

        public CutsceneMotion(Transform t, Vector2? targetPosition, float s, float z = 0.0f)
        {
            this.transform = t;
            this.lerpStart = t.position.XY();
            this.lerpEnd = targetPosition;
            this.lerpProgress = 0.0f;
            this.speed = s;
            this.zOffset = z;
        }
    }

