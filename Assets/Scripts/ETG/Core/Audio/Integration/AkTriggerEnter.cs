using UnityEngine;

#nullable disable

public class AkTriggerEnter : AkTriggerBase
  {
    public GameObject triggerObject;

    private void OnTriggerEnter(Collider in_other)
    {
      if (this.triggerDelegate == null || !((Object) this.triggerObject == (Object) null) && !((Object) this.triggerObject == (Object) in_other.gameObject))
        return;
      this.triggerDelegate(in_other.gameObject);
    }
  }

