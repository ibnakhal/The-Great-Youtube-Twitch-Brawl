using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour {
    public int numberOfUsers;
    public List<PlayerBase> players = new List<PlayerBase>();
    public List<CharacterBase> characterList = new List<CharacterBase>();

    public CharacterBase returnCharacterWithID(string id)
    {
        CharacterBase returnValue = null;

        for (int x = 0; x < characterList.Count; x++)
        {
            if(string.Equals(characterList[x].charId,id))
            {
                returnValue = characterList[x];
                break;
            }

        }

        return returnValue;
    }

    public PlayerBase returnPlayerFromStates(StateManager states)
    {
        PlayerBase ReturnValue = null;

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].playerStates == states) ;
            {
                ReturnValue = players[i];
                break;
            }
        }

        return ReturnValue;
    }

    public static CharacterManager instance;
    public static CharacterManager GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

	
}

[System.Serializable]
public class PlayerBase
{
    public string playerId;
    public string inputId;
    public PlayerType playerType;
    public bool hasCharacter;
    public GameObject playerPrefab;
    public StateManager playerStates;
    public int score;
    public enum PlayerType
    {
        user,
        ai,
        sim,
    }
}

[System.Serializable]
public class CharacterBase
{
    public string charId;
    public GameObject prefab;
    public Sprite image;
    public AudioClip selectionClip;
}