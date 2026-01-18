using UnityEngine;

#nullable disable
namespace InterfaceMovement
{
  public class ButtonFocus : MonoBehaviour
  {
    private void Update()
    {
      this.transform.position = Vector3.MoveTowards(this.transform.position, this.transform.parent.GetComponent<ButtonManager>().focusedButton.transform.position, Time.deltaTime * 10f);
    }
  }
}
