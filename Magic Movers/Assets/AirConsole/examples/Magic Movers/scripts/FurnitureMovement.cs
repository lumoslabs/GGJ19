using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction { horizontal, vertical };

[RequireComponent(typeof(Rigidbody2D), typeof(PolygonCollider2D))]
public class FurnitureMovement : MonoBehaviour
{
    [Range(0, 10)] public float speed = 5;
    public bool isPositive;
    public Direction direction;
    private Rigidbody2D rb2d;


    /*
     * TODO: tune up position-placement script to put it at the edges of the 
     * screen (or a little past) and move towards the opposite edge of the house
    */
    public void Start()
    {
        isPositive = Random.value >= 0.5 ? true : false;
        direction = Random.value >= 0.5 ? Direction.horizontal : Direction.vertical;

        rb2d = GetComponent<Rigidbody2D>();

        if (direction == Direction.horizontal)
        {
            transform.position = new Vector3(isPositive ? -10 : 10, transform.position.y);
            rb2d.velocity = new Vector2(speed * (isPositive ? 1 : -1), 0);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, isPositive ? -3 : 3);
            rb2d.velocity = new Vector2(0, speed * (isPositive ? 1 : -1));
        }

        //adding this in just for testing
        //transform.position = new Vector2(-15, 0);
        //rb2d.velocity = new Vector2(speed, 0);
    }

    private void Update()
    {
        /* TODO: Place this in a listener/event instead of Update(), this 
         * eventually will be a callback, since another script will trigger 
         * placing the furniture
        */
        if (Input.GetButton("Jump"))
        {
            PlaceFurniture();
        }
    }

    public void PlaceFurniture()
    {
        rb2d.velocity = new Vector2(0, 0);
    }

    public Vector2 GetCurrentVelocity()
    {
        return rb2d.velocity;
    }

    public void SetVelocity(Vector2 vel)
    {
        rb2d.velocity = vel;
    }

}
