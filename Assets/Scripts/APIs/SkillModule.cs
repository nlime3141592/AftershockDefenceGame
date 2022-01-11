using System.Collections.Generic;

using UnityEngine;

namespace DefenceGameSystem.OS.API
{
    [DisallowMultipleComponent]
    public class SkillModule : MonoBehaviour
    {
        public List<SkillData> skillDatas;
        public int higherPrioritySkill;

        public void UpdateCooltimes(float deltaTime)
        {
            foreach(SkillData data in skillDatas)
            {
                if(data.leftCooltime > 0)
                {
                    data.leftCooltime -= deltaTime;
                }
            }
        }

        public void Use(BattleModule attacker, List<BattleModule> victims, int victimCount, int skillIndex)
        {
            if(skillDatas[skillIndex].leftCooltime > 0) return;
            if(higherPrioritySkill >= skillDatas.Count) return;

            SkillData sdata = skillDatas[skillIndex];

            sdata.leftCooltime = sdata.cooltime;

            if(sdata.mainEffect != null)
            {
                GameObject effect = GameObject.Instantiate(sdata.mainEffect);
                // effect.transform.position = attacker.transform.position + Vector3.right * 3.0f;

                m_mainEffectTransform(effect, attacker.gameObject, false);
            }
            
            for(int i = 0; i < victimCount; i++)
            {
                victims[i].GetDamage(attacker.attack);

                // if(sdata.hitEffect != null) GameObject.Instantiate(sdata.hitEffect).transform.parent = victims[i].transform;
            }
        }

        private void m_mainEffectTransform(GameObject effect, GameObject unit, bool setParent)
        {
            Detector detector = unit.GetComponentInChildren<Detector>();
            Vector3 offset = detector.gameObject.GetComponent<BoxCollider2D>().offset;

            effect.transform.position = detector.transform.position;
            
            effect.transform.parent = detector.transform;
            effect.transform.localPosition = offset;
/*
            if(setParent)
            {
                effect.transform.parent = detector.transform;
                effect.transform.localPosition = offset;
            }
            else
            {
                effect.transform.position = detector.transform.position + offset;
            }*/
        }

        public void UseSkillByAutomatically(BattleModule attacker, List<BattleModule> victims, int victimCount)
        {
            victimCount = victims.Count < victimCount ? victims.Count : victimCount;

            Use(attacker, victims, victimCount, higherPrioritySkill);
        }
    }
}