﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBehavior : MonoBehaviour
{
    private SpriteRenderer _sprite;
    private const string BOUNDARY = "Boundary";
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
        if (string.Equals(collision.gameObject.tag, BOUNDARY, System.StringComparison.OrdinalIgnoreCase))
        {
            _isExtending = false;
            _sprite.color = Color.black;
            gameObject.transform.parent = null;

            GameObject grid = GameObject.Find("GridManager");
            if (grid != null)
            {
                grid.GetComponent<GridController>().OnWallCreate(gameObject);
            }
            //Renderer rend = gameObject.GetComponent<Renderer>();
            //Debug.Log("Bounds: " + rend.bounds);
            //Debug.Log("Center: " + rend.bounds.center);
            //Debug.Log("Extents: " + rend.bounds.extents);
            //Debug.Log("Max: " + rend.bounds.max);
        }            
    }



    public bool IsExtending()
    {
        return _isExtending;
    }
}
