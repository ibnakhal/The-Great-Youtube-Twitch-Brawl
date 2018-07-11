using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuImageRandomizer : MonoBehaviour {
    public List<Sprite> images;

	// Use this for initialization
	void Start () {
        int rando = Random.RandomRange(0, images.Count);
        this.GetComponent<Image>().sprite = images[rando];
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
