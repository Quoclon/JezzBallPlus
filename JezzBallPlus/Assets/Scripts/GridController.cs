using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private ContactFilter2D contactFilter;

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

        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(8));
        contactFilter.useLayerMask = true;
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
        List<GameObject> newTiles = new List<GameObject>();

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
                    newTiles.Add(block);
                }
            }
        }

        //get direction of wall
        //if z-rotation is 0 or 180 it is a vertical wall
        //if z-rotation is 90 or 270 it is a horizontal wall
        if (wall.transform.rotation.z == 0 || wall.transform.rotation.z == 180)
        {
            //vertical wall: find tiles with far-right x / middle y and far - left x / middle y
            float minX = newTiles.OrderBy(t => t.transform.position.x).First().transform.position.x;
            float maxX = newTiles.OrderByDescending(t => t.transform.position.x).First().transform.position.x;
            float y = GetMedianYCoordinate(newTiles);
            GameObject minTile = newTiles.Where(x => x.transform.position.x == minX).Where(z => z.transform.position.y == y).First();
            GameObject maxTile = newTiles.Where(x => x.transform.position.x == maxX).Where(z => z.transform.position.y == y).First();
            //raycast to the left of minTile
            RaycastHit2D minRay = Physics2D.Raycast(minTile.transform.position, Vector2.left, _offsetX, 1 << LayerMask.NameToLayer("Grid"));
            //Vector2 forward = transform.TransformDirection(Vector2.left) * 0.1f;
            //Debug.DrawRay(minTile.transform.position, forward, Color.red, 100);
            //raycast to the right of maxTile
            RaycastHit2D maxRay = Physics2D.Raycast(maxTile.transform.position, Vector2.right, _offsetX, 1 << LayerMask.NameToLayer("Grid"));

            if (minRay.collider != null)
            {
                if (minRay.collider.gameObject != null)
                    FloodFill(minRay.collider.gameObject);
            }
            if (maxRay.collider != null)
            {
                if (maxRay.collider.gameObject != null)
                    FloodFill(maxRay.collider.gameObject);
            }
        }
        else
        {
            //horizontal wall: find tiles with middle x/top y and middle x/top y
            float x = GetMedianXCoordinate(newTiles);
            float minY = newTiles.OrderBy(t => t.transform.position.y).First().transform.position.y;
            float maxY = newTiles.OrderByDescending(t => t.transform.position.y).First().transform.position.y;
            GameObject minTile = newTiles.Where(t => t.transform.position.x == x).Where(z => z.transform.position.y == minY).First();
            GameObject maxTile = newTiles.Where(t => t.transform.position.x == x).Where(z => z.transform.position.y == maxY).First();
            //raycast to the bottom of minTile
            RaycastHit2D minRay = Physics2D.Raycast(minTile.transform.position, Vector2.down, _offsetX, 1 << LayerMask.NameToLayer("Grid"));
            //raycast to the top of maxTile
            RaycastHit2D maxRay = Physics2D.Raycast(maxTile.transform.position, Vector2.up, _offsetX, 1 << LayerMask.NameToLayer("Grid"));


            if (minRay.collider != null)
            {
                if (minRay.collider.gameObject != null)
                    FloodFill(minRay.collider.gameObject);
            }
            if (maxRay.collider != null)
            {
                if (maxRay.collider.gameObject != null)
                    FloodFill(maxRay.collider.gameObject);
            }
        }


        ResetTileCounts();
    }

    private void FloodFill(GameObject tile)
    {
        Debug.Log("Flood filling for: " + tile.transform.position);
    }

    private void ResetTileCounts()
    {
        _startingColorCount = 0;
        _targetColorCount = 0;
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
        foreach (GameObject tile in tiles)
        {
            if (!tile.name.Contains("Black"))
                _startingColorCount++;
            else if (tile.name.Contains("Black"))
                _targetColorCount++;
        }
        Debug.Log("Black Tile Count: " + _targetColorCount);
        Debug.Log("Cyan Tile Count: " + _startingColorCount);
    }

    private float GetMedianXCoordinate(List<GameObject> objects)
    {
        float median = 0;
        int cnt = 0;
        List<float> numbers = new List<float>();
        foreach (GameObject obj in objects)
        {
            numbers.Add(obj.transform.position.x);
            cnt++;
        }
        numbers.Sort();
        int halfIndex = numbers.Count / 2;
        median = numbers[halfIndex];
        return median;
    }

    private float GetMedianYCoordinate(List<GameObject> objects)
    {
        float median = 0;
        int cnt = 0;
        List<float> numbers = new List<float>();
        foreach (GameObject obj in objects)
        {
            numbers.Add(obj.transform.position.y);
            cnt++;
        }
        numbers.Sort();
        int halfIndex = numbers.Count / 2;
        median = numbers[halfIndex];
        return median;
    }




}
