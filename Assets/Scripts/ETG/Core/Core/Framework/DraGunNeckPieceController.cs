using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class DraGunNeckPieceController : BraveBehaviour
  {
    public string leftSprite;
    public string forwardSprite;
    public string rightSprite;
    public float flipThreshold;
    [CurveRange(0.0f, -6f, 6f, 12f)]
    public AnimationCurve xCurve;
    [CurveRange(-5f, -5f, 9f, 10f)]
    public AnimationCurve yCurve;
    public float idleTime;
    public float idleOffset;
    private bool m_initialized;
    private Vector2 m_startingPos;
    private bool m_isIdleUp;
    private float m_idleTimer;

    [DebuggerHidden]
    public IEnumerator Start()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new DraGunNeckPieceController__Startc__Iterator0()
      {
        _this = this
      };
    }

    public void Update()
    {
      if (!this.m_initialized)
        return;
      this.m_idleTimer -= BraveTime.DeltaTime;
      if ((double) this.m_idleTimer >= 0.0)
        return;
      this.m_idleTimer += this.idleTime;
      this.m_isIdleUp = !this.m_isIdleUp;
    }

    protected override void OnDestroy() => base.OnDestroy();

    public void UpdateHeadDelta(Vector2 headDelta)
    {
      if (!this.m_initialized)
        return;
      Vector2 vector2 = this.m_startingPos + new Vector2(Mathf.Sign(headDelta.x) * this.xCurve.Evaluate(Mathf.Abs(headDelta.x)), 0.0f) + new Vector2(0.0f, this.yCurve.Evaluate(headDelta.y));
      if (this.m_isIdleUp)
        vector2 += PhysicsEngine.PixelToUnit(new IntVector2(0, 1));
      this.transform.position = new Vector3(BraveMathCollege.QuantizeFloat(vector2.x, 1f / 16f), BraveMathCollege.QuantizeFloat(vector2.y, 1f / 16f));
      if ((double) Mathf.Abs(headDelta.x) > (double) this.flipThreshold)
        this.sprite.SetSprite((double) Mathf.Sign(headDelta.x) >= 0.0 ? this.rightSprite : this.leftSprite);
      else
        this.sprite.SetSprite(this.forwardSprite);
    }
  }

