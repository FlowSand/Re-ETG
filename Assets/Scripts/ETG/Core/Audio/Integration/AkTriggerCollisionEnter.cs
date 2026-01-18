using UnityEngine;

#nullable disable

public class AkTriggerCollisionEnter : AkTriggerBase
  {
    public GameObject triggerObject;

    private void OnCollisionEnter(Collision in_other)
    {
      if (this.triggerDelegate == null || !((Object) this.triggerObject == (Object) null) && !((Object) this.triggerObject == (Object) in_other.gameObject))
        return;
      this.triggerDelegate(in_other.gameObject);
    }

    private void OnTriggerEnter(Collider in_other)
    {
      if (this.triggerDelegate == null || !((Object) this.triggerObject == (Object) null) && !((Object) this.triggerObject == (Object) in_other.gameObject))
        return;
      this.triggerDelegate(in_other.gameObject);
    }
  }

