using UnityEngine;

#nullable disable

public class AkTriggerEnable : AkTriggerBase
    {
        private void OnEnable()
        {
            if (this.triggerDelegate == null)
                return;
            this.triggerDelegate((GameObject) null);
        }
    }

