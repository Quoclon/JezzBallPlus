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

    [SerializeField]
    private GameObject _wallGeneratorPrefab;
    private GameObject _currentWallGenerator;

    //region private variables
    [SerializeField]
    private float _scrollSensitivity = 1f;
    private float _clickFourRotation = 45f;
    private float _clickTwoRotation = 90f;
    private bool _isGenerating = false;
    private RotationType _rotationType;

    void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _rotationType = RotationType.ClickTwo;
    }

    void Update()
    {
        //move character
        Vector3 mousePos = Input.mousePosition;
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(mousePos);
        transform.position = new Vector3(worldPoint.x, worldPoint.y, transform.position.z);

        //create WallGenerator
        if (Input.GetMouseButtonDown(0))
        {
            if (_currentWallGenerator == null)
            {
                _currentWallGenerator = Instantiate(_wallGeneratorPrefab) as GameObject;
                _currentWallGenerator.transform.position = gameObject.transform.position; //todo: looks like the wall generates slightly to the right of the cursor?
                _currentWallGenerator.GetComponent<WallGenerator>().CursorRotation = transform.eulerAngles;
            }
        }

        //_rotationType Scroll
        if (_rotationType == RotationType.Scroll)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            Vector3 zRotation = new Vector3(0, 0, scroll * _scrollSensitivity);
            transform.Rotate(zRotation * Time.deltaTime, Space.Self);
        }

        //_rotationType ClickFour
        if (_rotationType == RotationType.ClickFour)
        {
            if (Input.GetMouseButtonDown(1))
            {
                Vector3 currentEul = transform.eulerAngles;
                currentEul.z += _clickFourRotation;
                transform.eulerAngles = currentEul;
            }
        }

        //_rotationType ClickTwo
        if (_rotationType == RotationType.ClickTwo)
        {
            if (Input.GetMouseButtonDown(1))
            {
                Vector3 currentEul = transform.eulerAngles;
                currentEul.z += _clickTwoRotation;
                transform.eulerAngles = currentEul;
            }
        }
    }


}
