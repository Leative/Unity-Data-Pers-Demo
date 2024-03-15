using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public static MenuController Instance;

    public string PlayerName { get; private set; }
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void SetName(string name)
    {
        PlayerName = name;
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
}
