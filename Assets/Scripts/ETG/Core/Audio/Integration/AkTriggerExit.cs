using UnityEngine;

#nullable disable

public class AkTriggerExit : AkTriggerBase
  {
    public GameObject triggerObject;

    private void OnTriggerExit(Collider in_other)
    {
      if (this.triggerDelegate == null || !((Object) this.triggerObject == (Object) null) && !((Object) this.triggerObject == (Object) in_other.gameObject))
        return;
      this.triggerDelegate(in_other.gameObject);
    }
  }

