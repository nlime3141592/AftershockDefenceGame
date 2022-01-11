using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

using DefenceGameSystem.OS.Kernel;

namespace DefenceGameSystem.OS.API
{
    public class GameManager
    {
        public UnitFactory factoryLeft => m_unitFactoryLeft;
        public UnitFactory factoryRight => m_unitFactoryRight;
        
        // classes
        private UnitManager m_unitManagerLeft, m_unitManagerRight;
        private UnitFactory m_unitFactoryLeft, m_unitFactoryRight;
        private Basecamp m_campLeft, m_campRight;
        private GameManagerBehaviour m_behaviour;

        // internal states
        private bool m_isGameStop = true;
        private Stopwatch m_watch;

        private Unit characterUnit;

        public GameManager(GameManagerBehaviour behaviour)
        {
            m_behaviour = behaviour;

            m_unitManagerLeft = new UnitManager();
            m_unitManagerRight = new UnitManager();

            m_unitFactoryLeft = new UnitFactory();
            m_unitFactoryRight = new UnitFactory();

            m_isGameStop = true;
        }

        public void StartGame()
        {
            m_watch = new Stopwatch();

            m_isGameStop = false;
            m_watch.Start();
        }

        private void EndGame()
        {
            m_watch.Stop();
            m_isGameStop = true;
        }

        public void Awake()
        {

        }

        public void LoadBasecamp(string leftBasecampName, string rightBasecampName)
        {
            Func<string, string> key = (str) =>
            {
                return string.Format("asset.prefab.basecamp.{0}", str).Replace(" ", "").ToLower();
            };

            AsyncAssetLoader<GameObject> loaderLeft = new AsyncAssetLoader<GameObject>();
            AsyncAssetLoader<GameObject> loaderRight = new AsyncAssetLoader<GameObject>();

            loaderLeft.Completed += (result) =>
            {
                m_campLeft = new Basecamp(result, Team.A, Vector3.left * 5.0f + Vector3.up, 1);

                UIMediator.BasecampHealthNow = m_campLeft.health;
                UIMediator.BasecampHealthMax = m_campLeft.maxHealth;

                UIMediator.BasecampManaNow = m_campLeft.manaNow;
                UIMediator.BasecampManaMax = m_campLeft.manaMax;
            };

            loaderRight.Completed += (result) =>
            {
                m_campRight = new Basecamp(result, Team.B, Vector3.right * 35.0f + Vector3.up, 1);

                UIMediator.EnemyBasecampHealthNow = m_campRight.health;
                UIMediator.EnemyBasecampHealthMax = m_campRight.maxHealth;

                m_campRight.SetAutomate(true);
            };

            loaderLeft.LoadAsset(key(leftBasecampName));
            loaderRight.LoadAsset(key(rightBasecampName));
        }

        public void LoadUnit(Team team, string[] unitNames)
        {
            UnitFactory factory = team == Team.A ? m_unitFactoryLeft : m_unitFactoryRight;
            Vector3 prefabPosition = team == Team.A ? Vector3.down * 10.0f : Vector3.right * 30.0f + Vector3.down * 10.0f;

            foreach(string name in unitNames)
            {
                factory.Load(name);
            }
        }

        public void Start()
        {

        }

        public void GenerateCharacter(string characterName)
        {
            Unit gotUnit = GetUnit(characterName, m_campLeft, 1, false);
            characterUnit = gotUnit;
            m_unitManagerLeft.Add(gotUnit);
        }

        public void FixedUpdate()
        {
            if(m_isGameStop) return;

            float deltaTime = Time.fixedDeltaTime;

            m_unitManagerLeft.Move(deltaTime, AxisType.Positive);
            m_unitManagerRight.Move(deltaTime, AxisType.Negative);

            m_campLeft.FixedUpdateMana(Time.fixedDeltaTime);
            m_campRight.FixedUpdateMana(Time.fixedDeltaTime);
        }

        public void Update()
        {
            if(m_isGameStop) return;

            GenerateUnit(m_unitFactoryLeft, m_campLeft, m_unitManagerLeft, KeyType.Button011);
            GenerateUnit(m_unitFactoryRight, m_campRight, m_unitManagerRight, KeyType.Button021);
            m_campRight.GenerateAI(Time.deltaTime, m_unitFactoryRight, m_unitManagerRight);

            m_unitManagerLeft.UseSkill();
            m_unitManagerRight.UseSkill();

            m_unitManagerLeft.UpdateStat();
            m_unitManagerRight.UpdateStat();

            this.UpdateCharacter();

            m_unitManagerLeft.UpdateHealth();
            m_unitManagerRight.UpdateHealth();

            this.UpdateGUI();

            InputModule.Update();
        }

        private void GenerateUnit(UnitFactory factory, Basecamp camp, UnitManager manager, KeyType baseKeyType)
        {
            Func<KeyType, int> index = (type) => (int)type - (int)baseKeyType;
            Action<KeyType> generateAction = (type) =>
            {
                if(InputModule.GetKeyUp(type))
                {
                    try
                    {
                        string unitName = factory[index(type)];
                        float removeMana = factory.CanGetUnit(unitName, camp.manaNow);

                        if(removeMana > 0.0f)
                        {
                            Unit gotUnit = GetUnit(unitName, camp, 1, true);
                            manager.Add(gotUnit);
                            camp.RemoveMana(removeMana);
                        }

                        // else Debug.Log("마나 부족");
                    }
                    catch(IndexOutOfRangeException)
                    {
                        UnityEngine.Debug.Log("존재하지 않는 유닛 이름");
                    }
                }
            };

            generateAction((KeyType)((int)baseKeyType + 0));
            generateAction((KeyType)((int)baseKeyType + 1));
            generateAction((KeyType)((int)baseKeyType + 2));
            generateAction((KeyType)((int)baseKeyType + 3));
            generateAction((KeyType)((int)baseKeyType + 4));
        }

        private Unit GetUnit(string name, Basecamp camp, int level, bool isAutomated = true)
        {
            UnitFactory factory = camp.team == Team.A ? m_unitFactoryLeft : m_unitFactoryRight;

            try
            {
                Unit gotUnit = factory.Get(name, camp, level, isAutomated); // TODO: 이 오브젝트를 Object Pool에 넣어줘서 관리해야 한다.

                // m_unitManagerLeft.Add(gotUnit);

                return gotUnit;
            }
            catch(NullReferenceException)
            {
                UnityEngine.Debug.Log("NULL");
                return default(Unit);
            }
        }

        private void UpdateCharacter()
        {
            if(characterUnit == null) return;

            if(characterUnit.isDied())
            {
                characterUnit = null;
                return;
            }

            characterUnit.SetAutomate(UIMediator.isAutomated);
        }

        private void UpdateGUI()
        {
            UIMediator.BasecampHealthNow = m_campLeft.health;
            UIMediator.BasecampHealthMax = m_campLeft.maxHealth;

            UIMediator.BasecampManaNow = m_campLeft.manaNow;
            UIMediator.BasecampManaMax = m_campLeft.manaMax;

            UIMediator.EnemyBasecampHealthNow = m_campRight.health;
            UIMediator.EnemyBasecampHealthMax = m_campRight.maxHealth;

            if(InputModule.GetKeyUp(KeyType.Button002))
            {
                UIMediator.isAutomated = !UIMediator.isAutomated;
            }
        }

        public void LateUpdate()
        {
            if(m_isGameStop)
            {
                return;
            }
            else
            {
                isGameOver();
            }
        }

        private void isGameOver()
        {
            if (m_campRight.health <= 0)
            {
                // 승리
                EndGame();
                m_behaviour.OnEndGame("승리!");
                return;
            }
            if(m_campLeft.health <= 0)
            {
                // 패배
                EndGame();
                m_behaviour.OnEndGame("패배..");
                return;
            }
        }
    }
}