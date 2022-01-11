using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using DefenceGameSystem.OS.API;
using DefenceGameSystem.OS.Kernel;
using DefenceGameSystem.OS.API.GUI;

// NOTE:
// GameManager 클래스의 MonoBehaviour 구현체 클래스로 사용하는 클래스
// 테스트 클래스이다.
public class GameManagerBehaviour : MonoBehaviour
{
    // classes
    public GameManager Manager => m_gameManager;
    private GameManager m_gameManager;

    // internal states
    // public string unitName;
    // public bool checker;

    public UIViewModel viewModel;
    public GameObject resCanvasObj;
    public GameObject mainCamera;
    public Transform ResultCanvasParent;

    // 프리팹 이름을 editor에서 써 넣어줘야 함.
    public List<string> units_A;
    public List<string> units_B;
    public string characterUnitName;
    public string basecamp_A;
    public string basecamp_B;

    void Awake()
    {
        m_gameManager = new GameManager(this);

        m_gameManager.LoadBasecamp(basecamp_A, basecamp_B);
        m_gameManager.LoadUnit(Team.A, units_A.ToArray());
        m_gameManager.LoadUnit(Team.B, units_B.ToArray());

        m_gameManager.LoadUnit(Team.A, new string[] { characterUnitName });
    }

    void Start()
    {
        foreach(string name in units_A)
            viewModel.unitNames.Add(name);

        StartCoroutine(starter());
    }

    IEnumerator starter()
    {
        yield return new WaitForSeconds(1.0f);

        m_gameManager.StartGame();
        m_gameManager.GenerateCharacter(characterUnitName);
        viewModel.OnGameStart(m_gameManager);
    }

    void FixedUpdate()
    {
        m_gameManager.FixedUpdate();
    }

    void Update()
    {
        m_gameManager.Update();
    }

    void LateUpdate()
    {
        m_gameManager.LateUpdate();
    }

    public void OnEndGame(string message)
    {
        mainCamera.SetActive(false);

        GameObject canvas = GameObject.Instantiate(resCanvasObj);
        canvas.transform.SetParent(ResultCanvasParent, false);

        ResultCanvas canvClass = resCanvasObj.GetComponent<ResultCanvas>();

        canvClass.ShowResult(message);
    }
}