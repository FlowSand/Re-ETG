using UnityEngine;

#nullable disable
namespace InControl
{
    public class UnityGyroAxisSource : InputControlSource
    {
        private static Quaternion zeroAttitude;
        public int Axis;

        public UnityGyroAxisSource() => UnityGyroAxisSource.Calibrate();

        public UnityGyroAxisSource(UnityGyroAxisSource.GyroAxis axis)
        {
            this.Axis = (int) axis;
            UnityGyroAxisSource.Calibrate();
        }

        public float GetValue(InputDevice inputDevice) => UnityGyroAxisSource.GetAxis()[this.Axis];

        public bool GetState(InputDevice inputDevice) => Utility.IsNotZero(this.GetValue(inputDevice));

        private static Quaternion GetAttitude()
        {
            return Quaternion.Inverse(UnityGyroAxisSource.zeroAttitude) * Input.gyro.attitude;
        }

        private static Vector3 GetAxis()
        {
            Vector3 vector3 = UnityGyroAxisSource.GetAttitude() * Vector3.forward;
            return new Vector3(UnityGyroAxisSource.ApplyDeadZone(Mathf.Clamp(vector3.x, -1f, 1f)), UnityGyroAxisSource.ApplyDeadZone(Mathf.Clamp(vector3.y, -1f, 1f)));
        }

        private static float ApplyDeadZone(float value)
        {
            return Mathf.InverseLerp(0.05f, 1f, Utility.Abs(value)) * Mathf.Sign(value);
        }

        public static void Calibrate() => UnityGyroAxisSource.zeroAttitude = Input.gyro.attitude;

        public enum GyroAxis
        {
            X,
            Y,
        }
    }
}
