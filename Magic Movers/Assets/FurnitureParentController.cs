using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureParentController : MonoBehaviour
{
    public Color clearColor;
    public Color blueColor;
    public Color redColor;
    public SpriteRenderer upArrowSprite;
    public SpriteRenderer rightArrowSprite;
    public SpriteRenderer downArrowSprite;
    public SpriteRenderer leftArrowSprite;

    public FurnitureAnimationController animController;


    public GameObject[] furniturePrefabs;
    private GameObject furniturePrefab;
    private GameObject currentFurnitureObj;
    private FurnitureMovement currentFurnitureController;

    private Vector2 mainDirection;
    private Vector2 lateralDirection;

    public delegate void FurniturePlacedCallback();
    public FurniturePlacedCallback furniturePlacedCallback;

    public delegate void FurnitureCollisionCallback(GameObject defender, GameObject aggressor);
    public FurnitureCollisionCallback furnitureCollidedCallback;

    public delegate void FurnitureExitCallback(GameObject defender);
    public FurnitureExitCallback furnitureExitCallback;

    private AudioSource audio;
    private FurnitureClipHolder clipHolder;

    private void Start()
    {
        HideArrows();
        audio = GetComponent<AudioSource>();
        clipHolder = GetComponent<FurnitureClipHolder>();
    }

    public void AddFurniture()
    {
        furniturePrefab = (GameObject)furniturePrefabs.GetValue(Random.Range(0, furniturePrefabs.Length));
        audio.clip = clipHolder.RandomSpawnClip();
        audio.Play();
        currentFurnitureObj = Instantiate(furniturePrefab, transform);
        currentFurnitureObj.transform.localPosition = Vector3.zero;
        animController = currentFurnitureObj.GetComponent<FurnitureAnimationController>();
        currentFurnitureController = currentFurnitureObj.GetComponent<FurnitureMovement>();
        currentFurnitureController.furnitureCollidedCallback = FurnitureCollidedCallback;
        currentFurnitureController.furnitureExitedCallback = FurnitureExitedCallback;
        currentFurnitureController.Init();
        currentFurnitureController.AssignPositionAndDirection();
        mainDirection = currentFurnitureController.GetCurrentVelocity().normalized;
        lateralDirection = FindLateralDirection(mainDirection);

    }

    public void OnPauseGame()
    {
        currentFurnitureObj.GetComponent<FurnitureMovement>().OnPauseGame();    
    }

    public void OnResumeGame()
    {
        currentFurnitureObj.GetComponent<FurnitureMovement>().OnResumeGame();    
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

    void HandleFurnitureExitedCallback(GameObject defender)
    {
        this.furnitureExitCallback(defender);
    }


    public void FurnitureCollidedCallback(GameObject defender, GameObject aggressor)
    {
        this.furnitureCollidedCallback(defender, aggressor);
    }

    public void FurnitureExitedCallback(GameObject defender)
    {
        this.furnitureExitCallback(defender);
    }

    public GameObject Place(int playerId)
    {
        if (playerId == 1)
        {
            audio.clip = clipHolder.RandomPlaceClip1();
        }

        GameObject furnitureObj = currentFurnitureController.PlaceFurniture();
        furniturePlacedCallback();
        animController.PlayPlace();
        animController.SetColor(playerId);

        return furnitureObj;
    }

    public void Move(int playerId, float amt)
    {
        if (amt == 0)
        {
            HideArrows();

            animController.SetColor(2); 

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
            animController.SetColor(playerId);

            //drag LEFT
            if (currentFurnitureController.direction == Direction.vertical)
            {
                leftArrowSprite.color = blueColor;
                currentFurnitureController.SetVelocity(currentFurnitureController.GetCurrentVelocity() + Vector2.left * currentFurnitureController.speed);
            }
            //drag UP
            else
            {
                upArrowSprite.color = blueColor;
                currentFurnitureController.SetVelocity(currentFurnitureController.GetCurrentVelocity() + Vector2.up * currentFurnitureController.speed);
            }
            //currentFurnitureController.SetVelocity(currentFurnitureController.GetCurrentVelocity() + lateralDirection * currentFurnitureController.speed);
        }
        else if (playerId == 1)
        {
            animController.SetColor(playerId);

            //drag RIGHT
            if (currentFurnitureController.direction == Direction.vertical)
            {
                rightArrowSprite.color = redColor;
                currentFurnitureController.SetVelocity(currentFurnitureController.GetCurrentVelocity() + Vector2.right * currentFurnitureController.speed);
            }
            //drag DOWN
            else
            {
                downArrowSprite.color = redColor;
                currentFurnitureController.SetVelocity(currentFurnitureController.GetCurrentVelocity() + Vector2.down * currentFurnitureController.speed);
            }
            //currentFurnitureController.SetVelocity(currentFurnitureController.GetCurrentVelocity() - lateralDirection * currentFurnitureController.speed);
        }
    }

    private void HideArrows()
    {
        leftArrowSprite.color = clearColor;
        rightArrowSprite.color = clearColor;
        upArrowSprite.color = clearColor;
        downArrowSprite.color = clearColor;
    }
}
