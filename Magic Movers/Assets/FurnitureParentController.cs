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
        mainDirection = Vector2.right;
        lateralDirection = Vector2.up;


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
