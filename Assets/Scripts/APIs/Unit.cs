using System.Collections.Generic;

using UnityEngine;

using DefenceGameSystem.OS.Kernel;

// 유닛이 가지고 있는 컴포넌트 정리
// Transform, SpriteRenderer, Animator, Rigidbody2D, XXXCollider2D
// BaseStat, UnitAnimatorController, SkillModule, Movable2D
    // Detector, XXXCollider2D

namespace DefenceGameSystem.OS.API
{
    public class Unit
    {
        private static readonly int c_MIN_LEVEL = 1;
        private static readonly int c_MAX_LEVEL = 100;

        public GameObject implementObject => m_gameObject;
        public Transform transform => m_transform;
        public bool isRectricted => false; // m_animatorCtrl.isRestricted;
        public bool isAutomated => m_isAutomated;
        public Team team => m_team;
        public float health => m_battleModule.health;
        public int targetedCount => m_detector.targetedCount;
        public List<BattleModule> targetedEntities => m_detector.targetedEntities;
        public float attack => m_stat.attack;
        public Vector3 position => m_transform.position;
        public SkillModule skillModule => m_skillModule;
        public BattleModule battleModule => m_battleModule;
        public Detector detector => m_detector;

        // Components
        private GameObject m_gameObject;
        private Transform m_transform;
        // private SpriteRenderer m_renderer; // NOTE: Deprecated.
        // private Animator m_animator; // NOTE: Deprecated.
        private Rigidbody2D m_physics;
        private BaseStat m_baseStat;
        // private UnitAnimatorController m_animatorCtrl;
        private SkillModule m_skillModule;
        // private Movable2D m_movable;
        private BattleModule m_battleModule;
        private Detector m_detector;

        // inner States
        private int m_level;
        private Team m_team;
        private bool m_isAutomated;
        private Vector3 m_tempVector3;
        private float m_activeSpeed;
        private Stat m_stat;

        public Unit(GameObject gameObject, Team team, Vector3 position, int level, bool isAutomated = true)
        {
            this.m_gameObject = GameObject.Instantiate(gameObject);
            this.m_transform = m_gameObject.GetComponent<Transform>();
            this.m_physics = m_gameObject.GetComponent<Rigidbody2D>();
            this.m_baseStat = m_gameObject.GetComponent<BaseStat>();
            // this.m_animatorCtrl = m_gameObject.GetComponent<UnitAnimatorController>();
            this.m_skillModule = m_gameObject.GetComponent<SkillModule>();
            this.m_battleModule = m_gameObject.GetComponent<BattleModule>();
            this.m_detector = m_gameObject.GetComponentInChildren<Detector>();

            this.m_level = Unit.s_m_GetLevel(level, c_MIN_LEVEL, c_MAX_LEVEL);
            this.m_team = team;
            this.m_isAutomated = isAutomated;

            this.m_tempVector3 = Vector3.zero;
            this.m_activeSpeed = 0.0f;
            this.m_stat = m_baseStat.stat;

            this.m_transform.position = position;

            this.m_battleModule.health = m_baseStat.stat.health;
            this.m_battleModule.team = team;
            this.m_detector.targetTeam = team == Team.A ? Team.B : Team.A;

            this.m_detector.transform.eulerAngles = Vector3.up * (team == Team.A ? 0 : 180.0f);

            this.UpdateStat();
        }

        public Unit(Unit prefab, Vector3 position)
        {
            this.m_gameObject = GameObject.Instantiate(prefab.m_gameObject);
            this.m_transform = m_gameObject.GetComponent<Transform>();
            this.m_physics = m_gameObject.GetComponent<Rigidbody2D>();
            this.m_baseStat = m_gameObject.GetComponent<BaseStat>();
            // this.m_animatorCtrl = m_gameObject.GetComponent<UnitAnimatorController>();

            this.m_level = Unit.s_m_GetLevel(prefab.m_level, c_MIN_LEVEL, c_MAX_LEVEL);
            this.m_team = prefab.m_team;
            this.m_isAutomated = prefab.m_isAutomated;

            this.m_tempVector3 = Vector3.zero;
            this.m_activeSpeed = 0.0f;
            this.m_stat = m_baseStat.stat;

            this.m_transform.position = position;

            this.UpdateStat();
        }

        public void UpdateStat()
        {
            this.m_stat.attack = m_baseStat.stat.attack + 2.0f * (m_level - 1.0f);
            this.m_stat.moveSpeed = m_baseStat.stat.moveSpeed;
            this.m_stat.health = m_baseStat.stat.health + 6.0f * (m_level - 1.0f);
            this.m_stat.mana = m_baseStat.stat.mana;

            this.m_battleModule.attack = this.m_stat.attack;

            this.m_skillModule.UpdateCooltimes(Time.deltaTime);
        }

        public void UpdateAnimator()
        {
            // this.m_animatorCtrl.animator.SetFloat("activeSpeed", m_activeSpeed);
        }

        public bool Move(float deltaTime, AxisType axis)
        {
            if(axis != AxisType.None && (axis == AxisType.Negative ^ axis == AxisType.Positive))
            {
                float distance = m_stat.moveSpeed * (float)axis * deltaTime;

                m_tempVector3.x = distance;

                m_transform.Translate(m_tempVector3);

                m_activeSpeed = m_stat.moveSpeed;

                return true;
            }
            else
            {
                m_activeSpeed = 0.0f;

                return false;
            }
        }

        public bool isDied()
        {
            return health <= 0;
        }

        public void SetAutomate(bool isAutomated)
        {
            this.m_isAutomated = isAutomated;
        }

        public void AddLevel(int level)
        {
            if(level < 1) return;

            this.m_level = Unit.s_m_GetLevel(m_level + level, c_MIN_LEVEL, c_MAX_LEVEL);
        }

        public void RemoveLevel(int level)
        {
            if(level < 1) return;

            this.m_level = Unit.s_m_GetLevel(m_level - level, c_MIN_LEVEL, c_MAX_LEVEL);
        }

        public void GetDamage(float damage)
        {
            if(damage < 0) return;

            m_battleModule.GetDamage(damage);
        }

        private static int s_m_GetLevel(int level, int min, int max)
        {
            return level < min ? min : (level > max ? max : level);
        }
    }
}