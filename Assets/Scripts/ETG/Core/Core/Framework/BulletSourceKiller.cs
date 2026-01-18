using UnityEngine;

#nullable disable

public class BulletSourceKiller : BraveBehaviour
    {
        public BulletScriptSource BraveSource;
        public SpeculativeRigidbody TrackRigidbody;

        public void Start()
        {
            if ((bool) (Object) this.BraveSource)
                return;
            this.BraveSource = this.GetComponent<BulletScriptSource>();
        }

        public void Update()
        {
            if ((bool) (Object) this.TrackRigidbody)
                this.transform.position = (Vector3) this.TrackRigidbody.GetUnitCenter(ColliderType.HitBox);
            if ((bool) (Object) this.BraveSource && this.BraveSource.IsEnded)
                Object.Destroy((Object) this.gameObject);
            if ((bool) (Object) this.BraveSource)
                return;
            Object.Destroy((Object) this.gameObject);
        }

        protected override void OnDestroy() => base.OnDestroy();
    }

