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
