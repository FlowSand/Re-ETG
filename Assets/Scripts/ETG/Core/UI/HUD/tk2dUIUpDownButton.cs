using UnityEngine;

#nullable disable

[AddComponentMenu("2D Toolkit/UI/tk2dUIUpDownButton")]
public class tk2dUIUpDownButton : tk2dUIBaseItemControl
  {
    public GameObject upStateGO;
    public GameObject downStateGO;
    [SerializeField]
    private bool useOnReleaseInsteadOfOnUp;
    private bool isDown;

    public bool UseOnReleaseInsteadOfOnUp => this.useOnReleaseInsteadOfOnUp;

    private void Start() => this.SetState();

    private void OnEnable()
    {
      if (!(bool) (UnityEngine.Object) this.uiItem)
        return;
      this.uiItem.OnDown += new System.Action(this.ButtonDown);
      if (this.useOnReleaseInsteadOfOnUp)
        this.uiItem.OnRelease += new System.Action(this.ButtonUp);
      else
        this.uiItem.OnUp += new System.Action(this.ButtonUp);
    }

    private void OnDisable()
    {
      if (!(bool) (UnityEngine.Object) this.uiItem)
        return;
      this.uiItem.OnDown -= new System.Action(this.ButtonDown);
      if (this.useOnReleaseInsteadOfOnUp)
        this.uiItem.OnRelease -= new System.Action(this.ButtonUp);
      else
        this.uiItem.OnUp -= new System.Action(this.ButtonUp);
    }

    private void ButtonUp()
    {
      this.isDown = false;
      this.SetState();
    }

    private void ButtonDown()
    {
      this.isDown = true;
      this.SetState();
    }

    private void SetState()
    {
      tk2dUIBaseItemControl.ChangeGameObjectActiveStateWithNullCheck(this.upStateGO, !this.isDown);
      tk2dUIBaseItemControl.ChangeGameObjectActiveStateWithNullCheck(this.downStateGO, this.isDown);
    }

    public void InternalSetUseOnReleaseInsteadOfOnUp(bool state)
    {
      this.useOnReleaseInsteadOfOnUp = state;
    }
  }

