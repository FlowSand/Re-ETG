using Dungeonator;
using UnityEngine;

#nullable disable

public class ScarfAttachmentDoer : MonoBehaviour
  {
    public float StartWidth = 1f / 16f;
    public float EndWidth = 0.125f;
    public float AnimationSpeed = 1f;
    public float ScarfLength = 1.5f;
    public float AngleLerpSpeed = 15f;
    public float ForwardZOffset;
    public float BackwardZOffset = -0.2f;
    public Vector2 MinOffset;
    public Vector2 MaxOffset;
    public float CatchUpScale = 2f;
    public GameActor AttachTarget;
    public Material ScarfMaterial;
    private float m_additionalOffsetTime;
    private Vector2 m_currentOffset;
    private Mesh m_mesh;
    private Vector3[] m_vertices;
    private MeshFilter m_stringFilter;
    private MeshRenderer m_mr;
    private float m_lastVelAngle;
    private bool m_isLerpingBack;
    private float m_targetLength;
    private float m_currentLength = 0.05f;
    public float SinSpeed = 1f;
    public float AmplitudeMod = 0.25f;
    public float WavelengthMod = 1f;

    public void Initialize(GameActor target)
    {
      this.m_targetLength = this.ScarfLength;
      this.AttachTarget = target;
      this.m_currentOffset = new Vector2(1f, 2f);
      this.m_mesh = new Mesh();
      this.m_vertices = new Vector3[20];
      this.m_mesh.vertices = this.m_vertices;
      int[] numArray = new int[54];
      Vector2[] vector2Array = new Vector2[20];
      int num = 0;
      for (int index = 0; index < 9; ++index)
      {
        numArray[index * 6] = num;
        numArray[index * 6 + 1] = num + 2;
        numArray[index * 6 + 2] = num + 1;
        numArray[index * 6 + 3] = num + 2;
        numArray[index * 6 + 4] = num + 3;
        numArray[index * 6 + 5] = num + 1;
        num += 2;
      }
      this.m_mesh.triangles = numArray;
      this.m_mesh.uv = vector2Array;
      GameObject target1 = new GameObject("balloon string");
      this.m_stringFilter = target1.AddComponent<MeshFilter>();
      this.m_mr = target1.AddComponent<MeshRenderer>();
      this.m_mr.material = this.ScarfMaterial;
      Object.DontDestroyOnLoad((Object) this.gameObject);
      Object.DontDestroyOnLoad((Object) target1);
      this.m_stringFilter.mesh = this.m_mesh;
      this.transform.position = this.AttachTarget.transform.position + this.m_currentOffset.ToVector3ZisY(-3f);
    }

    private void LateUpdate()
    {
      if (GameManager.Instance.IsLoadingLevel || Dungeon.IsGenerating || !((Object) this.AttachTarget != (Object) null))
        return;
      if (this.AttachTarget is AIActor && (!(bool) (Object) this.AttachTarget || this.AttachTarget.healthHaver.IsDead))
      {
        Object.Destroy((Object) this.gameObject);
      }
      else
      {
        this.m_targetLength = this.ScarfLength;
        bool flag = false;
        if (this.AttachTarget is PlayerController)
        {
          PlayerController attachTarget = this.AttachTarget as PlayerController;
          this.m_mr.enabled = attachTarget.IsVisible && attachTarget.sprite.renderer.enabled;
          this.m_mr.gameObject.layer = attachTarget.gameObject.layer;
          if ((double) attachTarget.FacingDirection <= 155.0 && (double) attachTarget.FacingDirection >= 25.0)
            flag = true;
          if (attachTarget.IsFalling)
            this.m_targetLength = 0.05f;
        }
        this.m_currentLength = Mathf.MoveTowards(this.m_currentLength, this.m_targetLength, BraveTime.DeltaTime * 2.5f);
        if ((double) this.m_currentLength < 0.10000000149011612)
          this.m_mr.enabled = false;
        Vector2 commandedDirection = (this.AttachTarget as PlayerController).LastCommandedDirection;
        this.m_isLerpingBack = (double) commandedDirection.magnitude < 0.125;
        float lastVelAngle = this.m_lastVelAngle;
        float num1 = !this.m_isLerpingBack ? BraveMathCollege.Atan2Degrees(commandedDirection) : ((double) Mathf.DeltaAngle(this.m_lastVelAngle, -45f) <= (double) Mathf.DeltaAngle(this.m_lastVelAngle, 135f) ? 0.0f : 180f);
        this.m_lastVelAngle = Mathf.LerpAngle(this.m_lastVelAngle, num1, BraveTime.DeltaTime * this.AngleLerpSpeed * Mathf.Lerp(1f, 2f, Mathf.DeltaAngle(this.m_lastVelAngle, num1) / 180f));
        float num2 = this.m_currentLength * Mathf.Lerp(2f, 1f, Vector2.Distance(this.transform.position.XY(), this.AttachTarget.sprite.WorldCenter) / 3f);
        this.m_currentOffset = (Quaternion.Euler(0.0f, 0.0f, this.m_lastVelAngle) * (Vector3) Vector2.left * num2).XY();
        this.m_currentOffset += Vector2.Lerp(this.MinOffset, this.MaxOffset, Mathf.SmoothStep(0.0f, 1f, Mathf.PingPong(UnityEngine.Time.realtimeSinceStartup * this.AnimationSpeed, 3f) / 3f));
        Vector3 vector3_1 = (Vector3) (this.AttachTarget.sprite.WorldCenter + new Vector2(0.0f, -5f / 16f));
        Vector3 vector3_2 = vector3_1 + this.m_currentOffset.ToVector3ZisY(-3f);
        float num3 = Vector3.Distance(this.transform.position, vector3_2);
        if ((double) num3 > 10.0)
          this.transform.position = vector3_2;
        else
          this.transform.position = Vector3.MoveTowards(this.transform.position, vector3_2, BraveMathCollege.UnboundedLerp(1f, 10f, num3 / this.CatchUpScale) * BraveTime.DeltaTime);
        Vector2 vector2 = vector3_2.XY() - this.transform.position.XY();
        this.m_additionalOffsetTime += Random.Range(0.0f, BraveTime.DeltaTime);
        this.BuildMeshAlongCurve((Vector2) vector3_1, vector3_1.XY() + new Vector2(0.0f, 0.1f), this.transform.position.XY() + vector2, this.transform.position.XY(), !flag ? this.ForwardZOffset : this.BackwardZOffset);
        this.m_mesh.vertices = this.m_vertices;
        this.m_mesh.RecalculateBounds();
        this.m_mesh.RecalculateNormals();
      }
    }

    private void OnDestroy()
    {
      if (!(bool) (Object) this.m_stringFilter)
        return;
      Object.Destroy((Object) this.m_stringFilter.gameObject);
    }

    private void BuildMeshAlongCurve(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float zOffset)
    {
      Vector3[] vertices = this.m_vertices;
      Vector2? nullable1 = new Vector2?();
      Vector2 vector2_1 = p3 - p0;
      Vector2 vector2_2 = (Quaternion.Euler(0.0f, 0.0f, 90f) * (Vector3) vector2_1).XY();
      for (int index = 0; index < 10; ++index)
      {
        Vector2 bezierPoint = BraveMathCollege.CalculateBezierPoint((float) index / 9f, p0, p1, p2, p3);
        Vector2? nullable2 = index != 9 ? new Vector2?(BraveMathCollege.CalculateBezierPoint((float) index / 9f, p0, p1, p2, p3)) : new Vector2?();
        Vector2 vector2_3 = Vector2.zero;
        if (nullable1.HasValue)
          vector2_3 += (Quaternion.Euler(0.0f, 0.0f, 90f) * (Vector3) (bezierPoint - nullable1.Value)).XY().normalized;
        if (nullable2.HasValue)
          vector2_3 += (Quaternion.Euler(0.0f, 0.0f, 90f) * (Vector3) (nullable2.Value - bezierPoint)).XY().normalized;
        vector2_3 = vector2_3.normalized;
        float num = Mathf.Lerp(this.StartWidth, this.EndWidth, (float) index / 9f);
        Vector2 vector2_4 = bezierPoint + vector2_2.normalized * Mathf.Sin((float) ((double) UnityEngine.Time.realtimeSinceStartup * (double) this.SinSpeed + (double) index * (double) this.WavelengthMod)) * this.AmplitudeMod * ((float) index / 9f);
        vertices[index * 2] = (vector2_4 + vector2_3 * num).ToVector3ZisY(zOffset);
        vertices[index * 2 + 1] = (vector2_4 + -vector2_3 * num).ToVector3ZisY(zOffset);
        nullable1 = new Vector2?(vector2_4);
      }
    }
  }

