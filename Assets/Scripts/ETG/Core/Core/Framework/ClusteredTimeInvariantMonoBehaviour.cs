#nullable disable

public class ClusteredTimeInvariantMonoBehaviour : BraveBehaviour
    {
        protected float m_deltaTime;

        protected virtual void Awake()
        {
            StaticReferenceManager.AllClusteredTimeInvariantBehaviours.Add(this);
        }

        public void DoUpdate(float realDeltaTime)
        {
            this.m_deltaTime = realDeltaTime;
            this.InvariantUpdate(realDeltaTime);
        }

        protected virtual void InvariantUpdate(float realDeltaTime)
        {
        }

        protected override void OnDestroy()
        {
            StaticReferenceManager.AllClusteredTimeInvariantBehaviours.Remove(this);
            base.OnDestroy();
        }
    }

