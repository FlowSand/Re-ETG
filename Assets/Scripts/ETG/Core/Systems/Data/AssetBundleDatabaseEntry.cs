using System;

using UnityEngine;

#nullable disable

[Serializable]
    public abstract class AssetBundleDatabaseEntry
    {
        public string myGuid;
        public string unityGuid;
        public string path;
[NonSerialized]
        protected UnityEngine.Object loadedPrefab;

        public abstract AssetBundle assetBundle { get; }

        public string name
        {
            get => this.path.Substring(this.path.LastIndexOf('/') + 1).Replace(".prefab", string.Empty);
        }

        public bool HasLoadedPrefab => (bool) this.loadedPrefab;

        public virtual void DropReference() => this.loadedPrefab = (UnityEngine.Object) null;
    }

