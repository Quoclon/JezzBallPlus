using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    [SerializeField]
    private GameObject _cyanBlock;
    [SerializeField]
    private GameObject _blackBlock;

    private int _startingColorCount;
    private int _targetColorCount;
    private int _totalCount;

    private int[,] startingArray = new int[180, 80];

    private float _startingX = -8.94998f;
    private float _startingY = 3.95f;
    private float _offsetX = 0.1f;
    private float _offsetY = 0.1f;

    void Start()
    {
        for (int i = 0; i < startingArray.GetLength(0); i++)
        {
            for (int j = 0; j < startingArray.GetLength(1); j++)
            {
                GameObject block = Instantiate(_cyanBlock) as GameObject;
                float xPos = _startingX + (_offsetX * i);
                float yPos = _startingY - (_offsetY * j);
                block.transform.position = new Vector3(xPos, yPos, 0);
                block.transform.SetParent(transform, true);
                block.layer = 8;
            }
        }
        ResetTileCounts();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnWallCreate(GameObject wall)
    {
        Renderer rend = wall.GetComponent<Renderer>();
        BoxCollider2D coll = wall.GetComponent<BoxCollider2D>();
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");

        foreach (GameObject tile in tiles)
        {
            //if tile is covered by the newly created wall
            if (coll.bounds.Contains(tile.transform.position))
            {
                //replace non-black tiles with black tiles
                if (!tile.name.Contains("Black"))
                {
                    Vector3 pos = tile.transform.position;
                    Destroy(tile);
                    GameObject block = Instantiate(_blackBlock) as GameObject;
                    block.transform.position = pos;
                    block.transform.SetParent(transform, true);
                    block.layer = 8;
                }
            }
        }
        ResetTileCounts();
    }

    private void ResetTileCounts()
    {
        _startingColorCount = 0;
        _targetColorCount = 0;
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
        foreach(GameObject tile in tiles)
        {
            if (!tile.name.Contains("Black"))
                _startingColorCount++;
            else if (tile.name.Contains("Black"))
                _targetColorCount++;
        }
        Debug.Log("Black Tile Count: " + _targetColorCount);
        Debug.Log("Cyan Tile Count: " + _startingColorCount);
    }


}
