using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    //GameObject components
    private SpriteRenderer _sprite;
    private Rigidbody2D _body;
    private CircleCollider2D _circle;
    private AudioSource _audio;

    //sprites
    [SerializeField]
    private Sprite _ballSprite;

    //sounds
    [SerializeField]
    private AudioClip _ping;
    [SerializeField]
    private AudioClip _zing;
    [SerializeField]
    private AudioClip _crash;

    //private variables
    [SerializeField]
    private float speed = 10f;
    private int[] directionArray = new int[] { -1, 1 }; //todo: add direction '0'?

    private ContactFilter2D contactFilter;
    private RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
    private const float rayCastDistance = 0.1f;
    private float collisionBoundary = 0.2f;

    void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _body = GetComponent<Rigidbody2D>();
        _circle = GetComponent<CircleCollider2D>();
        _audio = GetComponent<AudioSource>();

        int randX = Random.Range(0, directionArray.Length);
        int randY = Random.Range(0, directionArray.Length);
        float vecX = directionArray[randX] * speed * Time.deltaTime;
        float vecY = directionArray[randY] * speed * Time.deltaTime;
        _body.velocity = new Vector2(vecX, vecY);

        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilter.useLayerMask = true;
    }

    void Update()
    {
        //todo: use OverlapCircleAll instead?  currently, balls will only react based on collision directly in front of them
        int count = _body.Cast(_body.velocity, contactFilter, hitBuffer, rayCastDistance);
        if (count > 0)
        {
            float xVel = _body.velocity.x;
            float yVel = _body.velocity.y;
            RaycastHit2D hit = hitBuffer[0];
            Vector2 collisionPoint = hit.point;

            if (hit.collider.gameObject.name.Contains("Circle"))
                _audio.PlayOneShot(_ping);
            if (hit.collider.gameObject.gameObject.name.Contains("Square") || hit.collider.gameObject.tag.Contains("Boundary"))
                _audio.PlayOneShot(_ping);

            if (Mathf.Abs(collisionPoint.y - _body.transform.position.y) >= collisionBoundary)
            {
                yVel *= -1;
                //ensure ball is moving at top speed - not sure if this code actually works
                if (Mathf.Abs(yVel) < speed * Time.deltaTime)
                    yVel = Mathf.Sign(yVel) * speed * Time.deltaTime;
            }                
            if (Mathf.Abs(collisionPoint.x - _body.transform.position.x) >= collisionBoundary)
            {
                xVel *= -1;
                //ensure ball is moving at top speed - not sure if this code actually works
                if (Mathf.Abs(xVel) < speed * Time.deltaTime)
                    xVel = Mathf.Sign(xVel) * speed * Time.deltaTime;                
            }                
            _body.velocity = new Vector2(xVel, yVel);

            //if ball detects an extending wall, destroy the wall
            if(string.Equals(hit.transform.gameObject.tag, "Wall", System.StringComparison.OrdinalIgnoreCase))
            {
                if (hit.transform.gameObject.GetComponent<WallBehavior>().IsExtending() == true)
                {
                    Messenger.Broadcast(GameEvent.LIVE_LOST);
                    Destroy(hit.transform.gameObject);
                    _audio.PlayOneShot(_crash);
                }
                else
                    _audio.PlayOneShot(_ping);
            }
        }        
    }

}
