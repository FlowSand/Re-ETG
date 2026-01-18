using UnityEngine;

#nullable disable

public class ExplosionManager : BraveBehaviour
  {
    private const float c_explosionStaggerDelay = 0.125f;
    private System.Collections.Generic.Queue<Exploder> m_queue = new System.Collections.Generic.Queue<Exploder>();
    private float m_timer;
    private static ExplosionManager m_instance;

    public static ExplosionManager Instance
    {
      get
      {
        if (!(bool) (UnityEngine.Object) ExplosionManager.m_instance)
          ExplosionManager.m_instance = new GameObject("_ExplosionManager", new System.Type[1]
          {
            typeof (ExplosionManager)
          }).GetComponent<ExplosionManager>();
        return ExplosionManager.m_instance;
      }
    }

    public static void ClearPerLevelData() => ExplosionManager.m_instance = (ExplosionManager) null;

    public void Update()
    {
      if ((double) this.m_timer <= 0.0)
        return;
      this.m_timer -= BraveTime.DeltaTime;
    }

    protected override void OnDestroy() => base.OnDestroy();

    public void Queue(Exploder exploder) => this.m_queue.Enqueue(exploder);

    public bool IsExploderReady(Exploder exploder)
    {
      if (this.m_queue.Count == 0)
        return true;
      return (UnityEngine.Object) this.m_queue.Peek() == (UnityEngine.Object) exploder && (double) this.m_timer <= 0.0;
    }

    public void Dequeue()
    {
      if (this.m_queue.Count > 0)
        this.m_queue.Dequeue();
      this.m_timer = 0.125f;
    }

    public int QueueCount => this.m_queue.Count;
  }

