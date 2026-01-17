// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.ComponentAction`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  public abstract class ComponentAction<T> : FsmStateAction where T : Component
  {
    private GameObject cachedGameObject;
    private T component;

    protected Rigidbody rigidbody => (object) this.component as Rigidbody;

    protected Rigidbody2D rigidbody2d => (object) this.component as Rigidbody2D;

    protected Renderer renderer => (object) this.component as Renderer;

    protected Animation animation => (object) this.component as Animation;

    protected AudioSource audio => (object) this.component as AudioSource;

    protected Camera camera => (object) this.component as Camera;

    protected GUIText guiText => (object) this.component as GUIText;

    protected GUITexture guiTexture => (object) this.component as GUITexture;

    protected Light light => (object) this.component as Light;

    protected NetworkView networkView => (object) this.component as NetworkView;

    protected bool UpdateCache(GameObject go)
    {
      if ((Object) go == (Object) null)
        return false;
      if ((Object) this.component == (Object) null || (Object) this.cachedGameObject != (Object) go)
      {
        this.component = go.GetComponent<T>();
        this.cachedGameObject = go;
        if ((Object) this.component == (Object) null)
          this.LogWarning($"Missing component: {typeof (T).FullName} on: {go.name}");
      }
      return (Object) this.component != (Object) null;
    }
  }
}
