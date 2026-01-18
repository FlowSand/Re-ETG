using System.Collections;
using System.Diagnostics;

using UnityEngine;

#nullable disable

public class MeshDissolver : MonoBehaviour
    {
        public void DissolveMesh(Vector2 startPosition, float duration)
        {
            this.StartCoroutine(this.Dissolve(startPosition, duration));
        }

        [DebuggerHidden]
        private IEnumerator Dissolve(Vector2 startPosition, float duration)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new MeshDissolver__Dissolvec__Iterator0()
            {
                startPosition = startPosition,
                duration = duration,
                _this = this
            };
        }
    }

