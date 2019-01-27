using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using UnityEngine.SceneManagement;

public class MagicMoversLogic : MonoBehaviour {

    public FurnitureParentController furnitureParentController;
    public GameObject replayText;
	public float ballSpeed = 10f;
	public Text uiText;
#if !DISABLE_AIRCONSOLE 
	private int scoreRacketLeft = 0;
	private int scoreRacketRight = 0;

    private ArrayList currentDefenders;
    private ArrayList placedFurnitureList;
    private GameObject currentAggressor;
    private bool gameOver;
    public StrikeController strikeController;

	void Awake () {
		AirConsole.instance.onMessage += OnMessage;
		AirConsole.instance.onConnect += OnConnect;
		AirConsole.instance.onDisconnect += OnDisconnect;

        furnitureParentController.furniturePlacedCallback = FurniturePlacedCallback;
        furnitureParentController.furnitureCollidedCallback = HandleFurnitureCollisionCallback;
        furnitureParentController.furnitureExitCallback = HandleFurnitureExitCallback;


        currentDefenders = new ArrayList();
        placedFurnitureList = new ArrayList();
        gameOver = false;
    }

    /// <summary>
    /// We start the game if 2 players are connected and the game is not already running (activePlayers == null).
    /// 
    /// NOTE: We store the controller device_ids of the active players. We do not hardcode player device_ids 1 and 2,
    ///       because the two controllers that are connected can have other device_ids e.g. 3 and 7.
    ///       For more information read: http://developers.airconsole.com/#/guides/device_ids_and_states
    /// 
    /// </summary>
    /// <param name="device_id">The device_id that connected</param>
    void OnConnect (int device_id) {
		if (AirConsole.instance.GetActivePlayerDeviceIds.Count == 0) {
			if (AirConsole.instance.GetControllerDeviceIds ().Count >= 2) {
				StartGame ();
			} else {
				uiText.text = "NEED MORE PLAYERS";
			}
		}
	}

	/// <summary>
	/// If the game is running and one of the active players leaves, we reset the game.
	/// </summary>
	/// <param name="device_id">The device_id that has left.</param>
	void OnDisconnect (int device_id) {
		int active_player = AirConsole.instance.ConvertDeviceIdToPlayerNumber (device_id);
		if (active_player != -1) {
			if (AirConsole.instance.GetControllerDeviceIds ().Count >= 2) {
				StartGame ();
			} else {
				AirConsole.instance.SetActivePlayers (0);
				uiText.text = "PLAYER LEFT - NEED MORE PLAYERS";
			}
		}
	}

	/// <summary>
	/// We check which one of the active players has moved the paddle.
	/// </summary>
	/// <param name="from">From.</param>
	/// <param name="data">Data.</param>
	void OnMessage (int device_id, JToken data) {

        //if game is over, restart
        if (gameOver == true)
        {
            Debug.Log("Restarting scene");
            RestartGame();
            return;
        }

        int active_player = AirConsole.instance.ConvertDeviceIdToPlayerNumber (device_id);
		if (active_player != -1) {



            //if someone has pressed PLACE
            if ((bool) data["place"] == true)
            { 
                PlayerPlaced(active_player);
            }

            //if someone has pressed MOVE
            else
            { 
                PlayerMoved(active_player, (float)data["move"]);
            }


		}
	}

    private void RestartGame()
    {
        strikeController.RestartGame();
        replayText.SetActive(false);
        for (int i = 0; i < placedFurnitureList.Count; i++)
        {
            Destroy(placedFurnitureList[i] as GameObject);
        }
        placedFurnitureList.Clear();
        gameOver = false;
        StartCoroutine(AddFurnitureAfterDelay());


    }

    void PlayerPlaced(int playerId)
    {
        Debug.Log("Placed: Player " + playerId);
        GameObject furnitureObj =  furnitureParentController.Place(playerId);
        placedFurnitureList.Add(furnitureObj);
    }

    void PlayerMoved(int playerId, float amt)
    {
        Debug.Log("Moved: Player " + playerId + ", Amt " + amt);
        furnitureParentController.Move(playerId, amt);
    }

    void SetMoveSpeed(int direction)
    {

    }

    void FurniturePlacedCallback()
    {
        if(currentDefenders.Count > 0)
        {
            DestroyCurrentFurniture();
        }
        else
        {
            StartCoroutine(AddFurnitureAfterDelay());
        }

    }

    IEnumerator AddFurnitureAfterDelay()
    {
        yield return new WaitForSeconds(2);
        AddFurniture();
    }

    private void DestroyCurrentFurniture()
    {
        for (int i = 0; i < currentDefenders.Count; i++)
        {
            if ((currentDefenders[i] as GameObject).tag == "Furniture")
            {
                Destroy(currentDefenders[i] as GameObject);
            }
        }
        Destroy(currentAggressor);
        currentDefenders.Clear();
        currentAggressor = null;

        GiveStrike();
    }

    private void GiveStrike()
    {
        bool shouldEndGame = strikeController.GiveStrike();
        if (shouldEndGame)
        {
            EndGame();
        }
        else
        {
            StartCoroutine(AddFurnitureAfterDelay());
        }


    }

    private void EndGame()
    {
        Debug.Log("GAMEOVER");
        gameOver = true;
        replayText.SetActive(true);
    }

    void HandleFurnitureCollisionCallback(GameObject defender, GameObject aggressor)
    {
        if (defender.tag == "Boundary")
        {
            DestroyCurrentFurniture();
        }
        else
        {
            currentDefenders.Add(defender);
            currentAggressor = aggressor;
        }
    }

    void HandleFurnitureExitCallback(GameObject defender)
    {
        currentDefenders.Remove(defender);
    }

    void StartGame () {
		AirConsole.instance.SetActivePlayers (2);
        AddFurniture();
		//ResetBall (true);
		//scoreRacketLeft = 0;
		//scoreRacketRight = 0;
		UpdateScoreUI ();
	}

    void AddFurniture()
    {
        furnitureParentController.AddFurniture();
    }


	void UpdateScoreUI () {
		// update text canvas
		uiText.text = scoreRacketLeft + ":" + scoreRacketRight;
	}

	void FixedUpdate () {

		// check if ball reached one of the ends
		//if (this.ball.position.x < -9f) {
		//	scoreRacketRight++;
		//	UpdateScoreUI ();
		//	ResetBall (true);
		//}

		//if (this.ball.position.x > 9f) {
		//	scoreRacketLeft++;
		//	UpdateScoreUI ();
		//	ResetBall (true);
		//}
	}

	void OnDestroy () {

		// unregister airconsole events on scene change
		if (AirConsole.instance != null) {
			AirConsole.instance.onMessage -= OnMessage;
		}
	}
#endif
}
