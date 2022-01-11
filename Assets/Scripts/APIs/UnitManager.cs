using System;
using System.Collections.Generic;

using DefenceGameSystem.OS.Kernel;

namespace DefenceGameSystem.OS.API
{
    public class UnitManager
    {
        private List<Unit> m_units;
        private Queue<int> m_indexes;
        private Unit m_tempUnit;
        private int i, length;

        public UnitManager()
        {
            m_units = new List<Unit>();
            m_indexes = new Queue<int>();
        }

        public void Add(Unit unit)
        {
            if(m_units.Contains(unit)) return;

            m_units.Add(unit);
        }

        public void Remove(Unit unit)
        {
            if(!m_units.Contains(unit)) return;

            m_units.Remove(unit);
        }

        public void Move(float deltaTime, AxisType handyAxisType)
        {
            length = m_units.Count;

            for(i = 0; i < length; i++)
            {
                m_MoveUnitAI(m_units[i], deltaTime, handyAxisType);
            }
        }

        public void UseSkill()
        {
            length = m_units.Count;

            for(i = 0; i < length; i++)
            {
                m_UseSkillAI(m_units[i]);
            }
        }

        public void UpdateStat()
        {
            length = m_units.Count;

            for(i = 0; i < length; i++)
            {
                m_units[i].UpdateStat();
            }
        }

        public void UpdateHealth()
        {
            length = m_units.Count;

            for(i = 0; i < length; i++)
            {
                if(m_units[i].isDied())
                {
                    m_indexes.Enqueue(i);
                }
            }

            while(m_indexes.Count > 0)
            {
                try
                {
                    int index = m_indexes.Dequeue();
                    Unit unit = m_units[index];

                    this.Remove(unit);
                    unit.implementObject.SetActive(false);
                }
                catch(Exception)
                {
                    
                }
            }
        }

        private bool s_m_CanMove(Unit unit)
        {
            return true;
        }

        private bool s_m_CanUseSkill(Unit unit)
        {
            return true;
        }

        private void m_MoveUnitAI(Unit unit, float deltaTime, AxisType handyAxisType)
        {
            if(unit.isRectricted)
            {
                return;
            }
            else if(unit.targetedCount > 0)
            {
                return;
            }
            else if(unit.isAutomated && s_m_CanMove(unit))
            {
                unit.Move(deltaTime, handyAxisType);
            }
            else if(!unit.isAutomated && s_m_CanMove(unit))
            {
                unit.Move(deltaTime, (AxisType)InputModule.GetAxis("Joystick Horizontal"));
            }
            else
            {
                return;
            }
        }

        public void m_UseSkillAI(Unit unit)
        {
            if(unit.isAutomated)
            {
                if(unit.targetedCount > 0)
                {
                    unit.skillModule.UseSkillByAutomatically(unit.battleModule, unit.targetedEntities, unit.targetedCount);
                }
            }
            else
            {

            }
        }
    }
}