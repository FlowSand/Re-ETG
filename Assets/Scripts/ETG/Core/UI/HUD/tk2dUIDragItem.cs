// Decompiled with JetBrains decompiler
// Type: tk2dUIDragItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.UI.HUD
{
    [AddComponentMenu("2D Toolkit/UI/tk2dUIDragItem")]
    public class tk2dUIDragItem : tk2dUIBaseItemControl
    {
      public tk2dUIManager uiManager;
      private Vector3 offset = Vector3.zero;
      private bool isBtnActive;

      private void OnEnable()
      {
        if (!(bool) (UnityEngine.Object) this.uiItem)
          return;
        this.uiItem.OnDown += new System.Action(this.ButtonDown);
        this.uiItem.OnRelease += new System.Action(this.ButtonRelease);
      }

      private void OnDisable()
      {
        if ((bool) (UnityEngine.Object) this.uiItem)
        {
          this.uiItem.OnDown -= new System.Action(this.ButtonDown);
          this.uiItem.OnRelease -= new System.Action(this.ButtonRelease);
        }
        if (!this.isBtnActive)
          return;
        if ((UnityEngine.Object) tk2dUIManager.Instance__NoCreate != (UnityEngine.Object) null)
          tk2dUIManager.Instance.OnInputUpdate -= new System.Action(this.UpdateBtnPosition);
        this.isBtnActive = false;
      }

      private void UpdateBtnPosition() => this.transform.position = this.CalculateNewPos();

      private Vector3 CalculateNewPos()
      {
        Vector2 position = this.uiItem.Touch.position;
        Camera cameraForControl = tk2dUIManager.Instance.GetUICameraForControl(this.gameObject);
        return cameraForControl.ScreenToWorldPoint(new Vector3(position.x, position.y, this.transform.position.z - cameraForControl.transform.position.z)) with
        {
          z = this.transform.position.z
        } + this.offset;
      }

      public void ButtonDown()
      {
        if (!this.isBtnActive)
          tk2dUIManager.Instance.OnInputUpdate += new System.Action(this.UpdateBtnPosition);
        this.isBtnActive = true;
        this.offset = Vector3.zero;
        this.offset = this.transform.position - this.CalculateNewPos();
      }

      public void ButtonRelease()
      {
        if (this.isBtnActive)
          tk2dUIManager.Instance.OnInputUpdate -= new System.Action(this.UpdateBtnPosition);
        this.isBtnActive = false;
      }
    }

}
