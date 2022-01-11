using UnityEngine;
using UnityEngine.UI;

namespace DefenceGameSystem.OS.API.GUI
{
    public class Gauge : MonoBehaviour
    {
        public float min;
        public float max;
        public float now;

        public Text text;
        public Image fillArea;

        private void Update()
        {
            now = (int)(Mathf.Clamp(now, min, max));

            fillArea.fillAmount = Mathf.Clamp01((now - min) / (max - min));

            text.text = string.Format("{0}/{1}", (int)now, (int)max);
        }
    }
}