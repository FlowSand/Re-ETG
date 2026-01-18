using UnityEngine;

#nullable disable

public class AkTriggerDisable : AkTriggerBase
    {
        private void OnDisable()
        {
            if (this.triggerDelegate == null)
                return;
            this.triggerDelegate((GameObject) null);
        }
    }

