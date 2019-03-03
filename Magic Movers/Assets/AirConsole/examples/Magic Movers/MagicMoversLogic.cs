using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using UnityEngine.SceneManagement;
using FMODUnity;

public class MagicMoversLogic : MonoBehaviour {

    public FurnitureParentController furnitureParentController;
    public GameObject replayText;
    public WizardAnimatorController wiz1;
    public WizardAnimatorController wiz2;

    public Transform leftBoundary, rightBoundary, topBoundary, bottomBoundary;

    public Camera camera;
    private AudioSource audio;
    private AudioClipHolder clipHolder;

	public float ballSpeed = 10f;
	public Text uiText;
#if !DISABLE_AIRCONSOLE 
	private int scoreRacketLeft = 0;
	private int scoreRacketRight = 0;

    private ArrayList currentDefenders;
    private ArrayList placedFurnitureList;
    private GameObject currentAggressor;
    private bool inputEnabled;
    private bool gameOver;
    public StrikeController strikeController;
    public TitleScreenController titleScreenController;
    private bool gameStarted = false;
    public WizardAudio wizardAudio;
    private int lastPlayerId;

    private bool isPaused = false;

	void Awake () {
        GetComponent<AudioSource>().Play();

		AirConsole.instance.onMessage += OnMessage;
		AirConsole.instance.onConnect += OnConnect;
		AirConsole.instance.onDisconnect += OnDisconnect;

        furnitureParentController.Init(leftBoundary.localPosition, rightBoundary.localPosition, topBoundary.localPosition, bottomBoundary.localPosition); 
        furnitureParentController.furniturePlacedCallback = FurniturePlacedCallback;
        furnitureParentController.furnitureCollidedCallback = HandleFurnitureCollisionCallback;
        furnitureParentController.furnitureExitCallback = HandleFurnitureExitCallback;

        audio = camera.GetComponent<AudioSource>();
        clipHolder = camera.GetComponent<AudioClipHolder>();

        currentDefenders = new ArrayList();
        placedFurnitureList = new ArrayList();
        gameOver = false;
    }

    private void Start()
    {
       
    }

    public void OnPauseGame()
    {
        this.isPaused = true;
        furnitureParentController.OnPauseGame();    
    }

    public void onResumeGame()
    {
        this.isPaused = false;
        furnitureParentController.OnResumeGame();    
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
			if (AirConsole.instance.GetControllerDeviceIds ().Count == 2) 
            {
                AirConsole.instance.SetActivePlayers(2);
                uiText.text = "";
                EnableInput();
                if (this.isPaused)
                {
                    this.onResumeGame();
                }
			} 
            else
            {
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
				// StartGame ();
			} else {
				AirConsole.instance.SetActivePlayers (0);
				uiText.text = "GAME PAUSED - PLEASE RECONNECT BOTH PLAYERS";
                Debug.Log("Player Disconnected - not enough to continue");
                this.OnPauseGame();
                // Debug.Break();
                // Scene scene = SceneManager.GetActiveScene(); 
                // SceneManager.LoadScene("magicMovers");
			}
		}
	}

	/// <summary>
	/// We check which one of the active players has moved the paddle.
	/// </summary>
	/// <param name="from">From.</param>
	/// <param name="data">Data.</param>
	void OnMessage (int device_id, JToken data) {
        if (inputEnabled == false)
        {
            return;
        }
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
                if (gameStarted)
                {
                    PlayerPlaced(active_player);
                }
                else
                {
                    titleScreenController.Play();
                }
            }

            //if someone has pressed MOVE
            else
            {
                if (gameStarted)
                {
                    PlayerMoved(active_player, (float)data["move"]);
                }
                else if ((float)data["move"] != 0)
                {
                    titleScreenController.Play();
                }
            }
        }
	}

    private void RestartGame()
    {
        DisableInput();
        strikeController.RestartGame();

        audio.clip = clipHolder.clips[0];
        audio.loop = true;
        audio.Play();

        wiz1.PlayWalk();
        wiz2.PlayWalk();

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
        if(playerId == 0)
        {
            wiz2.PlayWin();
        }
        else
        {
            wiz1.PlayWin();
        }
        lastPlayerId = playerId + 1;
    }

    void PlayerMoved(int playerId, float amt)
    {
        Debug.Log("Moved: Player " + playerId + ", Amt " + amt);
        furnitureParentController.Move(playerId, amt);
        if(playerId == 0)
        {
            if (amt > 0)
            {
                wiz2.PlayPull();
                wizardAudio.player2magic.volume = 1;
            }
            else
            {
                wiz2.PlayFloat();
                wizardAudio.player2magic.volume = 0;
            }
        }
        else
        {
            if (amt > 0)
            {
                wiz1.PlayPull();
                wizardAudio.player1magic.volume = 1;
            }
            else
            {
                wiz1.PlayFloat();
                wizardAudio.player1magic.volume = 0;
            }
        }
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
            // Play place correct audio
            if (lastPlayerId == 1)
            {
                wizardAudio.player1Audio.clip = wizardAudio.clips1.clips[Random.Range(16, 24)];
                wizardAudio.player1Audio.loop = false;
                wizardAudio.player1Audio.volume = 1;
                wizardAudio.player1Audio.Play();
            }
            else
            {
                wizardAudio.player2Audio.clip = wizardAudio.clips2.clips[Random.Range(16, 24)];
                wizardAudio.player2Audio.loop = false;
                wizardAudio.player2Audio.volume = 1;
                wizardAudio.player2Audio.Play();
            }

            StartCoroutine(AddFurnitureAfterDelay());
        }

    }

    IEnumerator AddFurnitureAfterDelay()
    {
        DisableInput();
        yield return new WaitForSeconds(2);
        AddFurniture();
        lastPlayerId = 0;
    }

    public void DisableInput()
    {
        inputEnabled = false;
    }

    IEnumerator EnableInputAfterDelay()
    {
        yield return new WaitForSeconds(2);
        EnableInput();
    }

    public void EnableInput()
    {
        inputEnabled = true;
    }

    private void DestroyCurrentFurniture(bool destroyImmediate = false)
    {
        if (destroyImmediate == false)
        {
            currentAggressor.GetComponent<FurnitureAnimationController>().PlayBreak();
        }
        else
        {
            Destroy(currentAggressor.gameObject);
        }
        for (int i = 0; i < currentDefenders.Count; i++)
        {
            if ((currentDefenders[i] as GameObject).tag == "Furniture") 
            {
                GameObject item = currentDefenders[i] as GameObject;
                //that object plays break animation
                item.GetComponent<FurnitureAnimationController>().PlayBreak();

                //turns tag to "grass"
                item.tag = "Grass";
                //Destroy(currentDefenders[i] as GameObject);
            }
        }
        //Destroy(currentAggressor);

        currentDefenders.Clear();
        currentAggressor = null;

        if (lastPlayerId == 1)
        {
            wizardAudio.player1Audio.clip = wizardAudio.clips1.clips[Random.Range(0, 16)];
            wizardAudio.player1Audio.loop = false;
            wizardAudio.player1Audio.volume = 1;
            wizardAudio.player1Audio.Play();
        }
        else
        {
            wizardAudio.player2Audio.clip = wizardAudio.clips2.clips[Random.Range(0, 16)];
            wizardAudio.player2Audio.loop = false;
            wizardAudio.player2Audio.volume = 1;
            wizardAudio.player2Audio.Play();
        }

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
        lastPlayerId = 0;
    }

    private void EndGame()
    {
        Debug.Log("GAMEOVER");
        
        DisableInput();

        gameOver = true;
        replayText.SetActive(true);

        audio.clip = clipHolder.clips[1];
        audio.loop = false;
        audio.Play();

        wiz1.PlayLose();
        wiz2.PlayLose();

        //enable input after a short timeout
        StartCoroutine(EnableInputAfterDelay());
    }

    void HandleFurnitureCollisionCallback(GameObject defender, GameObject aggressor)
    {
        if (defender.tag == "Boundary")
        {
            DestroyCurrentFurniture(true);
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
        gameStarted = true;
		AirConsole.instance.SetActivePlayers (2);
        StartCoroutine(AddFurnitureAfterDelay());
		UpdateScoreUI ();
        strikeController.gameObject.SetActive(true);
        wiz1.PlayWalk();
        wiz2.PlayWalk();
    }

    void AddFurniture()
    {
        furnitureParentController.AddFurniture();
        EnableInput();
    }

	void UpdateScoreUI () {
		// update text canvas
		uiText.text = "";
	}

	void OnDestroy () {

		// unregister airconsole events on scene change
		if (AirConsole.instance != null) {
			AirConsole.instance.onMessage -= OnMessage;
		}
	}

    private void Update()
    {
        if (titleScreenController.TitleFinished() && !gameStarted)
        {
            StartGame();

        }
    }
#endif
}
