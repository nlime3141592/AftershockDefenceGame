using UnityEngine;
using UnityEngine.UI;

namespace DefenceGameSystem.OS.API.GUI
{
    public class AutomatorController : MonoBehaviour
    {
        public Sprite On;
        public Sprite Off;
        private Image m_image;

        private void Awake()
        {
            m_image = GetComponent<Image>();
        }

        private void Update()
        {
            m_image.sprite = UIMediator.isAutomated ? On : Off;
        }
    }
}