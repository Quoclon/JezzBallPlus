using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //GameObject components
    private SpriteRenderer _sprite;

    //sprites
    [SerializeField]
    private Sprite _playerSprite;

    //region private variables
    [SerializeField]
    private float _scrollSensitivity = 1f;
    private float _clickRotation = 45f;
    private RotationType _rotationType;

    void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _rotationType = RotationType.Click;
    }


    void Update()
    {
        //move character
        Vector3 mousePos = Input.mousePosition;
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(mousePos);
        transform.position = new Vector3(worldPoint.x, worldPoint.y, transform.position.z);

        //_rotationType Scroll
        if (_rotationType == RotationType.Scroll)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            Vector3 zRotation = new Vector3(0, 0, scroll * _scrollSensitivity);
            transform.Rotate(zRotation * Time.deltaTime, Space.Self);
        }

        //_rotationType Click
        if (_rotationType == RotationType.Click)
        {
            if (Input.GetMouseButtonDown(1))
            {
                Vector3 currentEul = transform.eulerAngles;
                currentEul.z += _clickRotation;
                transform.eulerAngles = currentEul;
            }
        }
    }
}
