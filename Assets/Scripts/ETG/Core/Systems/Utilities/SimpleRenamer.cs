using UnityEngine;

#nullable disable

public class SimpleRenamer : MonoBehaviour
    {
        public string OverrideName;

        public void Start() => this.name = this.OverrideName;
    }

