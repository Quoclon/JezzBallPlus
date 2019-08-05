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
    [SerializeField]
    private GameObject _horizontalWall;
    [SerializeField]
    private GameObject _verticalWall;

    private float _arenaArea;
    private float _filledArea;
    private float _filledAreaPercent;

    private float _squareSize = 0.1f;
    private const float RAYCAST_DISTANCE = 0.5f;    

    void Start()
    {
        BoxCollider2D horizontalBox = _horizontalWall.GetComponent<BoxCollider2D>();
        BoxCollider2D verticalBox = _verticalWall.GetComponent<BoxCollider2D>();
        _arenaArea = horizontalBox.bounds.size.x * verticalBox.bounds.size.y;
    }
    // Update is called once per frame
    void Update()
    {

    }

    public void OnWallCreate(GameObject wall)
    {
        BoxCollider2D box = wall.GetComponent<BoxCollider2D>();
        float area = box.bounds.size.x * box.bounds.size.y;
        _filledArea += area;
        _filledAreaPercent = (_filledArea / _arenaArea) * 100f;
        Debug.Log("Percent: " + _filledAreaPercent);

        //get direction of wall
        //if z-rotation is 0 or 180 it is a vertical wall
        //if z-rotation is 90 or 270 it is a horizontal wall
        if (wall.transform.rotation.z == 0 || wall.transform.rotation.z == 180)
        {
            //vertical wall: find far-right x / middle y and far - left x / middle y
            float maxX = box.bounds.max.x;
            float minX = box.bounds.min.x;
            float y = wall.transform.position.y;
            Vector3 leftPoint = new Vector3(minX - RAYCAST_DISTANCE, y, 0);
            Vector3 rightPoint = new Vector3(maxX + RAYCAST_DISTANCE, y, 0);
            //Debug.Log("left:" + leftPoint);
            //Debug.Log("right:" + rightPoint);
            //FloodFill(leftPoint);
            FloodFill(rightPoint);
        }
        else
        {
            //horizontal wall: find middle x/top y and middle x/top y
            float minY = box.bounds.min.y;
            float maxY = box.bounds.max.y;
            float x = wall.transform.position.x;
            Vector3 downPoint = new Vector3(x, minY - RAYCAST_DISTANCE, 0);
            Vector3 upPoint = new Vector3(x, maxY + RAYCAST_DISTANCE, 0);
            //Debug.Log("down:" + downPoint);
            //Debug.Log("up:" + upPoint);
        }
    }


    private void FloodFill(Vector3 point)
    {
        List<Vector3> checkedPoints = new List<Vector3>();
        Queue<Vector3> pointsToCheck = new Queue<Vector3>();
        pointsToCheck.Enqueue(point);

       // int executionNumber = 0;
        while (pointsToCheck.Count > 0)
        {
            //executionNumber++;
            //if (executionNumber > 15000)
            //    Debug.Log("Something is wrong");

            Vector3 _point = pointsToCheck.Dequeue();
            if (checkedPoints.Contains(_point))
                continue;
            checkedPoints.Add(_point);

            RaycastHit2D left = Physics2D.Raycast(_point, Vector2.left, RAYCAST_DISTANCE, 1 << LayerMask.NameToLayer("Default"));
            RaycastHit2D right = Physics2D.Raycast(_point, Vector2.right, RAYCAST_DISTANCE, 1 << LayerMask.NameToLayer("Default"));
            RaycastHit2D up = Physics2D.Raycast(_point, Vector2.up, RAYCAST_DISTANCE, 1 << LayerMask.NameToLayer("Default"));
            RaycastHit2D down = Physics2D.Raycast(_point, Vector2.down, RAYCAST_DISTANCE, 1 << LayerMask.NameToLayer("Default"));

            if (left.collider != null)
            {
                if (left.collider.gameObject != null)
                {
                    if (left.collider.gameObject.name.Contains("Circle"))
                        return;
                    else if (left.collider.gameObject.name.Contains("Boundary") || left.collider.gameObject.name.Contains("Wall"))
                        checkedPoints.Add(left.point);
                }
            }
            else
            {
                Vector3 newPoint = new Vector3(_point.x - RAYCAST_DISTANCE, _point.y, _point.z);
                if (!checkedPoints.Contains(newPoint))
                    pointsToCheck.Enqueue(newPoint);
            }

            if (right.collider != null)
            {
                if (right.collider.gameObject != null)
                {
                    if (right.collider.gameObject.name.Contains("Circle"))
                        return;
                    else if (right.collider.gameObject.name.Contains("Boundary") || right.collider.gameObject.name.Contains("Wall"))
                        checkedPoints.Add(right.point);
                }
            }
            else
            {
                Vector3 newPoint = new Vector3(_point.x + RAYCAST_DISTANCE, _point.y, _point.z);
                if (!checkedPoints.Contains(newPoint))
                    pointsToCheck.Enqueue(newPoint);
            }

            if (up.collider != null)
            {
                if (up.collider.gameObject != null)
                {
                    if (up.collider.gameObject.name.Contains("Circle"))
                        return;
                    else if (up.collider.gameObject.name.Contains("Boundary") || up.collider.gameObject.name.Contains("Wall"))
                        checkedPoints.Add(up.point);
                }
            }
            else
            {
                Vector3 newPoint = new Vector3(_point.x, _point.y + RAYCAST_DISTANCE, _point.z);
                if (!checkedPoints.Contains(newPoint))
                    pointsToCheck.Enqueue(newPoint);
            }

            if (down.collider != null)
            {
                if (down.collider.gameObject != null)
                {
                    if (down.collider.gameObject.name.Contains("Circle"))
                        return;
                    else if (down.collider.gameObject.name.Contains("Boundary") || down.collider.gameObject.name.Contains("Wall"))
                        checkedPoints.Add(down.point);
                }
            }
            else
            {
                Vector3 newPoint = new Vector3(_point.x, _point.y - RAYCAST_DISTANCE, _point.z);
                if (!checkedPoints.Contains(newPoint))
                    pointsToCheck.Enqueue(newPoint);
            }
        }

        float minX = checkedPoints.OrderBy(p => p.x).First().x;
        float maxX = checkedPoints.OrderByDescending(p => p.x).First().x;
        float minY = checkedPoints.OrderBy(p => p.y).First().y;
        float maxY = checkedPoints.OrderByDescending(p => p.y).First().y;
        Debug.Log("minX: " + minX);
        Debug.Log("maxX: " + maxX);
        Debug.Log("minY: " + minY);
        Debug.Log("maxY: " + maxY);
    }
}
