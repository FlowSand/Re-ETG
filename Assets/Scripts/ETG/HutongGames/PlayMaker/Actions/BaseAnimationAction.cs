// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.BaseAnimationAction
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  public abstract class BaseAnimationAction : ComponentAction<Animation>
  {
    public override void OnActionTargetInvoked(object targetObject)
    {
      AnimationClip clip = targetObject as AnimationClip;
      if ((Object) clip == (Object) null)
        return;
      Animation component = this.Owner.GetComponent<Animation>();
      if (!((Object) component != (Object) null))
        return;
      component.AddClip(clip, clip.name);
    }
  }
}
