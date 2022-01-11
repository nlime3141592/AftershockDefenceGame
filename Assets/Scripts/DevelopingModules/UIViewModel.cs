using System.Collections.Generic;

using UnityEngine;

using DefenceGameSystem.OS.Kernel;

namespace DefenceGameSystem.OS.API.GUI
{
    public class UIViewModel : MonoBehaviour
    {
        // UI classes
        public Gauge healthGauge;
        public Gauge manaGauge;
        public Gauge enemyHealthGauge;

        // prefab
        public GameObject deckCardPrefab;
        public Transform deckTransform;
        public List<string> unitNames;
        public Dictionary<string, float> deck;

        private void Awake()
        {
            unitNames = new List<string>();
            deck = new Dictionary<string, float>();
        }

        private void Start()
        {
            
        }

        public void OnGameStart(GameManager gameManager)
        {
            int i = 0;

            foreach(string name in unitNames)
            {
                GameObject cardObject = GameObject.Instantiate(deckCardPrefab);
                cardObject.transform.SetParent(deckTransform, false);

                Card card = cardObject.GetComponent<Card>();
                card.Bind((KeyType)((int)KeyType.Button011 + i++));

                deck.Add(name, gameManager.factoryLeft.GetUnitCost(name));

                card.nameText.text = name;
                card.manaText.text = string.Format("{0}", deck[name]);
            }
        }

        private void Update()
        {
            m_UpdateValues();
        }

        private void m_UpdateValues()
        {
            m_UpdateGauge(healthGauge, UIMediator.BasecampHealthNow, 0, UIMediator.BasecampHealthMax);
            m_UpdateGauge(manaGauge, UIMediator.BasecampManaNow, 0, UIMediator.BasecampManaMax);
            m_UpdateGauge(enemyHealthGauge, UIMediator.EnemyBasecampHealthNow, 0, UIMediator.EnemyBasecampHealthMax);
        }

        private void m_UpdateGauge(Gauge gauge, float now, float min, float max)
        {
            gauge.min = min;
            gauge.max = max;
            gauge.now = now;
        }
    }
}