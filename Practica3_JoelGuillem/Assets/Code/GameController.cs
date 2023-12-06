using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{

    static GameController m_GameController = null;
    public GameObject m_DestroyObjects;
    List<IRestartGameElement> m_RestartGameElements = new List<IRestartGameElement>();
    static bool m_AlreadyInitialized = false;

    //List<EnemyGoomba> m_EnemyGoombas;

    static public GameController GetGameController()
    {
        if(m_GameController == null && !m_AlreadyInitialized)
        {
            GameObject l_GameObject = new GameObject("GameController");
            m_GameController = l_GameObject.AddComponent<GameController>();
            m_GameController.m_DestroyObjects = new GameObject("DestroyObjects");
            m_GameController.m_DestroyObjects.transform.SetParent(l_GameObject.transform);
            //m_GameController.m_EnemyGoombas = new List<EnemyGoomba>();
            GameController.DontDestroyOnLoad(l_GameObject);
            m_AlreadyInitialized = true;
        }
        return m_GameController;
    }
    public void AddRestartGameElement(IRestartGameElement _RestartGameElement)
    {
        m_RestartGameElements.Add(_RestartGameElement);
    }

    public void RestartGame()
    {
        DestroyGameObjects();
        foreach (IRestartGameElement _RestartGameElement in m_RestartGameElements)
            _RestartGameElement.RestartGame();
    }

    void DestroyGameObjects()
    {
        Transform[] l_Transforms = m_DestroyObjects.GetComponentsInChildren<Transform>();
        foreach (Transform l_Transform in l_Transforms)
        {
            if (l_Transform != m_DestroyObjects.transform)
            {
                GameObject.Destroy(l_Transform.gameObject);
            }
            
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }


    public void GoToLevel1()
    {
        DestroyGameObjects();
        SceneManager.LoadSceneAsync("Level1Scene");
    }
    public void GoToLevel2()
    {
        DestroyGameObjects();
        SceneManager.LoadSceneAsync("Level2Scene");
    }
    public void GoToMenu()
    {
        DestroyGameObjects();
        //GameObject.Destroy(m_Player.gameObject);
        SceneManager.LoadSceneAsync("MainMenuScene");
    }

}
