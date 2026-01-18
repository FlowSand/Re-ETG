using UnityEngine;

#nullable disable

public class SimpleSpriteUpdater : MonoBehaviour
  {
    private void Start() => this.GetComponent<tk2dBaseSprite>().UpdateZDepth();
  }

