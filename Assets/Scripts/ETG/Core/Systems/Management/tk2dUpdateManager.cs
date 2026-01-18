using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using UnityEngine;

#nullable disable

[AddComponentMenu("")]
public class tk2dUpdateManager : MonoBehaviour
    {
        private static tk2dUpdateManager inst;
        [SerializeField]
        private List<tk2dTextMesh> textMeshes = new List<tk2dTextMesh>(64);

        private static tk2dUpdateManager Instance
        {
            get
            {
                if ((Object) tk2dUpdateManager.inst == (Object) null)
                {
                    tk2dUpdateManager.inst = Object.FindObjectOfType(typeof (tk2dUpdateManager)) as tk2dUpdateManager;
                    if ((Object) tk2dUpdateManager.inst == (Object) null)
                    {
                        GameObject target = new GameObject("@tk2dUpdateManager");
                        target.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
                        tk2dUpdateManager.inst = target.AddComponent<tk2dUpdateManager>();
                        Object.DontDestroyOnLoad((Object) target);
                    }
                }
                return tk2dUpdateManager.inst;
            }
        }

        public static void QueueCommit(tk2dTextMesh textMesh)
        {
            tk2dUpdateManager.Instance.QueueCommitInternal(textMesh);
        }

        public static void FlushQueues() => tk2dUpdateManager.Instance.FlushQueuesInternal();

        private void OnEnable() => this.StartCoroutine(this.coSuperLateUpdate());

        private void LateUpdate() => this.FlushQueuesInternal();

        [DebuggerHidden]
        private IEnumerator coSuperLateUpdate()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new tk2dUpdateManager__coSuperLateUpdatec__Iterator0()
            {
                _this = this
            };
        }

        private void QueueCommitInternal(tk2dTextMesh textMesh) => this.textMeshes.Add(textMesh);

        private void FlushQueuesInternal()
        {
            int count = this.textMeshes.Count;
            for (int index = 0; index < count; ++index)
            {
                tk2dTextMesh textMesh = this.textMeshes[index];
                if ((Object) textMesh != (Object) null)
                    textMesh.DoNotUse__CommitInternal();
            }
            this.textMeshes.Clear();
        }
    }

