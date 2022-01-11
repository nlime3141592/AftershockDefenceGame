using System;
using System.Collections.Generic;

using UnityEngine;

using DefenceGameSystem.OS.Kernel;

namespace DefenceGameSystem.OS.API
{
    public class UnitFactory
    {
        public string this[int index]
        {
            get
            {
                if(index < 0 || index >= m_prefabNames.Count)
                    throw new IndexOutOfRangeException("Key out of range.");

                return m_prefabNames[index];
            }
        }
        
        private static readonly string c_ADDRESSABLE_ROOT_PATH = "asset.prefab.entity.";
        private Dictionary<string, GameObject> m_prefabs;
        private List<string> m_prefabNames;
        private Transform m_parent;

        public string GetRandomName()
        {
            System.Random rand = new System.Random();

            int i = rand.Next(0, m_prefabNames.Count);

            return m_prefabNames[i];
        }

        public UnitFactory()
        {
            m_prefabs = new Dictionary<string, GameObject>();
            m_prefabNames = new List<string>();
            m_parent = new GameObject("Unit Factory Container").transform;
            m_parent.position = Vector3.zero;
        }

        public float GetUnitCost(string name)
        {
            string key = s_m_GetKey(name);

            if(!m_prefabs.ContainsKey(key)) return -1.0f;

            return m_prefabs[key].GetComponent<BaseStat>().stat.mana;
        }

        public float CanGetUnit(string name, float basecampHavingMana)
        {
            string key = s_m_GetKey(name);

            if(!m_prefabs.ContainsKey(key)) return -1.0f;

            float mana = -1.0f;

            if(basecampHavingMana < m_prefabs[key].GetComponent<BaseStat>().stat.mana)
            {
                return mana;
            }
            else
            {
                mana = m_prefabs[key].GetComponent<BaseStat>().stat.mana;
                return mana;
            }
        }

        public Unit Get(string name, Basecamp camp, int level, bool isAutomated = true)
        {
            string key = s_m_GetKey(name);

            if(!m_prefabs.ContainsKey(key) || m_prefabs[key] == null) return default(Unit);

            GameObject prefab = m_prefabs[key];
            Unit unit = new Unit(prefab, camp.team, camp.position + Vector3.down, level, isAutomated);
            GameObject implement = unit.implementObject;
            implement.transform.parent = m_parent;
            implement.name = name;
            return unit;
        }

        public void Load(string name)
        {
            string key = s_m_GetKey(name);

            // 존재하지 않는 키 값인 경우 (addressable의 주소로 정의되지 않은 key일 경우)
            if(m_prefabs.ContainsKey(key)) return;

            m_prefabs.Add(key, null);

            // 여기서 유닛 게임 오브젝트 로드
            AsyncAssetLoader<GameObject> loader = new AsyncAssetLoader<GameObject>();
            loader.Completed += (result) =>
            {
                m_prefabs[key] = GameObject.Instantiate(result);
                m_prefabs[key].transform.parent = m_parent;
                
                if(!m_prefabNames.Contains(name))
                {
                    m_prefabNames.Add(name);
                }


            };
            loader.LoadAsset(key);
        }

        private static string s_m_GetKey(string name)
        {
            return string.Concat(c_ADDRESSABLE_ROOT_PATH, name).Replace(" ", "").ToLower();
        }
    }
}