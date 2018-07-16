using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class introSceneManager : MonoBehaviour {

    public GameObject startText;
    float timer;
    bool loadingLevel;
    bool init;

    public int activeElement;
    public GameObject menuObj;
    public ButtonRef[] menuOptioins;

    public string SelectScene;

    public CharacterManager charManager;

	// Use this for initialization
	void Start () {
        menuObj.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if(!init)
            {
            //flicker the 'start' text
            timer += Time.deltaTime;
            if(timer>0.6f)
            {
                timer = 0;
                startText.SetActive(!startText.activeInHierarchy);
            }

            //fix this to use input axis
            if(Input.GetKeyUp(KeyCode.Space) || Input.GetButtonUp("Block") || Input.GetButtonUp("Block1"))
            {
                init = true;
                startText.SetActive(false);
                menuObj.SetActive(true);
            }
        }
	}

    public void CharacterSelect(int PlayerNumbers)
    {

        charManager.numberOfUsers = PlayerNumbers;

        LoadLevel(SelectScene);
        
    }

    void LoadLevel(string SelectScene)
    {
        SceneManager.LoadSceneAsync(SelectScene, LoadSceneMode.Single);
    }




}
