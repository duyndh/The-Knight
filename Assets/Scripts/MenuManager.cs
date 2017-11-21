using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MenuManager : MonoBehaviour
{

    public Button startButton;
    public Button aboutButton;
    public Button exitButton;

    // Use this for initialization
    void Start()
    {
    }

    void StartOnClick()
    {
        Application.LoadLevel("Level");
    }

    void AboutOnClick()
    {
        Application.LoadLevel("About");
    }

    void ExitOnClick()
    {
        Application.Quit();
    }

    // Update is called once per frame
    void Update()
    {
        startButton.onClick.AddListener(StartOnClick);
        aboutButton.onClick.AddListener(AboutOnClick);
        exitButton.onClick.AddListener(ExitOnClick);
    }
}