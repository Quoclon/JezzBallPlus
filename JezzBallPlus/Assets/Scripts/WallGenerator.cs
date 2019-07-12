using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject _wallPrefab;

    private GameObject _WallOne;

    void Start()
    {
        _WallOne = Instantiate(_wallPrefab) as GameObject;

        _WallOne.transform.position = transform.position;
        RectTransform rt = _WallOne.GetComponent<RectTransform>();
        BoxCollider2D box = _WallOne.GetComponent<BoxCollider2D>();
        //float width = rt.rect.width;
        //float height = rt.rect.height;
        float width = box.bounds.size.x;
        float height = box.bounds.size.y;

        float newX = transform.position.x + (width / 2);
        float newY = transform.position.y + (height / 2);
        _WallOne.transform.position = new Vector3(newX, newY, 0);

        _WallOne.transform.SetParent(transform, true);

    }


    void Update()
    {
        Vector3 localScale = transform.localScale;
        float xScale = localScale.x;
        float yScale = localScale.y;
        float zScale = localScale.z;
        yScale += 0.01f;
        transform.localScale = new Vector3(xScale, yScale, zScale);
    }
}
