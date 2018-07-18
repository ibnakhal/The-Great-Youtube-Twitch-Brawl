using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class SelectScreenManager : MonoBehaviour
{

    public int numberOfPlayers = 1;
    public List<PlayerInterfaces> plInterfaces = new List<PlayerInterfaces>();
    public PortraitInfo[] portraitPrefabs;
    public int MaxX;
    public int maxY;
    PortraitInfo[,] charGrid;

    public GameObject portraitCanvas;

    bool loadLevel;
    public bool bothPlayersSelected;

    CharacterManager charManager;


    public float LevelLoadWaitTime;
    public string LevelToLoad;

    public float SelectMoveDelay;

    #region Singleton
    public static SelectScreenManager instance;
    public static SelectScreenManager GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
    }
    #endregion

    private void Start()
    {
        charManager = CharacterManager.GetInstance();
        numberOfPlayers = charManager.numberOfUsers;

        charGrid = new PortraitInfo[MaxX, maxY];
        int x = 0;
        int y = 0;

        portraitPrefabs = portraitCanvas.GetComponentsInChildren<PortraitInfo>();

        for (int i = 0; i < portraitPrefabs.Length ; i++)
        {
            portraitPrefabs[i].posX += x;
            portraitPrefabs[i].posY += y;

            charGrid[x, y] = portraitPrefabs[i];

            if (x < MaxX - 1)
            {
                x++;
            }
            else
            {
                x = 0;
                y++;
            }
        }
    }

    


    private void Update()
    {
        if(!loadLevel)
        {
            for (int i = 0; i < plInterfaces.Count; i++)
            {
                if(i<numberOfPlayers)
                {
                    if(!charManager.players[i].hasCharacter)
                    {
                        plInterfaces[i].playerBase = charManager.players[i];
                        HandleSelectorPosition(plInterfaces[i]);
                        HandleScreenInput(plInterfaces[i], charManager.players[i].inputId);
                        HandleSelectorPreview(plInterfaces[i]);
                    }
                }
                else
                {
                    charManager.players[i].hasCharacter = true;
                }
            }
        }
        //change this when you want to add level select
        if(bothPlayersSelected)
        {
            StartCoroutine(LoadLevel());
            loadLevel = true;
        }
        else
        {
            if(charManager.players[0].hasCharacter && charManager.players[1].hasCharacter)
            {
                bothPlayersSelected = true;
            }
        }
    }

    void HandleScreenInput(PlayerInterfaces pl, string playerId)
    {
        #region Grid Navigation
        #region vertical
        float vertical = Input.GetAxis("Vertical" + playerId);

        if (vertical !=0)
        {
            if(!pl.hitInputOnce)
            {
                if(vertical > 0)
                {
                    pl.ActiveY = (pl.ActiveY > 0) ? pl.ActiveY - 1 : maxY - 1;
                }
                else
                {
                    pl.ActiveY = (pl.ActiveY < maxY - 1) ? pl.ActiveY + 1 : 0;
                }
                pl.hitInputOnce = true;
            }
        }
        #endregion
        #region Horizontal
       float horizontal = Input.GetAxis("Horizontal" + playerId);
       if(horizontal !=0)
        {
            if(!pl.hitInputOnce)
            {
                if(horizontal > 0)
                {
                    pl.ActiveX = (pl.ActiveX > 0) ? pl.ActiveX - 1 : MaxX - 1;
                }
                else
                {
                    pl.ActiveX = (pl.ActiveX < MaxX - 1) ? pl.ActiveX + 1 : 0;
                }
                pl.timerToReset = 0;
                pl.hitInputOnce = true;
            }
        }
        #endregion

        if (vertical == 0 && horizontal == 0)
        {
            pl.hitInputOnce = false;
        }

        if(pl.hitInputOnce)
        {
            pl.timerToReset += Time.deltaTime;

            if(pl.timerToReset > SelectMoveDelay)
            {
                pl.hitInputOnce = false;
                pl.timerToReset = 0;
            }
        }
        #endregion

        //character has been selected
        if (Input.GetButtonUp("Block" + playerId))
        {
            //reaction goes here, 

            //tell the character manager what preafb to instantiate
            pl.playerBase.playerPrefab = charManager.returnCharacterWithID(pl.activePortrait.characterId).prefab;

            pl.playerBase.hasCharacter = true;

        }


        
    }

    IEnumerator LoadLevel()
    {
        for (int i = 0; i < charManager.players.Count; i++)
        {
            if(charManager.players[i].playerType == PlayerBase.PlayerType.ai)
            {
                if(charManager.players[i].playerPrefab == null)
                {
                    int randomValue = Random.Range(0, portraitPrefabs.Length);
                    charManager.players[i].playerPrefab = charManager.returnCharacterWithID(portraitPrefabs[randomValue].characterId).prefab;

                    Debug.Log(portraitPrefabs[randomValue].characterId);
                }
            }
        }

        yield return new WaitForSeconds(LevelLoadWaitTime);
        SceneManager.LoadSceneAsync(LevelToLoad, LoadSceneMode.Single);
    }

    void HandleSelectorPosition(PlayerInterfaces pl)
    {
        pl.selector.SetActive(true);

        pl.activePortrait = charGrid[pl.ActiveX, pl.ActiveY];

        Debug.Log(pl.activePortrait.transform.localPosition);

        Vector2 selectorPosition = pl.activePortrait.transform.localPosition;
        selectorPosition = selectorPosition + new Vector2(portraitCanvas.transform.localPosition.x, portraitCanvas.transform.localPosition.y);

        pl.selector.transform.localPosition = selectorPosition;
    }

    void HandleSelectorPreview(PlayerInterfaces pl)
    {
        if(pl.previewPortrait != pl.activePortrait)
        {
            if(pl.createdCharacter !=null)
            {
                Destroy(pl.createdCharacter);
            }

            GameObject go = Instantiate(CharacterManager.GetInstance().returnCharacterWithID(pl.activePortrait.characterId).prefab, pl.charVisPos.position, Quaternion.identity) as GameObject;
            pl.createdCharacter = go;
            pl.previewPortrait = pl.activePortrait;

            if(string.Equals(pl.playerBase.playerId, charManager.players[0].playerId))
            {
                //pl.createdCharacter.GetComponent<StateManager>().lookRight = false;
            }
        }
    }
}


[System.Serializable]
public class PlayerInterfaces
{
    public PortraitInfo activePortrait;
    public PortraitInfo previewPortrait;
    public GameObject selector;
    public Transform charVisPos;
    public GameObject createdCharacter;

    public int ActiveX;
    public int ActiveY;

    public bool hitInputOnce;
    public float timerToReset;

    public PlayerBase playerBase;

}