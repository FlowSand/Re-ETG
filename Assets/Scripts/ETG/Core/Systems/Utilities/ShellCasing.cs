// Decompiled with JetBrains decompiler
// Type: ShellCasing
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class ShellCasing : MonoBehaviour
  {
    private Vector3 startPosition;
    public float heightVariance = 0.5f;
    private float amountToFall;
    public float angularVelocity = 1080f;
    public float pushStrengthMultiplier = 1f;
    public bool usesCustomTargetHeight;
    public float customTargetHeight;
    public float overallMultiplier = 1f;
    public bool usesCustomVelocity;
    public Vector2 customVelocity = Vector2.zero;
    public string audioEventName;
    private Vector3 velocity;
    private bool isDone;
    private bool hasBeenAboveTargetHeight;
    private Transform m_transform;
    private Renderer m_renderer;
    private bool m_hasBeenTriggered;

    private void Start() => this.m_transform = this.transform;

    public void Trigger()
    {
      this.m_transform = this.transform;
      this.m_renderer = this.GetComponent<Renderer>();
      float worldY = !this.usesCustomTargetHeight ? GameManager.Instance.PrimaryPlayer.transform.position.y : this.customTargetHeight;
      this.startPosition = this.m_transform.position;
      int num = (double) this.m_transform.right.x <= 0.0 ? -1 : 1;
      this.velocity = !this.usesCustomVelocity ? Vector3.up * (float) ((double) Random.value * 1.5 + 1.0) + -1.5f * Vector3.right * (float) num * (Random.value + 1.5f) : this.customVelocity.ToVector3ZUp();
      this.amountToFall = (float) ((double) this.startPosition.y - (double) worldY + (double) Random.value * (double) this.heightVariance);
      if ((double) this.amountToFall > 0.0)
        this.hasBeenAboveTargetHeight = true;
      this.GetComponent<tk2dSprite>().automaticallyManagesDepth = false;
      DepthLookupManager.ProcessRenderer(this.m_renderer, DepthLookupManager.GungeonSortingLayer.PLAYFIELD);
      DepthLookupManager.UpdateRendererWithWorldYPosition(this.m_renderer, worldY);
      this.isDone = false;
      this.m_hasBeenTriggered = true;
    }

    private void Update()
    {
      if (BraveUtility.isLoadingLevel || !this.m_hasBeenTriggered || this.isDone)
        return;
      IntVector2 vec = new IntVector2(Mathf.FloorToInt(this.m_transform.position.x), Mathf.FloorToInt(this.m_transform.position.y));
      if (!GameManager.Instance.Dungeon.data.CheckInBounds(vec))
      {
        this.isDone = true;
      }
      else
      {
        CellData cellData = GameManager.Instance.Dungeon.data.cellData[vec.x][vec.y];
        if (cellData.type == CellType.WALL)
          this.velocity.x = -this.velocity.x;
        float num1 = BraveTime.DeltaTime * this.overallMultiplier;
        if ((double) this.velocity.y < 0.0)
          this.hasBeenAboveTargetHeight = true;
        if ((double) this.m_transform.position.y > (double) this.startPosition.y - (double) this.amountToFall || cellData != null && !cellData.IsPassable || !this.hasBeenAboveTargetHeight)
        {
          this.m_transform.Rotate(0.0f, 0.0f, this.angularVelocity * num1);
          this.m_transform.position += this.velocity * num1;
          this.velocity += Vector3.down * 10f * num1;
        }
        else
        {
          this.isDone = true;
          if (!string.IsNullOrEmpty(this.audioEventName))
          {
            int num2 = (int) AkSoundEngine.PostEvent(this.audioEventName, this.gameObject);
          }
          DepthLookupManager.UpdateRendererWithWorldYPosition(this.m_renderer, this.m_transform.position.y);
          MinorBreakable component = this.GetComponent<MinorBreakable>();
          if (!((Object) component != (Object) null))
            return;
          component.Break(Random.insideUnitCircle);
        }
      }
    }

    public void ApplyVelocity(Vector2 vel)
    {
      this.StartCoroutine(this.HandlePush(vel * this.pushStrengthMultiplier));
    }

    [DebuggerHidden]
    private IEnumerator HandlePush(Vector2 vel)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new ShellCasing__HandlePushc__Iterator0()
      {
        vel = vel,
        _this = this
      };
    }
  }

