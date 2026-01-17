// Decompiled with JetBrains decompiler
// Type: FullInspector.tkTypeProxy`4
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace FullInspector
{
  public class tkTypeProxy<TFrom, TContextFrom, TTo, TContextTo> : tkControl<TTo, TContextTo>
  {
    private tkControl<TFrom, TContextFrom> _control;

    public tkTypeProxy(tkControl<TFrom, TContextFrom> control) => this._control = control;

    private static T Cast<T>(object val) => (T) val;

    public override bool ShouldShow(TTo obj, TContextTo context, fiGraphMetadata metadata)
    {
      return this._control.ShouldShow(tkTypeProxy<TFrom, TContextFrom, TTo, TContextTo>.Cast<TFrom>((object) obj), tkTypeProxy<TFrom, TContextFrom, TTo, TContextTo>.Cast<TContextFrom>((object) context), metadata);
    }

    protected override TTo DoEdit(Rect rect, TTo obj, TContextTo context, fiGraphMetadata metadata)
    {
      return tkTypeProxy<TFrom, TContextFrom, TTo, TContextTo>.Cast<TTo>((object) this._control.Edit(rect, tkTypeProxy<TFrom, TContextFrom, TTo, TContextTo>.Cast<TFrom>((object) obj), tkTypeProxy<TFrom, TContextFrom, TTo, TContextTo>.Cast<TContextFrom>((object) context), metadata));
    }

    protected override float DoGetHeight(TTo obj, TContextTo context, fiGraphMetadata metadata)
    {
      return this._control.GetHeight(tkTypeProxy<TFrom, TContextFrom, TTo, TContextTo>.Cast<TFrom>((object) obj), tkTypeProxy<TFrom, TContextFrom, TTo, TContextTo>.Cast<TContextFrom>((object) context), metadata);
    }
  }
}
