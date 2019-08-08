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
    private const float FLOOD_FILL_OFFSET = 0.01f;
    private const float RAYCAST_DISTANCE = 0.5f;
    private const string ON_WALL_CREATE_AUDIT = "OnWallCreate: Wall Position: {0}, Boundary Min X: {1}, Boundary Max X: {2}, Y: {3}";
    private const string FLOOD_FILL_AUDIT = "Flood Fill: Starting Point {0}, MinX: {1}, MaxX: {2}, MinY: {3}, MaxY: {4}, New Wall Position: {5}";    

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
        Debug.Log("ROTATION IS: " + wall.transform.rotation.z);

        //get direction of wall
        //if z-rotation is 0 or 180 it is a vertical wall
        //if z-rotation is 90 or 270 it is a horizontal wall
        //if (wall.transform.rotation.z == 0 || wall.transform.rotation.z == 180)
        if (Mathf.Approximately(wall.transform.rotation.z, 0) || Mathf.Approximately(wall.transform.rotation.z, -1) || (Mathf.Abs(wall.transform.rotation.z) < 0.1))
        {
            //vertical wall: find far-right x / middle y and far - left x / middle y
            float maxX = box.bounds.max.x;
            float minX = box.bounds.min.x;
            float y = wall.transform.position.y;
            Vector3 leftPoint = new Vector3(minX - FLOOD_FILL_OFFSET, y, 0);
            Vector3 rightPoint = new Vector3(maxX + FLOOD_FILL_OFFSET, y, 0);
            FloodFill(leftPoint);
            FloodFill(rightPoint);
            Debug.Log(string.Format(ON_WALL_CREATE_AUDIT, wall.transform.position.ToString(), minX, maxX, y));
        }
        else
        {
            //horizontal wall: find middle x/top y and middle x/top y
            float minY = box.bounds.min.y;
            float maxY = box.bounds.max.y;
            float x = wall.transform.position.x;
            Vector3 downPoint = new Vector3(x, minY - FLOOD_FILL_OFFSET, 0);
            Vector3 upPoint = new Vector3(x, maxY + FLOOD_FILL_OFFSET, 0);
            FloodFill(downPoint);
            FloodFill(upPoint);
        }
    }


    private void FloodFill(Vector3 point)
    {
        List<Vector3> checkedPoints = new List<Vector3>();
        Queue<Vector3> pointsToCheck = new Queue<Vector3>();
        pointsToCheck.Enqueue(point);
        Vector3 startingPoint = point;
        int executionNumber = 0;
        while (pointsToCheck.Count > 0)
        {
            executionNumber++;
            if (executionNumber > 15000)
            {
                Debug.Log("Something has gone horribly wrong");
                return;
            }                

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
                    else if (left.collider.gameObject.name.Contains("Square") || left.collider.gameObject.name.Contains("Wall") || left.collider.gameObject.name.Contains("Black"))
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
                    else if (right.collider.gameObject.name.Contains("Square") || right.collider.gameObject.name.Contains("Wall") || right.collider.gameObject.name.Contains("Black"))
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
                    else if (up.collider.gameObject.name.Contains("Square") || up.collider.gameObject.name.Contains("Wall") || up.collider.gameObject.name.Contains("Black"))
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
                    else if (down.collider.gameObject.name.Contains("Square") || down.collider.gameObject.name.Contains("Wall") || down.collider.gameObject.name.Contains("Black"))
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

        float xLength = maxX - minX;
        float yLength = maxY - minY;
        float centerX = (xLength / 2) + minX;
        float centerY = (yLength / 2) + minY;

        GameObject newWall = Instantiate(_blackBlock) as GameObject;
        newWall.transform.position = new Vector3(centerX, centerY, 0);
        float xScale = xLength / _squareSize;
        float yScale = yLength / _squareSize;
        newWall.transform.localScale = new Vector3(xScale, yScale, 1);
        
        Debug.Log(string.Format(FLOOD_FILL_AUDIT, startingPoint.ToString(), minX, maxX, minY, maxY, newWall.transform.position.ToString()));
    }
}
