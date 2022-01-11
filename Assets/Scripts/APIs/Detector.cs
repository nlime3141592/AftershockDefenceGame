using System;
using System.Collections.Generic;

using UnityEngine;

namespace DefenceGameSystem.OS.API
{
    public class Detector : MonoBehaviour
    {
        public int targetedCount => m_targetingEntities.Count;
        public List<BattleModule> targetedEntities => m_targetingEntities;

        public LayerMask targetLayer;
        public int maxTargetingCount = 15;
        public Team targetTeam;

        private List<BattleModule> m_targetingEntities;
        private List<BattleModule> m_detectedEntities;

        private void Awake()
        {
            m_targetingEntities = new List<BattleModule>();
            m_detectedEntities = new List<BattleModule>();
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            int colliderLayer;
            Detectee detectee;
            BattleModule battleModule;
            bool canAdd;

            detectee = default(Detectee);
            battleModule = default(BattleModule);

            try
            {
                colliderLayer = (1 << collider.gameObject.layer);
                canAdd = true;

                canAdd &= ((targetLayer.value & colliderLayer) != 0);
                canAdd &= collider.gameObject.TryGetComponent<Detectee>(out detectee);
                canAdd &= detectee.CanGetComponent<BattleModule>(out battleModule);
                canAdd &= (battleModule.team == targetTeam);
                canAdd &= !m_Contains(battleModule);
            }
            catch(Exception)
            {
                canAdd = false;
            }

            if(canAdd)
            {
                m_Add(battleModule);
            }
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            int colliderLayer;
            Detectee detectee;
            BattleModule battleModule;
            bool canRemove;

            detectee = default(Detectee);
            battleModule = default(BattleModule);

            try
            {
                colliderLayer = (1 << collider.gameObject.layer);
                canRemove = true;

                canRemove &= ((targetLayer.value & colliderLayer) != 0);
                canRemove &= collider.gameObject.TryGetComponent<Detectee>(out detectee);
                canRemove &= detectee.CanGetComponent<BattleModule>(out battleModule);
                canRemove &= m_Contains(battleModule);
            }
            catch(Exception)
            {
                canRemove = false;
            }

            if(canRemove)
            {
                m_Remove(battleModule);
            }
        }

        public void OnDie(BattleModule module)
        {
            m_Remove(module);
        }

        private bool m_Contains(BattleModule module)
        {
            return (m_targetingEntities.Contains(module) || m_detectedEntities.Contains(module));
        }

        private void m_Add(BattleModule module)
        {
            if(m_Contains(module))
            {
                return;
            }
            else if(m_targetingEntities.Count == maxTargetingCount)
            {
                m_detectedEntities.Add(module);
            }
            else if(m_detectedEntities.Count == 0)
            {
                m_targetingEntities.Add(module);
            }
            else
            {
                BattleModule battleModule;

                // 정렬 알고리즘이 여기 온다.

                while(m_detectedEntities.Count > 0 && m_targetingEntities.Count < maxTargetingCount)
                {
                    battleModule = m_detectedEntities[0];
                    m_targetingEntities.Add(battleModule);
                    m_detectedEntities.Remove(battleModule);
                }
            }
        }

        private void m_Remove(BattleModule module)
        {
            if(m_targetingEntities.Contains(module)) m_targetingEntities.Remove(module);
            if(m_detectedEntities.Contains(module)) m_detectedEntities.Remove(module);
        }
    }
}