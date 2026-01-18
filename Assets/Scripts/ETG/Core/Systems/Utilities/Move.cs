using UnityEngine;

#nullable disable

public class Move : MonoBehaviour
  {
    private void Update()
    {
      Vector3 zero = Vector3.zero;
      this.transform.Translate(Vector3.right * Input.GetAxis("Horizontal") * BraveTime.DeltaTime * 5f, Space.World);
      this.transform.Translate(Vector3.forward * Input.GetAxis("Vertical") * BraveTime.DeltaTime * 5f, Space.World);
    }
  }

