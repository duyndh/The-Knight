using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSpawner : MonoBehaviour {

    public int nRow;
    public int nColumn;

    public GameObject[] tilePrefabs;

    public GameObject wall_U;
    public GameObject wall_D;
    public GameObject wall_L;
    public GameObject wall_R;

    public GameObject wall_UR;
    public GameObject wall_DR;
    public GameObject wall_DL;
    public GameObject wall_UL;


    private float spacing;
    private GameObject[,] map;

    private int[] allowedKinds = new int[] { 3, 5, 6, 7, 9, 10, 11, 12, 13, 14 };

    private int[] nodes; // decimal form, binary form like U-R-D-L (up - right - down -left) 
                         // 1: allowed, 0: not allowed

    
    int ToIndex(int row, int column)
    {
        return row * (nColumn + 2) + column;
    }

    int ToRow(int index)
    {
        return index / (nColumn + 2);
    }

    int ToColumn(int index)
    {
        return index % (nColumn + 2);
    }

    int GetUp(int k)
    {
        return (k >> 3) & 1;
    }

    int GetDown(int k)
    {
        return (k >> 1) & 1;
    }

    int GetLeft(int k)
    {
        return k & 1;
    }

    int GetRight(int k)
    {
        return (k >> 2) & 1;
    }

    bool CheckTileUp(int kind, int index)
    {
        int up = nodes[index - (nColumn + 2)];
        return (GetDown(up) == GetUp(kind)); 
    }

    bool CheckTileDown(int kind, int index)
    {
        int down = nodes[index + (nColumn + 2)];
        return (GetUp(down) == GetDown(kind));
    }

    bool CheckTileLeft(int kind, int index)
    {
        int left = nodes[index - 1];
        return (GetRight(left) == GetLeft(kind));
    }

    bool CheckTileRight(int kind, int index)
    {
        int right = nodes[index + 1];
        return (GetLeft(right) == GetRight(kind));
    }


    Vector3 Coordinate(int row, int column)
    {
        return new Vector3(column, -row, 0)
                   - 0.5f * new Vector3((nColumn + 1) / 2 + (nColumn / 2 + 1),
                        -(nRow + 1) / 2 - (nColumn / 2 + 1), 0);
    }

    GameObject SpawnTile(GameObject prefab, int row, int column)
    {
        GameObject tile = (GameObject)Instantiate(prefab,
                    transform.position + spacing * Coordinate(row, column), prefab.transform.rotation);
        tile.transform.localScale = spacing * 6.25f * Vector3.one;
        return tile;
    }

    void DepthFirstSearch(int i, ref bool[] free)
    {
        free[i] = false;
        //Debug.Log(i);

        int up = ToIndex(ToRow(i) - 1, ToColumn(i));
        if ((GetUp(nodes[i]) == 1) && free[up])
            DepthFirstSearch(up, ref free);

        int down = ToIndex(ToRow(i) + 1, ToColumn(i));
        if ((GetDown(nodes[i]) == 1) && free[down])
            DepthFirstSearch(down, ref free);

        int left = ToIndex(ToRow(i), ToColumn(i) - 1);
        if ((GetLeft(nodes[i]) == 1) && free[left])
            DepthFirstSearch(left, ref free);

        int right = ToIndex(ToRow(i), ToColumn(i) + 1);
        if ((GetRight(nodes[i]) == 1) && free[right])
            DepthFirstSearch(right, ref free);

    }


    bool CheckInterConnectedGraph()
    {
        bool[] free = new bool[(nRow + 2) * (nColumn + 2)];
        
        // init all node is free, except walls
        for (int row = 0; row <= nRow + 1; row++)
            for (int column = 0; column <= nColumn + 1; column++)
            {
                // walls
                if (row == 0 || row == nRow + 1 || column == 0 || column == nColumn + 1)
                    free[ToIndex(row, column)] = false;
                else
                // tiles 
                    free[ToIndex(row, column)] = true;
            }
        
        DepthFirstSearch(ToIndex(1, 1), ref free);

        for (int i = 0; i < free.Length; i++)
            if (free[i])
                return false;

        return true;
    }

    void Shuffle(int[] array)
    {
        int length = array.Length;
        for (int i = 0; i < length; i++)
        {
            int j = Random.Range(0, length);
            
            // swap
            int t = array[i];
            array[i] = array[j];
            array[j] = t;
        }
    }

    void BackTracking(int index, ref bool valid)
    {
        //Debug.Log(index);
        if (index > ToIndex(nRow, nColumn))
        {
            //Debug.Log("OK");
            valid = CheckInterConnectedGraph();
        }
        else
        {
            Shuffle(allowedKinds);

            foreach (int kind in allowedKinds)
            {
                if (!CheckTileUp(kind, index) || !CheckTileLeft(kind, index))
                    continue;

                if (ToColumn(index) == nColumn && !CheckTileRight(kind, index))
                    continue;
                if (ToRow(index) == nRow && !CheckTileDown(kind, index))
                    continue;


                nodes[index] = kind;

                if (ToColumn(index) == nColumn)
                    BackTracking(index + 3, ref valid);
                else
                    BackTracking(index + 1, ref valid);

                if (valid)
                    break;
            }
        }
    }

    void GenerateGraph()
    {
        // add 2 row and 2 column of wall
        nodes = new int[(nRow + 2) * (nColumn + 2)];

        // init node kind of walls is 0 (not allowed move to)
        for (int row = 0; row <= nRow + 1; row++)
        {
            nodes[ToIndex(row, 0)] = 0; // 0000
            nodes[ToIndex(row, nColumn + 1)] = 0; // 0000
        }
        for (int column = 0; column <= nColumn + 1; column++)
        {
            nodes[ToIndex(0, column)] = 0; // 0000
            nodes[ToIndex(nRow + 1, column)] = 0; // 0000
        }

        bool valid = false;

        BackTracking(ToIndex(1, 1), ref valid);
    }

    // Use this for initialization
	void Start () {
        
        GenerateGraph();

        float height = Camera.main.orthographicSize * 2f;
        float width = height * Screen.width / Screen.height;
        spacing = Mathf.Min(height / (nRow + 2), width / (nColumn + 2));

        map = new GameObject[nRow + 2, nColumn + 2];

        // Maze Tiles
        for (int row = 1; row <= nRow; row++)
        {
            for (int column = 1; column <= nColumn; column++)
            {
                map[row, column] = SpawnTile(tilePrefabs[nodes[ToIndex(row, column)]], row, column);
            }
        }

        // LEFT and RIGHT walls
        for (int row = 1; row < nRow + 1; row++)
        {
            map[row, 0] = SpawnTile(wall_L, row, 0);
            map[row, nColumn + 1] = SpawnTile(wall_R, row, nColumn + 1);
        }

        // UP and DOWN walls
        for (int column = 1; column < nColumn + 1; column++)
        {
            map[0, column] = SpawnTile(wall_U, 0, column);
            map[nRow + 1, column] = SpawnTile(wall_D, nRow + 1, column);
        }

        // UP-LEFT corner
        map[0, 0] = SpawnTile(wall_UL, 0, 0);
        
        // UP-RIGHT corner 
        map[0, nColumn + 1] = SpawnTile(wall_UR, 0, nColumn + 1);

        // DOWN-LEFT corner
        map[nRow + 1, 0] = SpawnTile(wall_DL, nRow + 1, 0);

        //DOWN - RIGHT corner
        map[nRow + 1, nColumn + 1] = SpawnTile(wall_DR, nRow + 1, nColumn + 1);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
