using System.Collections.Generic;

using UnityEngine;

#nullable disable

[RequireComponent(typeof (GenericIntroDoer))]
public class BulletKingIntroDoer : SpecificIntroDoer
    {
        protected override void OnDestroy() => base.OnDestroy();

        public override void StartIntro(List<tk2dSpriteAnimator> animators)
        {
            BulletKingToadieController[] objectsOfType = Object.FindObjectsOfType<BulletKingToadieController>();
            for (int index = 0; index < objectsOfType.Length; ++index)
            {
                animators.Add(objectsOfType[index].spriteAnimator);
                if ((bool) (Object) objectsOfType[index].scepterAnimator)
                    animators.Add(objectsOfType[index].scepterAnimator);
            }
        }
    }

