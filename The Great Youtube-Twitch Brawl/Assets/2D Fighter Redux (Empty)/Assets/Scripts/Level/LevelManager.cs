using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelManager : MonoBehaviour {

    WaitForSeconds oneSec; //cutting out redundancies, we will only have one variable.
    public Transform[] spawnPosition; //characters will spawn on these positions


    CharacterManager charM;
    LevelUI levelUI; //ease of access

    public int maxTurns = 2; //how many turns for victory
    int currentTurn; //current turn


    [Header("Countdown Variables")]
    public bool countDown;
    public int maxTurnTimer = 30;
    int currentTimer;
    float internalTimer;

	void Start () {
        //grab references
        charM = CharacterManager.GetInstance();
        levelUI = LevelUI.GetInstance();


        //initialize the wait.
        oneSec = new WaitForSeconds(1);

        levelUI.AnnouncerTextLine1.gameObject.SetActive(false);
        levelUI.AnnouncerTextLine2.gameObject.SetActive(false);

        StartCoroutine(StartGame());
    }

    private void FixedUpdate()
    {
        //handle player orientation in the scene; compare X positions and judge who is where.
        if(charM.players[0].playerStates.transform.position.x<charM.players[1].playerStates.transform.position.x)
        {
            charM.players[0].playerStates.lookRight = true;
            charM.players[1].playerStates.lookRight = false;
        }
        else
        {
            charM.players[0].playerStates.lookRight = false;
            charM.players[1].playerStates.lookRight = true;
        }
    }

    void Update () {
		if(countDown)
        {
            HandleTurnTimer(); // control the timer.
        }
	}

    void HandleTurnTimer()
    {
        levelUI.LevelTimer.text = currentTimer.ToString();
        internalTimer += Time.deltaTime; 

        if(internalTimer > 1)
        {
            currentTimer--;
            internalTimer = 0;
        }

        if(currentTimer <=0)
        {
            EndTurnFunction(true); //end the turn
            countDown = false;
        }

    }

    IEnumerator StartGame()
    {
        //Game Start

        //Create Players
        yield return CreatePlayers();

        //Initialize turn
        yield return InitTurn();
    }

    IEnumerator InitTurn()
    {
        //every time a new turn is started. initialize

        //disable announcer lines
        levelUI.AnnouncerTextLine1.gameObject.SetActive(false);
        levelUI.AnnouncerTextLine2.gameObject.SetActive(false);

        //reset the timer
        currentTimer = maxTurnTimer;
        countDown = false;

        //start player initialization;
        yield return InitPlayers();

        //give players control
        yield return EnableControl();
    }

    IEnumerator CreatePlayers()
    {
        //grab all players
        for (int i = 0; i < charM.players.Count; i++)
        {
            //instantiate their prefabs
            GameObject go = Instantiate(charM.players[i].playerPrefab, spawnPosition[i].position, Quaternion.identity) as GameObject;

            //assign all references needed.
            charM.players[i].playerStates = go.GetComponent<StateManager>();

            charM.players[i].playerStates.healthSlider = levelUI.healthSliders[i];
        }
        yield return null;
    }

    IEnumerator InitPlayers()
    {
        //reset the players
        for (int i = 0; i < charM.players.Count; i++)
        {
            charM.players[i].playerStates.health = 100;
            //play their start animations - look into next round if statements
            //charM.players[i].handleAnim.anim.Play("Locomotion");

            //resets the player position -- look into if we want this or not
            charM.players[i].playerStates.transform.position = spawnPosition[i].position;
        }
        yield return null;
    }

    IEnumerator EnableControl()
    {
        //start with the announcer text
        levelUI.AnnouncerTextLine1.gameObject.SetActive(true);
        levelUI.AnnouncerTextLine1.text = "Round" + currentTurn;
        levelUI.AnnouncerTextLine1.color = Color.white;
        yield return oneSec;
        yield return oneSec;


        //change UI Text every second
        levelUI.AnnouncerTextLine1.text = "Ready?";
        levelUI.AnnouncerTextLine1.color = Color.green;
        yield return oneSec;
        levelUI.AnnouncerTextLine1.text = "BRAWL!";
        levelUI.AnnouncerTextLine1.color = Color.red;
        yield return oneSec;

        for (int i = 0; i < charM.players.Count; i++)
        {
            //enable the input handler for players
            if(charM.players[i].playerType == PlayerBase.PlayerType.user)
            {
                InputHandler ih = charM.players[i].playerStates.gameObject.GetComponent<InputHandler>();
                ih.playerInput = charM.players[i].inputId;
                ih.enabled = true;
            }
        }

        //disable the announcer text
        yield return oneSec;
        levelUI.AnnouncerTextLine1.gameObject.SetActive(false);
        countDown = true;
    }

    void DisableControl()
    {
        //disable character components
        for (int i = 0; i < charM.players.Count; i++)
        {
            //reset variables
            charM.players[i].playerStates.ResetStateInputs();

            //disable for players
            if(charM.players[i].playerType == PlayerBase.PlayerType.user)
            {
                charM.players[i].playerStates.GetComponent<InputHandler>().enabled = false;
            }

            //add diasble for AI here...eventually.
        }
    }

    public void EndTurnFunction(bool timeOut = false)
    {
        // end the turn. keep track of if it's by time out or not.
        countDown = false;

        //reset the timer
        levelUI.LevelTimer.text = maxTurnTimer.ToString();

        //if it's a timeout
        if(timeOut)
        {
            levelUI.AnnouncerTextLine1.gameObject.SetActive(true);
            levelUI.AnnouncerTextLine1.text = "TIME'S UP";
            levelUI.AnnouncerTextLine1.color = Color.red;
        }
        else
        {
            levelUI.AnnouncerTextLine1.gameObject.SetActive(true);
            levelUI.AnnouncerTextLine1.text = "K.O.";
            levelUI.AnnouncerTextLine1.color = Color.red;
        }

        DisableControl();

        StartCoroutine(EndTurn());


    }

    IEnumerator EndTurn()
    {
        //wait a few seconds for previous text
        yield return oneSec;
        yield return oneSec;
        yield return oneSec;

        //find the victor
        PlayerBase vPlayer = FindWinningPlayer();

        //is it a draw?
        if(vPlayer == null)
        {
            //yes. so announce it
            levelUI.AnnouncerTextLine1.text = "DRAW";
            levelUI.AnnouncerTextLine1.color = Color.blue;
        }
        else
        {
            //vPlayer is the winner!
            levelUI.AnnouncerTextLine1.text = vPlayer.playerId + "WINS";
            levelUI.AnnouncerTextLine1.color = Color.red;
        }

        //wait some more;
        yield return oneSec;
        yield return oneSec;
        yield return oneSec;


        //has the winner taken damage?
        if(vPlayer != null)
        {
            //no? FLAWLESS VICTORY
            if (vPlayer.playerStates.health == 100)
            {
                levelUI.AnnouncerTextLine2.gameObject.SetActive(true);
                levelUI.AnnouncerTextLine2.text = "FLAWLESS VICTORY";

            }
        }

        //more waiting here
        yield return oneSec;
        yield return oneSec;
        yield return oneSec;

        //next turn
        currentTurn++;

        bool matchOver = isMatchOver();

        if(!matchOver)
        {
            StartCoroutine(InitTurn()); //start from the top
        }
        else
        {
            //reset to selection. Replace with menu later
            for (int i = 0; i < charM.players.Count; i++)
            {
                charM.players[i].hasCharacter = false;
            }
            SceneManager.LoadSceneAsync("select");
        }


    }

    bool isMatchOver()
    {
        bool retVal = false;

        for (int i = 0; i < charM.players.Count; i++)
        {
            if (charM.players[i].score >= maxTurns)
            {
                retVal = true;
                break;
            }
        }

        return retVal;
    }


    PlayerBase FindWinningPlayer()
    {
        //to find out who won
        PlayerBase retVal = null;

        StateManager targetPlayer = null;

        //check to see if both players have equal health;
        if(charM.players[0].playerStates.health != charM.players[1].playerStates.health)
        {
            //no? who is lower? higher is winner
            if(charM.players[0].playerStates.health < charM.players[1].playerStates.health)
            {
                charM.players[1].score++;
                targetPlayer = charM.players[1].playerStates;
                levelUI.AddWinIndicator(1);
            }
            else
            {
                charM.players[0].score++;
                targetPlayer = charM.players[0].playerStates;
                levelUI.AddWinIndicator(0);
            }

            retVal = charM.returnPlayerFromStates(targetPlayer);
        }
        return retVal;
    }


    public static LevelManager instance;
    public static LevelManager GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
    }


}
