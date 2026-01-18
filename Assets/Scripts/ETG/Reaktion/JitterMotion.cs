using UnityEngine;

#nullable disable
namespace Reaktion
{
    public class JitterMotion : MonoBehaviour
    {
        public bool AllowCameraInfluence;
        public float InfluenceAxialX = 20f;
        public float InfluenceAxialY = 20f;
        public float positionFrequency = 0.2f;
        public float rotationFrequency = 0.2f;
        public float positionAmount = 1f;
        public float rotationAmount = 30f;
        public Vector3 positionComponents = Vector3.one;
        public Vector3 rotationComponents = new Vector3(1f, 1f, 0.0f);
        public int positionOctave = 3;
        public int rotationOctave = 3;
        public bool UseMainCameraShakeAmount = true;
        private float timePosition;
        private float timeRotation;
        private Vector2[] noiseVectors;
        private Vector3 initialPosition;
        private Quaternion initialRotation;
        private Vector2 m_currentInfluenceVec = Vector2.zero;

        public Vector3 GetInitialPosition() => this.initialPosition;

        public Quaternion GetInitialRotation() => this.initialRotation;

        private void Awake()
        {
            this.timePosition = Random.value * 10f;
            this.timeRotation = Random.value * 10f;
            this.noiseVectors = new Vector2[6];
            for (int index = 0; index < 6; ++index)
            {
                float f = (float) ((double) Random.value * 3.1415927410125732 * 2.0);
                this.noiseVectors[index].Set(Mathf.Cos(f), Mathf.Sin(f));
            }
            this.initialPosition = this.transform.localPosition;
            this.initialRotation = this.transform.localRotation;
        }

        private void Update()
        {
            this.timePosition += BraveTime.DeltaTime * this.positionFrequency;
            this.timeRotation += BraveTime.DeltaTime * this.rotationFrequency;
            if ((double) this.positionAmount != 0.0)
                this.transform.localPosition = this.initialPosition + Vector3.Scale(new Vector3(JitterMotion.Fbm(this.noiseVectors[0] * this.timePosition, this.positionOctave), JitterMotion.Fbm(this.noiseVectors[1] * this.timePosition, this.positionOctave), JitterMotion.Fbm(this.noiseVectors[2] * this.timePosition, this.positionOctave)), this.positionComponents) * this.positionAmount * 2f + GameManager.Instance.MainCameraController.ScreenShakeVector * 5f;
            if ((double) this.rotationAmount != 0.0)
                this.transform.localRotation = Quaternion.Euler(Vector3.Scale(new Vector3(JitterMotion.Fbm(this.noiseVectors[3] * this.timeRotation, this.rotationOctave), JitterMotion.Fbm(this.noiseVectors[4] * this.timeRotation, this.rotationOctave), JitterMotion.Fbm(this.noiseVectors[5] * this.timeRotation, this.rotationOctave)), this.rotationComponents) * this.rotationAmount * 2f) * this.initialRotation;
            if (!this.AllowCameraInfluence)
                return;
            Vector2 target = Vector2.zero;
            if ((Object) BraveInput.PrimaryPlayerInstance != (Object) null)
                target = !BraveInput.PrimaryPlayerInstance.IsKeyboardAndMouse() ? BraveInput.PrimaryPlayerInstance.ActiveActions.Aim.Vector : ((Vector2) GameManager.Instance.MainCameraController.Camera.ScreenToViewportPoint(Input.mousePosition) + new Vector2(-0.5f, -0.5f)) * 2f;
            this.m_currentInfluenceVec = Vector2.MoveTowards(this.m_currentInfluenceVec, target, 1.25f * GameManager.INVARIANT_DELTA_TIME);
            float angle1 = this.m_currentInfluenceVec.x * this.InfluenceAxialX;
            float angle2 = (float) ((double) this.m_currentInfluenceVec.y * (double) this.InfluenceAxialY * -1.0);
            this.transform.RotateAround(this.transform.position + this.transform.forward * 10f, Vector3.up, angle1);
            this.transform.RotateAround(this.transform.position + this.transform.forward * 10f, Vector3.right, angle2);
        }

        public static float Fbm(Vector2 coord, int octave)
        {
            float num1 = 0.0f;
            float num2 = 1f;
            for (int index = 0; index < octave; ++index)
            {
                num1 += num2 * (Mathf.PerlinNoise(coord.x, coord.y) - 0.5f);
                coord *= 2f;
                num2 *= 0.5f;
            }
            return num1;
        }
    }
}
