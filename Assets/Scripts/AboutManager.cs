using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AboutManager : MonoBehaviour {

    public Button backButton;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        backButton.onClick.AddListener(()=>Application.LoadLevel("Menu"));
	}
}
