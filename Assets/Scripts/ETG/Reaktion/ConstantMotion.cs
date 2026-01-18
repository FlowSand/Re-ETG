using System;

using UnityEngine;

#nullable disable
namespace Reaktion
{
    public class ConstantMotion : MonoBehaviour
    {
        public ConstantMotion.TransformElement position = new ConstantMotion.TransformElement();
        public ConstantMotion.TransformElement rotation = new ConstantMotion.TransformElement()
        {
            velocity = 30f
        };
        public bool useLocalCoordinate = true;

        private void Awake()
        {
            this.position.Initialize();
            this.rotation.Initialize();
        }

        private void Update()
        {
            if (this.position.mode != ConstantMotion.TransformMode.Off)
            {
                if (this.useLocalCoordinate)
                    this.transform.localPosition += this.position.Vector * this.position.Delta;
                else
                    this.transform.position += this.position.Vector * this.position.Delta;
            }
            if (this.rotation.mode == ConstantMotion.TransformMode.Off)
                return;
            Quaternion quaternion = Quaternion.AngleAxis(this.rotation.Delta, this.rotation.Vector);
            if (this.useLocalCoordinate)
                this.transform.localRotation = quaternion * this.transform.localRotation;
            else
                this.transform.rotation = quaternion * this.transform.rotation;
        }

        public enum TransformMode
        {
            Off,
            XAxis,
            YAxis,
            ZAxis,
            Arbitrary,
            Random,
        }

        [Serializable]
        public class TransformElement
        {
            public ConstantMotion.TransformMode mode;
            public float velocity = 1f;
            public Vector3 arbitraryVector = Vector3.up;
            public float randomness;
            private Vector3 randomVector;
            private float randomScalar;

            public void Initialize()
            {
                this.randomVector = UnityEngine.Random.onUnitSphere;
                this.randomScalar = UnityEngine.Random.value;
            }

            public Vector3 Vector
            {
                get
                {
                    switch (this.mode)
                    {
                        case ConstantMotion.TransformMode.XAxis:
                            return Vector3.right;
                        case ConstantMotion.TransformMode.YAxis:
                            return Vector3.up;
                        case ConstantMotion.TransformMode.ZAxis:
                            return Vector3.forward;
                        case ConstantMotion.TransformMode.Arbitrary:
                            return this.arbitraryVector;
                        case ConstantMotion.TransformMode.Random:
                            return this.randomVector;
                        default:
                            return Vector3.zero;
                    }
                }
            }

            public float Delta
            {
                get
                {
                    return this.velocity * (float) (1.0 - (double) this.randomness * (double) this.randomScalar) * BraveTime.DeltaTime;
                }
            }
        }
    }
}
