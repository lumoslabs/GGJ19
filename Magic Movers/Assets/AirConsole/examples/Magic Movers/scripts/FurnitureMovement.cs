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

    private bool isActiveFurniture = false;
   
    public delegate void FurnitureCollidedCallback(GameObject defender, GameObject aggressor);
    public FurnitureCollidedCallback furnitureCollidedCallback;


    public delegate void FurnitureExitedCallback(GameObject defender);
    public FurnitureExitedCallback furnitureExitedCallback;


    public void Init()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    /*
     * TODO: tune up position-placement script to put it at the edges of the 
     * screen (or a little past) and move towards the opposite edge of the house
    */
    public void Start()
    {
        isActiveFurniture = true;
    }

    public void AssignPositionAndDirection()
    {
        int[] angles = new int[] { 0, 90, 180, 270 };
        isPositive = Random.value >= 0.5 ? true : false;
        direction = Random.value >= 0.5 ? Direction.horizontal : Direction.vertical;
        speed = Random.Range(7, 10);
        transform.rotation = Quaternion.Euler(0, 0, angles[Random.Range(0, angles.Length)]);

        if (direction == Direction.horizontal)
        {
            transform.position = new Vector3(isPositive ? -7 : 7, Random.Range(-3, 3));
            rb2d.velocity = new Vector2(speed * (isPositive ? 1 : -1), 0);
        }
        else
        {
            transform.position = new Vector3(Random.Range(-5, 5), isPositive ? -7 : 7);
            rb2d.velocity = new Vector2(0, speed * (isPositive ? 1 : -1));
        }
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

    public GameObject PlaceFurniture()
    {

        rb2d.velocity = new Vector2(0, 0);

        isActiveFurniture = false;

        return gameObject;

    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger entered!  " + collision.name);
        if (isActiveFurniture)
        {
            furnitureCollidedCallback(collision.gameObject, this.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Trigger exited! " + collision.name);
        if (isActiveFurniture)
        {
            furnitureExitedCallback(collision.gameObject);
        }
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
