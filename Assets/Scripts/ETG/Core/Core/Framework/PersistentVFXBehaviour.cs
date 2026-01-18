using System.Collections.Generic;

using UnityEngine;

#nullable disable

public class PersistentVFXBehaviour : BraveBehaviour
    {
        public void BecomeDebris(
            Vector3 startingForce,
            float startingHeight,
            params System.Type[] keepComponents)
        {
            List<System.Type> typeList = new List<System.Type>((IEnumerable<System.Type>) keepComponents);
            foreach (Component component in this.GetComponents<Component>())
            {
                int num;
                switch (component)
                {
                    case tk2dBaseSprite _:
                    case tk2dSpriteAnimator _:
                    case Renderer _:
                    case MeshFilter _:
                    case DebrisObject _:
                    case SpriteAnimatorStopper _:
                    case Transform _:
                        num = 1;
                        break;
                    default:
                        num = typeList.Contains(component.GetType()) ? 1 : 0;
                        break;
                }
                if (num == 0)
                    UnityEngine.Object.Destroy((UnityEngine.Object) component);
            }
            DebrisObject orAddComponent = this.gameObject.GetOrAddComponent<DebrisObject>();
            orAddComponent.angularVelocity = 45f;
            orAddComponent.angularVelocityVariance = 20f;
            orAddComponent.decayOnBounce = 0.5f;
            orAddComponent.bounceCount = 1;
            orAddComponent.canRotate = true;
            orAddComponent.Trigger(startingForce, startingHeight);
        }

        protected override void OnDestroy() => base.OnDestroy();
    }

