using System;

using UnityEngine;

// 기지가 가지고 있는 컴포넌트 정리
// Transform, SpriteRenderer, Animator, Rigidbody2D, XXXCollider2D
// BaseStat, BasecampAnimatorController

namespace DefenceGameSystem.OS.API
{
    public class Basecamp
    {
        private static readonly int c_MIN_LEVEL = 1;
        private static readonly int c_MAX_LEVEL = 100;

        public Team team => m_team;
        public Vector3 position => m_transform.position;
        public float health => m_battleModule.health;
        public float maxHealth => m_baseStat.stat.health;
        public float manaNow => m_manaNow;
        public float manaMax => m_manaMax;

        public float manaSpeed = 5.0f;
        private float m_manaNow;
        private float m_manaMax;

        // Components
        private GameObject m_gameObject;
        private Transform m_transform;
        private Rigidbody2D m_physics;
        private BaseStat m_baseStat;
        // private BasecampAnimatorController m_animatorCtrl;
        private BattleModule m_battleModule;

        // inner States
        private int m_level;
        private Team m_team;
        private Vector3 m_tempVector3;
        private Stat m_stat;
        private bool m_isAutomated = false;

        public Basecamp(GameObject gameObject, Team team, Vector3 position, int level)
        {
            this.m_gameObject = GameObject.Instantiate(gameObject);
            this.m_transform = m_gameObject.GetComponent<Transform>();
            this.m_physics = m_gameObject.GetComponent<Rigidbody2D>();
            this.m_baseStat = m_gameObject.GetComponent<BaseStat>();
            // this.m_animatorCtrl = m_gameObject.GetComponent<BasecampAnimatorController>();
            this.m_battleModule = m_gameObject.GetComponent<BattleModule>();

            this.m_level = Basecamp.s_m_GetLevel(level, c_MIN_LEVEL, c_MAX_LEVEL);
            this.m_team = team;

            this.m_tempVector3 = Vector3.zero;
            this.m_stat = m_baseStat.stat;

            this.m_transform.position = position;

            this.m_battleModule.health = m_baseStat.stat.health;
            this.m_battleModule.team = team;
            this.m_manaNow = 0.0f;

            this.UpdateStat();
        }

        public void FixedUpdateMana(float deltaTime)
        {
            this.m_manaNow = Mathf.Clamp(this.m_manaNow + manaSpeed * deltaTime, 0, this.m_manaMax);
        }

        public void GenerateAI(float deltaTime, UnitFactory factory, UnitManager manager)
        {
            if(!m_isAutomated)
            {
                m_aiGenerateCooltime = 0.5f;
                return;
            }

            m_aiGenerateCooltime -= deltaTime;

            if(m_aiGenerateCooltime > 0.0f) return;

            string name = factory.GetRandomName();
            float removeMana = factory.CanGetUnit(name, m_manaNow);
            System.Random rand = new System.Random();

            Func<float, float, double, float> getCooltime = (min, max, constant) => (max - min) * (float)constant + min;

            if(removeMana < 0.0f)
            {
                m_aiGenerateCooltime = getCooltime(0.5f, 2.0f, rand.NextDouble());

                return;
            }

            Unit gotUnit = factory.Get(name, this, m_level, true);
            manager.Add(gotUnit);
            this.RemoveMana(removeMana);
            m_aiGenerateCooltime = getCooltime(1.0f, 4.0f, rand.NextDouble());
        }

        private float m_aiGenerateCooltime;

        public void UpdateStat()
        {
            this.m_stat.attack = 0.0f;
            this.m_stat.moveSpeed = 0.0f;
            this.m_stat.health = m_baseStat.stat.health + 10.0f * (m_level - 1.0f);
            this.m_stat.mana = m_baseStat.stat.mana + 30.0f * (m_level - 1.0f);

            this.m_manaMax = this.m_stat.mana;
        }

        public void AddLevel(int level)
        {
            if(level < 1) return;

            this.m_level = Basecamp.s_m_GetLevel(m_level + level, c_MIN_LEVEL, c_MAX_LEVEL);
        }

        public void RemoveLevel(int level)
        {
            if(level < 1) return;

            this.m_level = Basecamp.s_m_GetLevel(m_level - level, c_MIN_LEVEL, c_MAX_LEVEL);
        }

        public void GetDamage(float damage)
        {
            if(damage < 0) return;

            m_battleModule.GetDamage(damage);
        }

        public void RemoveMana(float mana)
        {
            if(mana < 0) return;

            m_manaNow -= mana;
        }

        public void SetAutomate(bool isAutomated)
        {
            m_isAutomated = isAutomated;
        }

        private static int s_m_GetLevel(int level, int min, int max)
        {
            return level < min ? min : (level > max ? max : level);
        }
    }
}