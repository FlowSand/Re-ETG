using UnityEngine;

#nullable disable

public class GunAttachmentUnbreaker : MonoBehaviour
  {
    private void Start()
    {
    }

    private void Update()
    {
      if ((double) this.gameObject.transform.position.y >= 0.0)
        return;
      this.gameObject.transform.position = new Vector3(this.transform.position.x, Mathf.Abs(this.gameObject.transform.position.y), this.transform.position.z);
    }
  }

