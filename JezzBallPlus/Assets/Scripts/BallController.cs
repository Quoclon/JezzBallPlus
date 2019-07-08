using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    //GameObject components
    private SpriteRenderer _sprite;
    private Rigidbody2D _body;

    //sprites
    [SerializeField]
    private Sprite _ballSprite;

    //private variables
    [SerializeField]
    private float speed = 10f;
    private int[] directionArray = new int[] { -1, 1 }; //todo: add direction '0'?
    private float collisionBoundary = 3.8f;

    void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _body = GetComponent<Rigidbody2D>();

        int randX = Random.Range(0, directionArray.Length);
        int randY = Random.Range(0, directionArray.Length);
        float vecX = directionArray[randX] * speed * Time.deltaTime;
        float vecY = directionArray[randY] * speed * Time.deltaTime;
        _body.velocity = new Vector2(vecX, vecY);        
    }

    void Update()
    {
        
    }

    //top-right collision point: 3.7, 4.0
    //bottom-right collision point: 3.7, -4.0
    //top-left: -3.7, 4.0
    //bottom-left:
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("First point that collided: " + collision.contacts[0].point);
        Vector2 collisionPoint = collision.contacts[0].point;
        float xVel = _body.velocity.x;
        float yVel = _body.velocity.y;
        if (Mathf.Abs(collisionPoint.x) >= collisionBoundary)
            xVel *= -1f;
        if (Mathf.Abs(collisionPoint.y) >= collisionBoundary)
            yVel *= -1f;
        _body.velocity = new Vector2(xVel, yVel);         
    }

}
