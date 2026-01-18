// Decompiled with JetBrains decompiler
// Type: dfFollowObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Examples/General/Follow Object")]
public class dfFollowObject : MonoBehaviour
  {
    public Camera mainCamera;
    public GameObject attach;
    public dfPivotPoint anchor = dfPivotPoint.MiddleCenter;
    public Vector3 offset;
    public float hideDistance = 20f;
    public float fadeDistance = 15f;
    public bool constantScale;
    public bool stickToScreenEdge;
    [HideInInspector]
    public bool overrideVisibility = true;
    private Transform followTransform;
    private dfControl myControl;
    private dfGUIManager manager;

    private void OnEnable()
    {
      if ((UnityEngine.Object) this.mainCamera == (UnityEngine.Object) null)
      {
        this.mainCamera = Camera.main;
        if ((UnityEngine.Object) this.mainCamera == (UnityEngine.Object) null)
        {
          Debug.LogError((object) "dfFollowObject component is unable to determine which camera is the MainCamera", (UnityEngine.Object) this.gameObject);
          this.enabled = false;
          return;
        }
      }
      this.myControl = this.GetComponent<dfControl>();
      if ((UnityEngine.Object) this.myControl == (UnityEngine.Object) null)
      {
        Debug.LogError((object) ("No dfControl component on this GameObject: " + this.gameObject.name), (UnityEngine.Object) this.gameObject);
        this.enabled = false;
      }
      if ((UnityEngine.Object) this.myControl == (UnityEngine.Object) null || (UnityEngine.Object) this.attach == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) ("Configuration incomplete: " + this.name));
        this.enabled = false;
      }
      else
      {
        this.followTransform = this.attach.transform;
        this.manager = this.myControl.GetManager();
        dfFollowObjectSorter.Register(this);
        GameManager.Instance.MainCameraController.OnFinishedFrame -= new System.Action(this.OnMainCameraFinishedFrame);
        GameManager.Instance.MainCameraController.OnFinishedFrame += new System.Action(this.OnMainCameraFinishedFrame);
      }
    }

    private void OnMainCameraFinishedFrame()
    {
    }

    private void OnDisable()
    {
      if (!GameManager.HasInstance)
        return;
      dfFollowObjectSorter.Unregister(this);
      GameManager.Instance.MainCameraController.OnFinishedFrame -= new System.Action(this.OnMainCameraFinishedFrame);
    }

    private void OnDestroy()
    {
      if (!GameManager.HasInstance || !(bool) (UnityEngine.Object) GameManager.Instance.MainCameraController)
        return;
      GameManager.Instance.MainCameraController.OnFinishedFrame -= new System.Action(this.OnMainCameraFinishedFrame);
    }

    public void ForceUpdate()
    {
      this.OnEnable();
      this.Update();
    }

    public static Vector3 ConvertWorldSpaces(Vector3 inPoint, Camera inCamera, Camera outCamera)
    {
      Vector3 viewportPoint = inCamera.WorldToViewportPoint(inPoint);
      return outCamera.ViewportToWorldPoint(viewportPoint);
    }

    private void Update()
    {
      if (!(bool) (UnityEngine.Object) this.followTransform)
      {
        this.enabled = false;
      }
      else
      {
        this.transform.position = dfFollowObject.ConvertWorldSpaces(this.followTransform.position + this.offset, this.mainCamera, this.manager.RenderCamera).WithZ(0.0f);
        this.transform.position = this.transform.position.QuantizeFloor(this.myControl.PixelsToUnits() / (Pixelator.Instance.ScaleTileScale / Pixelator.Instance.CurrentTileScale));
      }
    }

    private Vector2 getAnchoredControlPosition()
    {
      float height = this.myControl.Height;
      float num1 = this.myControl.Width / 2f;
      float width = this.myControl.Width;
      float num2 = this.myControl.Height / 2f;
      Vector2 anchoredControlPosition = new Vector2();
      switch (this.anchor)
      {
        case dfPivotPoint.TopLeft:
          anchoredControlPosition.x = width;
          anchoredControlPosition.y = height;
          break;
        case dfPivotPoint.TopCenter:
          anchoredControlPosition.x = num1;
          anchoredControlPosition.y = height;
          break;
        case dfPivotPoint.TopRight:
          anchoredControlPosition.x = 0.0f;
          anchoredControlPosition.y = height;
          break;
        case dfPivotPoint.MiddleLeft:
          anchoredControlPosition.x = width;
          anchoredControlPosition.y = num2;
          break;
        case dfPivotPoint.MiddleCenter:
          anchoredControlPosition.x = num1;
          anchoredControlPosition.y = num2;
          break;
        case dfPivotPoint.MiddleRight:
          anchoredControlPosition.x = 0.0f;
          anchoredControlPosition.y = num2;
          break;
        case dfPivotPoint.BottomLeft:
          anchoredControlPosition.x = width;
          anchoredControlPosition.y = 0.0f;
          break;
        case dfPivotPoint.BottomCenter:
          anchoredControlPosition.x = num1;
          anchoredControlPosition.y = 0.0f;
          break;
        case dfPivotPoint.BottomRight:
          anchoredControlPosition.x = 0.0f;
          anchoredControlPosition.y = 0.0f;
          break;
      }
      return anchoredControlPosition;
    }
  }

