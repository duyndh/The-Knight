using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour {

    private GameObject mapSpawner;
    private MapSpawner mapSpawnerScript;
    private float spacing;
    private int[] nodes;

    private int[,] adjacencyMatrix;

    private int position, exit;
    
    // Use this for initialization
    void Start () {
        
        mapSpawner = GameObject.Find("MapSpawner");
        mapSpawnerScript = mapSpawner.GetComponent<MapSpawner>();

        spacing = mapSpawnerScript.GetSpacing();
        nodes = mapSpawnerScript.GetNodes();

        adjacencyMatrix = mapSpawnerScript.GetAdjacencyMatrix();

        // start position
        position = mapSpawnerScript.GetSource();
        //Debug.Log(position);

        // exit door position
        exit = mapSpawnerScript.GetTarget();
     
	}
	
    void UpdatePosition()
    {
        gameObject.transform.position = mapSpawner.transform.position
            + spacing * mapSpawnerScript.Coordinate(mapSpawnerScript.ToRow(mapSpawnerScript.ToIndex(position)), mapSpawnerScript.ToColumn(mapSpawnerScript.ToIndex(position)), Vector3.back);
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.UpArrow) 
            && (mapSpawnerScript.GetUp(nodes[mapSpawnerScript.ToIndex(position)]) == 1))
        {
            int up = mapSpawnerScript.ToIndex0(mapSpawnerScript.ToRow0(position) - 1, mapSpawnerScript.ToColumn0(position));// mapSpawnerScript.ToIndex(mapSpawnerScript.ToRow(position) - 1, mapSpawnerScript.ToColumn(position));
            Debug.Log(up);
            mapSpawnerScript.DecreasePower(adjacencyMatrix[position, up]);
            position = up;
            UpdatePosition();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) 
            && (mapSpawnerScript.GetDown(nodes[mapSpawnerScript.ToIndex(position)]) == 1))
        {
            int down = mapSpawnerScript.ToIndex0(mapSpawnerScript.ToRow0(position) + 1, mapSpawnerScript.ToColumn0(position));// mapSpawnerScript.ToIndex(mapSpawnerScript.ToRow(position) + 1, mapSpawnerScript.ToColumn(position));
            Debug.Log(down);
            mapSpawnerScript.DecreasePower(adjacencyMatrix[position, down]);
            position = down;
            UpdatePosition();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) 
            && (mapSpawnerScript.GetLeft(nodes[mapSpawnerScript.ToIndex(position)]) == 1))
        {
            int left = mapSpawnerScript.ToIndex0(mapSpawnerScript.ToRow0(position), mapSpawnerScript.ToColumn0(position) - 1);// mapSpawnerScript.ToIndex(mapSpawnerScript.ToRow(position), mapSpawnerScript.ToColumn(position) - 1);
            Debug.Log(left);
            mapSpawnerScript.DecreasePower(adjacencyMatrix[position, left]);
            position = left;
            UpdatePosition();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) 
            && (mapSpawnerScript.GetRight(nodes[mapSpawnerScript.ToIndex(position)]) == 1))
        {
            int right = mapSpawnerScript.ToIndex0(mapSpawnerScript.ToRow0(position), mapSpawnerScript.ToColumn0(position) + 1);// mapSpawnerScript.ToIndex(mapSpawnerScript.ToRow(position), mapSpawnerScript.ToColumn(position) + 1);
            Debug.Log(right);
            mapSpawnerScript.DecreasePower(adjacencyMatrix[position, right]);
            position = right;
            UpdatePosition();
        }

        
        //if (position == exit)
        //    Debug.Log("Yeah");
    }
}
