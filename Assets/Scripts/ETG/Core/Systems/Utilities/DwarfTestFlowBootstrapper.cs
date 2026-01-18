using UnityEngine;

#nullable disable

public class DwarfTestFlowBootstrapper : MonoBehaviour
    {
        public static bool IsBootstrapping;
        public static bool ShouldConvertToCoopMode;
        public bool ConvertToCoopMode;

        private void Start()
        {
            foreach (UnityEngine.Object @object in UnityEngine.Object.FindObjectsOfType<GameManager>())
                UnityEngine.Object.Destroy(@object);
            if (this.ConvertToCoopMode)
                DwarfTestFlowBootstrapper.ShouldConvertToCoopMode = true;
            UnityEngine.Random.InitState(new System.Random().Next(1, 1000));
        }
    }

