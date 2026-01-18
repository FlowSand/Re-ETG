using System;

using UnityEngine;

#nullable disable

[Serializable]
public class VFXPool
    {
        public VFXPoolType type;
        public VFXComplex[] effects;
        private int m_iterator;

        public VFXComplex GetEffect()
        {
            if (this.effects == null || this.effects.Length == 0)
                return (VFXComplex) null;
            switch (this.type)
            {
                case VFXPoolType.None:
                    return (VFXComplex) null;
                case VFXPoolType.All:
                    return this.effects[0];
                case VFXPoolType.SequentialGroups:
                    VFXComplex effect = this.effects[this.m_iterator];
                    this.m_iterator = (this.m_iterator + 1) % this.effects.Length;
                    return effect;
                case VFXPoolType.RandomGroups:
                    return this.effects[UnityEngine.Random.Range(0, this.effects.Length)];
                case VFXPoolType.Single:
                    return this.effects[0];
                default:
                    Debug.LogWarning((object) ("Unknown VFXPoolType " + (object) this.type));
                    return (VFXComplex) null;
            }
        }

        public void SpawnAtPosition(
            float xPosition,
            float yPositionAtGround,
            float heightOffGround,
            float zRotation,
            Transform parent = null,
            Vector2? sourceNormal = null,
            Vector2? sourceVelocity = null,
            bool keepReferences = false,
            VFXComplex.SpawnMethod spawnMethod = null,
            bool ignoresPools = false)
        {
            this.GetEffect()?.SpawnAtPosition(xPosition, yPositionAtGround, heightOffGround, zRotation, parent, sourceNormal, sourceVelocity, keepReferences, spawnMethod, ignoresPools);
        }

        public void SpawnAtPosition(
            Vector3 position,
            float zRotation = 0.0f,
            Transform parent = null,
            Vector2? sourceNormal = null,
            Vector2? sourceVelocity = null,
            float? heightOffGround = null,
            bool keepReferences = false,
            VFXComplex.SpawnMethod spawnMethod = null,
            tk2dBaseSprite spriteParent = null,
            bool ignoresPools = false)
        {
            this.GetEffect()?.SpawnAtPosition(position, zRotation, parent, sourceNormal, sourceVelocity, heightOffGround, keepReferences, spawnMethod, spriteParent, ignoresPools);
        }

        public void SpawnAtTilemapPosition(
            Vector3 position,
            float yPositionAtGround,
            float zRotation,
            Vector2 sourceNormal,
            Vector2 sourceVelocity,
            bool keepReferences = false,
            VFXComplex.SpawnMethod spawnMethod = null,
            bool ignoresPools = false)
        {
            VFXComplex effect = this.GetEffect();
            if (effect == null)
                return;
            float heightOffGround = position.y - yPositionAtGround;
            effect.SpawnAtPosition(position.x, yPositionAtGround, heightOffGround, zRotation, sourceNormal: new Vector2?(sourceNormal), sourceVelocity: new Vector2?(sourceVelocity), keepReferences: keepReferences, spawnMethod: spawnMethod, ignoresPools: ignoresPools);
        }

        public void SpawnAtLocalPosition(
            Vector3 localPosition,
            float zRotation,
            Transform parent,
            Vector2? sourceNormal = null,
            Vector2? sourceVelocity = null,
            bool keepReferences = false,
            VFXComplex.SpawnMethod spawnMethod = null,
            bool ignoresPools = false)
        {
            this.GetEffect()?.SpawnAtLocalPosition(localPosition, zRotation, parent, sourceNormal, sourceVelocity, keepReferences, spawnMethod, ignoresPools);
        }

        public void RemoveDespawnedVfx()
        {
            for (int index = 0; index < this.effects.Length; ++index)
                this.effects[index].RemoveDespawnedVfx();
        }

        public void DestroyAll()
        {
            for (int index = 0; index < this.effects.Length; ++index)
                this.effects[index].DestroyAll();
        }

        public void ForEach(Action<GameObject> action)
        {
            for (int index = 0; index < this.effects.Length; ++index)
                this.effects[index].ForEach(action);
        }

        public void ToggleRenderers(bool value)
        {
            for (int index = 0; index < this.effects.Length; ++index)
                this.effects[index].ToggleRenderers(value);
        }

        public void SetHeightOffGround(float height)
        {
            for (int index = 0; index < this.effects.Length; ++index)
                this.effects[index].SetHeightOffGround(height);
        }
    }

