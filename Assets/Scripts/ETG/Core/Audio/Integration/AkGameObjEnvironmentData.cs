using System.Collections.Generic;

using UnityEngine;

#nullable disable

public class AkGameObjEnvironmentData
    {
        private readonly List<AkEnvironment> activeEnvironments = new List<AkEnvironment>();
        private readonly List<AkEnvironment> activeEnvironmentsFromPortals = new List<AkEnvironment>();
        private readonly List<AkEnvironmentPortal> activePortals = new List<AkEnvironmentPortal>();
        private readonly AkAuxSendArray auxSendValues = new AkAuxSendArray();
        private Vector3 lastPosition = Vector3.zero;
        private bool hasEnvironmentListChanged = true;
        private bool hasActivePortalListChanged = true;
        private bool hasSentZero;

        private void AddHighestPriorityEnvironmentsFromPortals(Vector3 position)
        {
            for (int index1 = 0; index1 < this.activePortals.Count; ++index1)
            {
                for (int index2 = 0; index2 < 2; ++index2)
                {
                    AkEnvironment environment = this.activePortals[index1].environments[index2];
                    if ((Object) environment != (Object) null)
                    {
                        int num = this.activeEnvironmentsFromPortals.BinarySearch(environment, (IComparer<AkEnvironment>) AkEnvironment.s_compareByPriority);
                        if (num >= 0 && num < 4)
                        {
                            this.auxSendValues.Add(environment.GetAuxBusID(), this.activePortals[index1].GetAuxSendValueForPosition(position, index2));
                            if (this.auxSendValues.isFull)
                                return;
                        }
                    }
                }
            }
        }

        private void AddHighestPriorityEnvironments(Vector3 position)
        {
            if (this.auxSendValues.isFull || this.auxSendValues.Count() >= this.activeEnvironments.Count)
                return;
            for (int index = 0; index < this.activeEnvironments.Count; ++index)
            {
                AkEnvironment activeEnvironment = this.activeEnvironments[index];
                uint auxBusId = activeEnvironment.GetAuxBusID();
                if ((!activeEnvironment.isDefault || index == 0) && !this.auxSendValues.Contains(auxBusId))
                {
                    this.auxSendValues.Add(auxBusId, activeEnvironment.GetAuxSendValueForPosition(position));
                    if (activeEnvironment.excludeOthers || this.auxSendValues.isFull)
                        break;
                }
            }
        }

        public void UpdateAuxSend(GameObject gameObject, Vector3 position)
        {
            if (!this.hasEnvironmentListChanged && !this.hasActivePortalListChanged && this.lastPosition == position)
                return;
            this.auxSendValues.Reset();
            this.AddHighestPriorityEnvironmentsFromPortals(position);
            this.AddHighestPriorityEnvironments(position);
            bool flag = this.auxSendValues.Count() == 0;
            if (!this.hasSentZero || !flag)
            {
                int num = (int) AkSoundEngine.SetEmitterAuxSendValues(gameObject, this.auxSendValues, (uint) this.auxSendValues.Count());
            }
            this.hasSentZero = flag;
            this.lastPosition = position;
            this.hasActivePortalListChanged = false;
            this.hasEnvironmentListChanged = false;
        }

        private void TryAddEnvironment(AkEnvironment env)
        {
            if (!((Object) env != (Object) null))
                return;
            int num1 = this.activeEnvironmentsFromPortals.BinarySearch(env, (IComparer<AkEnvironment>) AkEnvironment.s_compareByPriority);
            if (num1 >= 0)
                return;
            this.activeEnvironmentsFromPortals.Insert(~num1, env);
            int num2 = this.activeEnvironments.BinarySearch(env, (IComparer<AkEnvironment>) AkEnvironment.s_compareBySelectionAlgorithm);
            if (num2 < 0)
                this.activeEnvironments.Insert(~num2, env);
            this.hasEnvironmentListChanged = true;
        }

        private void RemoveEnvironment(AkEnvironment env)
        {
            this.activeEnvironmentsFromPortals.Remove(env);
            this.activeEnvironments.Remove(env);
            this.hasEnvironmentListChanged = true;
        }

        public void AddAkEnvironment(Collider environmentCollider, Collider gameObjectCollider)
        {
            AkEnvironmentPortal component = environmentCollider.GetComponent<AkEnvironmentPortal>();
            if ((Object) component != (Object) null)
            {
                this.activePortals.Add(component);
                this.hasActivePortalListChanged = true;
                for (int index = 0; index < 2; ++index)
                    this.TryAddEnvironment(component.environments[index]);
            }
            else
                this.TryAddEnvironment(environmentCollider.GetComponent<AkEnvironment>());
        }

        private bool AkEnvironmentBelongsToActivePortals(AkEnvironment env)
        {
            for (int index1 = 0; index1 < this.activePortals.Count; ++index1)
            {
                for (int index2 = 0; index2 < 2; ++index2)
                {
                    if ((Object) env == (Object) this.activePortals[index1].environments[index2])
                        return true;
                }
            }
            return false;
        }

        public void RemoveAkEnvironment(Collider environmentCollider, Collider gameObjectCollider)
        {
            AkEnvironmentPortal component1 = environmentCollider.GetComponent<AkEnvironmentPortal>();
            if ((Object) component1 != (Object) null)
            {
                for (int index = 0; index < 2; ++index)
                {
                    AkEnvironment environment = component1.environments[index];
                    if ((Object) environment != (Object) null && !gameObjectCollider.bounds.Intersects(environment.GetCollider().bounds))
                        this.RemoveEnvironment(environment);
                }
                this.activePortals.Remove(component1);
                this.hasActivePortalListChanged = true;
            }
            else
            {
                AkEnvironment component2 = environmentCollider.GetComponent<AkEnvironment>();
                if (!((Object) component2 != (Object) null) || this.AkEnvironmentBelongsToActivePortals(component2))
                    return;
                this.RemoveEnvironment(component2);
            }
        }
    }

