using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DefenceGameSystem.OS.API.GUI
{
    public class ResultCanvas : MonoBehaviour
    {
        public Text resultText;
        public Button endButton;

        private void Awake()
        {
            endButton.onClick.AddListener(
                () =>
                {
                    Debug.Log("앱 종료");
                    Application.Quit();
                }
            );
        }

        public void ShowResult(string result)
        {
            resultText.text = result;
        }
    }
}