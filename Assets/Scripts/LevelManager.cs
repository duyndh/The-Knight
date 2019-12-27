using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{

    public Button backButton;

    public Button[] levels;

    public static int levelsLength = 0;
    public static int level = 10;    

    // Use this for initialization
    void Start()
    {
        levelsLength = levels.Length;
		levels[0].onClick.AddListener(() => Load1());
		levels[1].onClick.AddListener(() => Load2());
		levels[2].onClick.AddListener(() => Load3());
		levels[3].onClick.AddListener(() => Load4());
		levels[4].onClick.AddListener(() => Load5());
		levels[5].onClick.AddListener(() => Load6());
		levels[6].onClick.AddListener(() => Load7());
		levels[7].onClick.AddListener(() => Load8());
		levels[8].onClick.AddListener(() => Load9());
		levels[9].onClick.AddListener(() => Load10());

		backButton.onClick.AddListener(() => Application.LoadLevel("Menu"));
    }

    void Load1()
    {
        level = 1;
        Application.LoadLevel("PlayScene");
    }
    void Load2()
    {
        level = 2;
        Application.LoadLevel("PlayScene");
    }
    void Load3()
    {
        level = 3;
        Application.LoadLevel("PlayScene");
    }
    void Load4()
    {
        level = 4;
        Application.LoadLevel("PlayScene");
    }
    void Load5()
    {
        level = 5;
        Application.LoadLevel("PlayScene");
    }
    void Load6()
    {
        level = 6;
        Application.LoadLevel("PlayScene");
    }
    void Load7()
    {
        level = 7;
        Application.LoadLevel("PlayScene");
    }
    void Load8()
    {
        level = 8;
        Application.LoadLevel("PlayScene");
    }
    void Load9()
    {
        level = 9;
        Application.LoadLevel("PlayScene");
    }
    void Load10()
    {
        level = 10;
        Application.LoadLevel("PlayScene");
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}