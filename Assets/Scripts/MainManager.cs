using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public Text ScoreText2;
    public GameObject GameOverText;

    private bool m_Started = false;
    private int m_Points;

    private bool m_GameOver = false;
    private Highscore m_Highscore;

    private string HIGHSCORE_STORAGE_PATH;


    // Start is called before the first frame update
    void Start()
    {
        HIGHSCORE_STORAGE_PATH = Application.persistentDataPath + Path.DirectorySeparatorChar + "highscore.json";
        Debug.Log("New");
        LoadHighscore();
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);

        int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 };
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);

        if (m_Highscore.score < m_Points)
        {
            UpdateHighscore(m_Points, MenuController.Instance.PlayerName);
        }
    }

    private void UpdateHighscore(int points, string playerName)
    {
        m_Highscore = new Highscore(points, playerName);
        try
        {
            Debug.Log(HIGHSCORE_STORAGE_PATH);
            string jsonHighscore = JsonUtility.ToJson(m_Highscore);
            File.WriteAllText(HIGHSCORE_STORAGE_PATH, jsonHighscore);
        }
        catch
        {
            Debug.Log("Couldn't store highscore file.");
        }
        ShowHighscore();
    }

    private void LoadHighscore()
    {
        Highscore highscore = new Highscore(0, "---");
        if (File.Exists(HIGHSCORE_STORAGE_PATH))
        {
            Debug.Log("Found file");
            try
            {
                string jsonHighscore = File.ReadAllText(HIGHSCORE_STORAGE_PATH);
                highscore = JsonUtility.FromJson<Highscore>(jsonHighscore);
            }
            catch
            {
                Debug.Log("Couldn't load highscore file.");
            }
        }
        m_Highscore = highscore;
        ShowHighscore();
    }

    private void ShowHighscore()
    {
        ScoreText2.text = "Highscore: " + m_Highscore.score + " Name: " + m_Highscore.name;
    }

    private class Highscore
    {

        public int score;
        public string name;

        public Highscore(int score, string name)
        {
            this.score = score;
            this.name = name;
        }
    }
}
