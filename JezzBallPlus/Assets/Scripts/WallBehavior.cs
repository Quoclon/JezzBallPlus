using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBehavior : MonoBehaviour
{
    private const string BOUNDARY = "Boundary";
    private const string ENEMY = "Enemy";
    private bool _isExtending;
    void Start()
    {
        _isExtending = true;
    }


    void Update()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (string.Equals(collision.gameObject.tag, BOUNDARY, System.StringComparison.OrdinalIgnoreCase))
        {
            _isExtending = false;
            gameObject.transform.parent = null;
        }            
    }

    public bool IsExtending()
    {
        return _isExtending;
    }
}
