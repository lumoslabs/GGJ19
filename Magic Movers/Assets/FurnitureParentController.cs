using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureParentController : MonoBehaviour
{
    public GameObject furniturePrefab;
    private GameObject currentFurnitureObj;
    private FurnitureMovement currentFurnitureController;

    private Vector2 mainDirection;
    private Vector2 lateralDirection;

    public delegate void FurniturePlacedCallback();
    public FurniturePlacedCallback furniturePlacedCallback;

    public delegate void FurnitureCollisionCallback(GameObject defender, GameObject aggressor);
    public FurnitureCollisionCallback furnitureCollidedCallback;

    public delegate void FurnitureExitCallback();
    public FurnitureExitCallback furnitureExitCallback;


    public void AddFurniture()
    {
        currentFurnitureObj = Instantiate(furniturePrefab, transform);
        currentFurnitureObj.transform.localPosition = Vector3.zero;
        currentFurnitureController = currentFurnitureObj.GetComponent<FurnitureMovement>();
        currentFurnitureController.furnitureCollidedCallback = FurnitureCollidedCallback;
        currentFurnitureController.furnitureExitedCallback = FurnitureExitedCallback;
        currentFurnitureController.AssignPositionAndDirection();
        mainDirection = currentFurnitureController.GetCurrentVelocity().normalized;
        lateralDirection = FindLateralDirection(mainDirection);

    }

    private Vector2 FindLateralDirection(Vector2 direction)
    {

        if (direction == Vector2.left)
        {
            return Vector2.up;
        }
        else if (direction == Vector2.up)
        {
            return Vector2.right;
        }
        else if (direction == Vector2.right)
        {
            return Vector2.down;
        }
        else
        {
            return Vector2.left;
        }

    }

    void HandleFurnitureExitedCallback()
    {
    }


    public void FurnitureCollidedCallback(GameObject defender, GameObject aggressor)
    {
        this.furnitureCollidedCallback(defender, aggressor);
    }

    public void FurnitureExitedCallback()
    {
        this.furnitureExitCallback();
    }

    public void Place(int playerId)
    {
        currentFurnitureController.PlaceFurniture();
        furniturePlacedCallback();
    }

    public void Move(int playerId, float amt)
    {
        if (amt == 0)
        {
            currentFurnitureController.SetVelocity(mainDirection);
        }

        else if (playerId == 0)
        {
            currentFurnitureController.SetVelocity(mainDirection + lateralDirection);
        }
        else if (playerId == 1)
        {
            currentFurnitureController.SetVelocity(mainDirection - lateralDirection);
        }
    }
}
