// Decompiled with JetBrains decompiler
// Type: tk2dButton
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [AddComponentMenu("2D Toolkit/Deprecated/GUI/tk2dButton")]
    public class tk2dButton : MonoBehaviour
    {
      public Camera viewCamera;
      public string buttonDownSprite = "button_down";
      public string buttonUpSprite = "button_up";
      public string buttonPressedSprite = "button_up";
      private int buttonDownSpriteId = -1;
      private int buttonUpSpriteId = -1;
      private int buttonPressedSpriteId = -1;
      public AudioClip buttonDownSound;
      public AudioClip buttonUpSound;
      public AudioClip buttonPressedSound;
      public GameObject targetObject;
      public string messageName = string.Empty;
      private tk2dBaseSprite sprite;
      private bool buttonDown;
      public float targetScale = 1.1f;
      public float scaleTime = 0.05f;
      public float pressedWaitTime = 0.3f;

      public event tk2dButton.ButtonHandlerDelegate ButtonPressedEvent;

      public event tk2dButton.ButtonHandlerDelegate ButtonAutoFireEvent;

      public event tk2dButton.ButtonHandlerDelegate ButtonDownEvent;

      public event tk2dButton.ButtonHandlerDelegate ButtonUpEvent;

      private void OnEnable() => this.buttonDown = false;

      private void Start()
      {
        if ((Object) this.viewCamera == (Object) null)
        {
          Transform transform = this.transform;
          while ((bool) (Object) transform && (Object) transform.GetComponent<Camera>() == (Object) null)
            transform = transform.parent;
          if ((bool) (Object) transform && (Object) transform.GetComponent<Camera>() != (Object) null)
            this.viewCamera = transform.GetComponent<Camera>();
          if ((Object) this.viewCamera == (Object) null && (bool) (Object) tk2dCamera.Instance)
            this.viewCamera = tk2dCamera.Instance.GetComponent<Camera>();
          if ((Object) this.viewCamera == (Object) null)
            this.viewCamera = Camera.main;
        }
        this.sprite = this.GetComponent<tk2dBaseSprite>();
        if ((bool) (Object) this.sprite)
          this.UpdateSpriteIds();
        if ((Object) this.GetComponent<Collider>() == (Object) null)
        {
          BoxCollider boxCollider = this.gameObject.AddComponent<BoxCollider>();
          Vector3 size = boxCollider.size with { z = 0.2f };
          boxCollider.size = size;
        }
        if (!((Object) this.buttonDownSound != (Object) null) && !((Object) this.buttonPressedSound != (Object) null) && !((Object) this.buttonUpSound != (Object) null) || !((Object) this.GetComponent<AudioSource>() == (Object) null))
          return;
        this.gameObject.AddComponent<AudioSource>().playOnAwake = false;
      }

      public void UpdateSpriteIds()
      {
        this.buttonDownSpriteId = this.buttonDownSprite.Length <= 0 ? -1 : this.sprite.GetSpriteIdByName(this.buttonDownSprite);
        this.buttonUpSpriteId = this.buttonUpSprite.Length <= 0 ? -1 : this.sprite.GetSpriteIdByName(this.buttonUpSprite);
        this.buttonPressedSpriteId = this.buttonPressedSprite.Length <= 0 ? -1 : this.sprite.GetSpriteIdByName(this.buttonPressedSprite);
      }

      private void PlaySound(AudioClip source)
      {
        if (!(bool) (Object) this.GetComponent<AudioSource>() || !(bool) (Object) source)
          return;
        this.GetComponent<AudioSource>().PlayOneShot(source);
      }

      [DebuggerHidden]
      private IEnumerator coScale(Vector3 defaultScale, float startScale, float endScale)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new tk2dButton.<coScale>c__Iterator0()
        {
          defaultScale = defaultScale,
          startScale = startScale,
          endScale = endScale,
          _this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator LocalWaitForSeconds(float seconds)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new tk2dButton.<LocalWaitForSeconds>c__Iterator1()
        {
          seconds = seconds
        };
      }

      [DebuggerHidden]
      private IEnumerator coHandleButtonPress(int fingerId)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new tk2dButton.<coHandleButtonPress>c__Iterator2()
        {
          fingerId = fingerId,
          _this = this
        };
      }

      private void Update()
      {
        if (this.buttonDown)
          return;
        bool flag = false;
        if (Input.multiTouchEnabled)
        {
          for (int index = 0; index < Input.touchCount; ++index)
          {
            Touch touch = Input.GetTouch(index);
            if (touch.phase == TouchPhase.Began)
            {
              Ray ray = this.viewCamera.ScreenPointToRay((Vector3) touch.position);
              RaycastHit hitInfo;
              if (this.GetComponent<Collider>().Raycast(ray, out hitInfo, 1E+08f) && !Physics.Raycast(ray, hitInfo.distance - 0.01f))
              {
                this.StartCoroutine(this.coHandleButtonPress(touch.fingerId));
                flag = true;
                break;
              }
            }
          }
        }
        if (flag || !Input.GetMouseButtonDown(0))
          return;
        Ray ray1 = this.viewCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo1;
        if (!this.GetComponent<Collider>().Raycast(ray1, out hitInfo1, 1E+08f) || Physics.Raycast(ray1, hitInfo1.distance - 0.01f))
          return;
        this.StartCoroutine(this.coHandleButtonPress(-1));
      }

      public delegate void ButtonHandlerDelegate(tk2dButton source);
    }

}
