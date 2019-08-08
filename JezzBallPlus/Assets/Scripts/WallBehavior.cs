using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBehavior : MonoBehaviour
{
    public GameObject sibling;
    private SpriteRenderer _sprite;
    private const string BOUNDARY = "Boundary";
    private const string WALL = "Wall";
    private const string ENEMY = "Enemy";
    private bool _isExtending;
    void Start()
    {
        _isExtending = true;
        _sprite = GetComponent<SpriteRenderer>();
    }


    void Update()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (string.Equals(collision.gameObject.tag, BOUNDARY, System.StringComparison.OrdinalIgnoreCase)
            || (string.Equals(collision.gameObject.tag, WALL, System.StringComparison.OrdinalIgnoreCase) && collision.gameObject.GetComponent<WallBehavior>().IsExtending() == false))
        {
            _isExtending = false;
            _sprite.color = Color.black;
            gameObject.transform.parent = null;

            GameObject grid = GameObject.Find("GridManager");
            if (grid != null)
            {
                if (sibling != null)
                {
                    //only flood-fill if this is a newly generated wall, whose sibling wall was also generated successfully
                    if (sibling.GetComponent<WallBehavior>().IsExtending() == false)
                    {
                        grid.GetComponent<GridController>().OnWallCreate(gameObject);
                        //set both siblings to null to prevent excessive flood-filling
                        sibling.GetComponent<WallBehavior>().sibling = null;
                        sibling = null;
                    }                     
                }
            }
        }
    }



    public bool IsExtending()
    {
        return _isExtending;
    }
}
