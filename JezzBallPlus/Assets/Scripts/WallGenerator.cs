using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject _wallPrefab;
    private GameObject _wallOne;
    private GameObject _wallTwo;

    [SerializeField]
    private float _scaleSpeed = 0.05f;    

    void Start()
    {
        _wallOne = Instantiate(_wallPrefab) as GameObject;
        _wallOne.GetComponent<SpriteRenderer>().color = Color.red;
        _wallOne.transform.position = transform.position;
        BoxCollider2D box = _wallOne.GetComponent<BoxCollider2D>();
        float width = box.bounds.size.x;
        float height = box.bounds.size.y;
        _wallOne.transform.position = new Vector3(transform.position.x + (width / 2), transform.position.y + (height / 2), 0);
        _wallOne.transform.SetParent(transform, true);

        _wallTwo = Instantiate(_wallPrefab) as GameObject;
        _wallTwo.GetComponent<SpriteRenderer>().color = Color.blue;
        _wallTwo.transform.position = transform.position;
        _wallTwo.transform.position = new Vector3(transform.position.x + (width / 2), transform.position.y - (height / 2), 0);
        _wallTwo.transform.SetParent(transform, true);
    }


    void Update()
    {
        Vector3 localScale = transform.localScale;
        float xScale = localScale.x;
        float yScale = localScale.y;
        float zScale = localScale.z;
        yScale += _scaleSpeed;
        transform.localScale = new Vector3(xScale, yScale, zScale);
    }

}
