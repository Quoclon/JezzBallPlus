using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private int _level;
    [SerializeField]
    private Text _levelValue;

    private float _percent;
    [SerializeField]
    private Text _percentValue;

    private int _lives;
    [SerializeField]
    private Text _livesValue;

    [SerializeField]
    private GameObject _enemyPrefab;

    private float minX = -8;
    private float maxX = 8;
    private float maxY = 3.5f;
    private float minY = -3.5f;

    private void Awake()
    {
        Messenger<float>.AddListener(GameEvent.GRID_UPDATE, OnGridUpdate);
        Messenger.AddListener(GameEvent.LIVE_LOST, OnLiveLost);
    }

    private void OnDestroy()
    {
        Messenger<float>.RemoveListener(GameEvent.GRID_UPDATE, OnGridUpdate);
        Messenger.RemoveListener(GameEvent.LIVE_LOST, OnLiveLost);
    }

    // Start is called before the first frame update
    void Start()
    {
        _level = 1;
        _levelValue.text = _level.ToString();
        _percent = 0;
        _percentValue.text = _percent.ToString();
        _lives = 5;
        _livesValue.text = _lives.ToString();

        GenerateCircles();
    }
    void OnGridUpdate(float value)
    {
        _percent = value;
        _percentValue.text = _percent.ToString();

        if (value > 90)
        {
            _level++;
            _levelValue.text = _level.ToString();

            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject obj in enemies)
                Destroy(obj);
            GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
            foreach (GameObject obj in tiles)
                Destroy(obj);
            GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
            foreach (GameObject obj in walls)
                Destroy(obj);

            _percent = 0;
            _percentValue.text = _percent.ToString();

            GenerateCircles();

            Messenger.Broadcast(GameEvent.ADVANCE_LEVEL);
        }
    }

    void OnLiveLost()
    {
        _lives--;
        _livesValue.text = _lives.ToString();
        if (_lives == 0)
        {
            SceneManager.LoadScene("SampleScene");
        }
    }

    void GenerateCircles()
    {
        for (int i = 0; i < _level; i++)
        {
            GameObject newEnemy = Instantiate(_enemyPrefab) as GameObject;
            float xPos = Random.Range(minX, maxX);
            float yPos = Random.Range(minY, maxY);
            newEnemy.transform.position = new Vector3(xPos, yPos, 1);
        }
    }


}
