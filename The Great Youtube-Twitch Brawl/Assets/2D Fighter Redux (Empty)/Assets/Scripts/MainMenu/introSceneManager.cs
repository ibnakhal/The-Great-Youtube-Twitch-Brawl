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

	// Use this for initialization
	void Start () {
        menuObj.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if(!init)
            {
            timer += Time.deltaTime;
            if(timer>0.6f)
            {
                timer = 0;
                startText.SetActive(!startText.activeInHierarchy);
            }

            if()

        }
	}
}
