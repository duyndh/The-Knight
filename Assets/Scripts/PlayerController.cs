using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    private GameObject mapSpawner;
    private MapSpawner mapSpawnerScript;
    private float spacing;
    private int[] nodes;

    private int position, exit;

	// Use this for initialization
	void Start () {
        mapSpawner = GameObject.Find("MapSpawner");
        mapSpawnerScript = mapSpawner.GetComponent<MapSpawner>();

        spacing = mapSpawnerScript.GetSpacing();
        nodes = mapSpawnerScript.GetNodes();

        // start position
        position = mapSpawnerScript.ToIndex(mapSpawnerScript.GetPlayerStartPosition());
        //Debug.Log(position);

        // exit door position
        exit = mapSpawnerScript.ToIndex(mapSpawnerScript.GetExitDoorPosition());
        
	}
	
    void UpdatePosition()
    {
        gameObject.transform.position = mapSpawner.transform.position
            + spacing * mapSpawnerScript.Coordinate(mapSpawnerScript.ToRow(position), mapSpawnerScript.ToColumn(position), Vector3.back);
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.UpArrow) 
            && (mapSpawnerScript.GetUp(nodes[position]) == 1))
        {
            //Debug.Log("Up");
            position = mapSpawnerScript.ToIndex(mapSpawnerScript.ToRow(position) - 1, mapSpawnerScript.ToColumn(position));
            //Debug.Log(position);
            UpdatePosition();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) 
            && (mapSpawnerScript.GetDown(nodes[position]) == 1))
        {
            //Debug.Log("Down");
            position = mapSpawnerScript.ToIndex(mapSpawnerScript.ToRow(position) + 1, mapSpawnerScript.ToColumn(position));
            //Debug.Log(position);
            UpdatePosition();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) 
            && (mapSpawnerScript.GetLeft(nodes[position]) == 1))
        {
            //Debug.Log("Left");
            position = mapSpawnerScript.ToIndex(mapSpawnerScript.ToRow(position), mapSpawnerScript.ToColumn(position) - 1);
            //Debug.Log(position);
            UpdatePosition();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) 
            && (mapSpawnerScript.GetRight(nodes[position]) == 1))
        {
            //Debug.Log("Right");
            position = mapSpawnerScript.ToIndex(mapSpawnerScript.ToRow(position), mapSpawnerScript.ToColumn(position) + 1);
            //Debug.Log(position);
            UpdatePosition();
        }

        //if (
    }
}
