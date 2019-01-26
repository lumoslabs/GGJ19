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

    public void AddFurniture()
    {
        currentFurnitureObj = Instantiate(furniturePrefab, transform);
        currentFurnitureObj.transform.localPosition = Vector3.zero;
        currentFurnitureController = currentFurnitureObj.GetComponent<FurnitureMovement>();
        currentFurnitureController.Start();
        lateralDirection = currentFurnitureController.direction == Direction.horizontal ? Vector2.up : Vector2.left;
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
            if (currentFurnitureController.direction == Direction.horizontal)
            {
                currentFurnitureController.SetVelocity(new Vector2(currentFurnitureController.GetCurrentVelocity().x, 0));
            }
            else
            {
                currentFurnitureController.SetVelocity(new Vector2(0, currentFurnitureController.GetCurrentVelocity().y));
            }
        }
        else if (playerId == 0)
        {
            currentFurnitureController.SetVelocity(currentFurnitureController.GetCurrentVelocity() + lateralDirection);
        }
        else if (playerId == 1)
        {
            currentFurnitureController.SetVelocity(currentFurnitureController.GetCurrentVelocity() - lateralDirection);
        }
    }
}
