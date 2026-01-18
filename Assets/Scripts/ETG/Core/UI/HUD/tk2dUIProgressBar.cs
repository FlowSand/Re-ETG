// Decompiled with JetBrains decompiler
// Type: tk2dUIProgressBar
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

[AddComponentMenu("2D Toolkit/UI/tk2dUIProgressBar")]
public class tk2dUIProgressBar : MonoBehaviour
  {
    public Transform scalableBar;
    public tk2dClippedSprite clippedSpriteBar;
    public tk2dSlicedSprite slicedSpriteBar;
    private bool initializedSlicedSpriteDimensions;
    private Vector2 emptySlicedSpriteDimensions = Vector2.zero;
    private Vector2 fullSlicedSpriteDimensions = Vector2.zero;
    private Vector2 currentDimensions = Vector2.zero;
    [SerializeField]
    private float percent;
    private bool isProgressComplete;
    public GameObject sendMessageTarget;
    public string SendMessageOnProgressCompleteMethodName = string.Empty;

    public event System.Action OnProgressComplete;

    private void Start()
    {
      this.InitializeSlicedSpriteDimensions();
      this.Value = this.percent;
    }

    public float Value
    {
      get => this.percent;
      set
      {
        this.percent = Mathf.Clamp(value, 0.0f, 1f);
        if (!Application.isPlaying)
          return;
        if ((UnityEngine.Object) this.clippedSpriteBar != (UnityEngine.Object) null)
          this.clippedSpriteBar.clipTopRight = new Vector2(this.Value, 1f);
        else if ((UnityEngine.Object) this.scalableBar != (UnityEngine.Object) null)
          this.scalableBar.localScale = new Vector3(this.Value, this.scalableBar.localScale.y, this.scalableBar.localScale.z);
        else if ((UnityEngine.Object) this.slicedSpriteBar != (UnityEngine.Object) null)
        {
          this.InitializeSlicedSpriteDimensions();
          this.currentDimensions.Set(Mathf.Lerp(this.emptySlicedSpriteDimensions.x, this.fullSlicedSpriteDimensions.x, this.Value), this.fullSlicedSpriteDimensions.y);
          this.slicedSpriteBar.dimensions = this.currentDimensions;
        }
        if (!this.isProgressComplete && (double) this.Value == 1.0)
        {
          this.isProgressComplete = true;
          if (this.OnProgressComplete != null)
            this.OnProgressComplete();
          if (!((UnityEngine.Object) this.sendMessageTarget != (UnityEngine.Object) null) || this.SendMessageOnProgressCompleteMethodName.Length <= 0)
            return;
          this.sendMessageTarget.SendMessage(this.SendMessageOnProgressCompleteMethodName, (object) this, SendMessageOptions.RequireReceiver);
        }
        else
        {
          if (!this.isProgressComplete || (double) this.Value >= 1.0)
            return;
          this.isProgressComplete = false;
        }
      }
    }

    private void InitializeSlicedSpriteDimensions()
    {
      if (this.initializedSlicedSpriteDimensions)
        return;
      if ((UnityEngine.Object) this.slicedSpriteBar != (UnityEngine.Object) null)
      {
        tk2dSpriteDefinition currentSprite = this.slicedSpriteBar.CurrentSprite;
        Vector3 boundsDataExtents = currentSprite.boundsDataExtents;
        this.fullSlicedSpriteDimensions = this.slicedSpriteBar.dimensions;
        this.emptySlicedSpriteDimensions.Set((this.slicedSpriteBar.borderLeft + this.slicedSpriteBar.borderRight) * boundsDataExtents.x / currentSprite.texelSize.x, this.fullSlicedSpriteDimensions.y);
      }
      this.initializedSlicedSpriteDimensions = true;
    }
  }

