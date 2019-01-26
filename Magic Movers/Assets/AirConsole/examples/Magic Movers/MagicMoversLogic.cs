using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;

public class MagicMoversLogic : MonoBehaviour {

    public FurnitureParentController furnitureParentController;
	public Rigidbody2D racketLeft;
	public Rigidbody2D racketRight;
	public Rigidbody2D ball;
	public float ballSpeed = 10f;
	public Text uiText;
#if !DISABLE_AIRCONSOLE 
	private int scoreRacketLeft = 0;
	private int scoreRacketRight = 0;

    private ArrayList currentDefenders;
    private GameObject currentAggressor;

	void Awake () {
		AirConsole.instance.onMessage += OnMessage;
		AirConsole.instance.onConnect += OnConnect;
		AirConsole.instance.onDisconnect += OnDisconnect;

        furnitureParentController.furniturePlacedCallback = FurniturePlacedCallback;
        furnitureParentController.furnitureCollidedCallback = HandleFurnitureCollisionCallback;
        furnitureParentController.furnitureExitCallback = HandleFurnitureExitCallback;


        currentDefenders = new ArrayList();
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
				ResetBall (false);
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

    void PlayerPlaced(int playerId)
    {
        Debug.Log("Placed: Player " + playerId);
        furnitureParentController.Place(playerId);
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
            for(int i = 0; i < currentDefenders.Count; i++)
            {
                Destroy(currentDefenders[i] as GameObject);
            }
            Destroy(currentAggressor);
            currentDefenders.Clear();
            currentAggressor = null;
        }
        AddFurniture();
    }

    void HandleFurnitureCollisionCallback(GameObject defender, GameObject aggressor)
    {
        currentDefenders.Add(defender);
        currentAggressor = aggressor;
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

    void ResetBall (bool move) {
		
		// place ball at center
		this.ball.position = Vector3.zero;
		
		// push the ball in a random direction
		if (move) {
			Vector3 startDir = new Vector3 (Random.Range (-1, 1f), Random.Range (-0.1f, 0.1f), 0);
			this.ball.velocity = startDir.normalized * this.ballSpeed;
		} else {
			this.ball.velocity = Vector3.zero;
		}
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
