using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;


public class MapSpawner : MonoBehaviour
{
    public GameObject playerPrefabs;
    public GameObject exitDoorPrefabs;

    public Text playerPowerText;
    public Text gameResultText;
    public Button restartButton;
    public Button menuButton;
    public Button nextLevelButton;

    public GameObject[] tilePrefabs;

    public GameObject wall_U;
    public GameObject wall_D;
    public GameObject wall_L;
    public GameObject wall_R;

    public GameObject wall_UR;
    public GameObject wall_DR;
    public GameObject wall_DL;
    public GameObject wall_UL;


    private int nRow;
    private int nColumn;

    private bool isWait;

    private GameObject player;

    private float spacing;

    private GameObject[,] map;

    private int[] allowedKinds = new int[] { 3, 5, 6, 7, 9, 10, 11, 12, 13, 14 };

    private int[] nodes; // decimal form, binary form like U-R-D-L (up - right - down -left) 
                         // 1: allowed, 0: not allowed

    private int playerPower;

    private int source, target;
    //private int[] specialsPosition;
    //private int[] specialsValue;

    private List<int>[] adjacencyList;
    private int[,] adjacencyMatrix;

    private int[] distance;
    private int[] trace;

    public int[,] GetAdjacencyMatrix()
    {
        return adjacencyMatrix;
    }

    public void DecreasePower(int x)
    {
        playerPower -= x;
        playerPowerText.text = playerPower.ToString();
    }

    public int GetPlayerPower()
    {
        return playerPower;
    }

    public float GetSpacing()
    {
        return spacing;
    }

    public int GetSource()
    {
        return source;
    }

    public int GetTarget()
    {
        return target;
    }

    public int[] GetNodes()
    {
        return nodes;
    }

    public int ToIndex(int row, int column)
    {
        return row * (nColumn + 2) + column;
    }

    public int ToIndex(int index)
    {
        return ToIndex(ToRow0(index) + 1, ToColumn0(index) + 1);
    }

    public int ToRow(int index)
    {
        return index / (nColumn + 2);
    }

    public int ToColumn(int index)
    {
        return index % (nColumn + 2);
    }

    public int GetUp(int k)
    {
        return (k >> 3) & 1;
    }

    public int GetDown(int k)
    {
        return (k >> 1) & 1;
    }

    public int GetLeft(int k)
    {
        return k & 1;
    }

    public int GetRight(int k)
    {
        return (k >> 2) & 1;
    }

    public bool CheckTileUp(int kind, int index)
    {
        int up = nodes[index - (nColumn + 2)];
        return (GetDown(up) == GetUp(kind));
    }

    public bool CheckTileDown(int kind, int index)
    {
        int down = nodes[index + (nColumn + 2)];
        return (GetUp(down) == GetDown(kind));
    }

    public bool CheckTileLeft(int kind, int index)
    {
        int left = nodes[index - 1];
        return (GetRight(left) == GetLeft(kind));
    }

    public bool CheckTileRight(int kind, int index)
    {
        int right = nodes[index + 1];
        return (GetLeft(right) == GetRight(kind));
    }


    public Vector3 Coordinate(int row, int column, Vector3 delta)
    {
        return delta + new Vector3(column, -row, 0)
                   - 0.5f * new Vector3((nColumn + 1) / 2 + (nColumn / 2 + 1),
                        -(nRow + 1) / 2 - (nColumn / 2 + 1), 0);
    }

    GameObject SpawnTile(GameObject prefab, int row, int column, Vector3 delta)
    {
        GameObject tile = (GameObject)Instantiate(prefab,
                    transform.position + spacing * Coordinate(row, column, delta), prefab.transform.rotation);
        tile.transform.localScale = spacing * 6.25f * Vector3.one;
        return tile;
    }

    void DepthFirstSearch(int i, ref bool[] free)
    {
        free[i] = false;

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
        if (index > ToIndex(nRow, nColumn))
        {
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
    void SaveMap(string fileName)
    {
        StreamWriter sw = File.CreateText(fileName);
        sw.Write(nRow + " " + nColumn + " ");

        sw.Write(distance[target]);

        sw.Write(ToIndex(nRow / 2, nColumn / 2) + " " + ToIndex(nRow, nColumn) + " ");

        //for (int i = 0; i < (int)Mathf.Sqrt(nodes.Length); i++)
        //    sw.Write(Random.Range(1, ToIndex(nRow, nColumn) + 1) + " " + Random.Range(-(int)Mathf.Sqrt(nodes.Length), -1));

        for (int row = 0; row < nRow; row++)
            for (int column = 0; column < nColumn; column++)
                sw.Write(nodes[ToIndex(ToIndex0(row, column))] + " ");

        //for (int i = 0; i < nodes.Length; i++)
        //    sw.Write(nodes[i] + " ");
        sw.WriteLine();
        sw.Close();
    }

    void LoadMap(string fileName, int level)
    {
        //Debug.Log(level);
        StreamReader sr = File.OpenText(fileName);
        string t = "";
        for (int i = 0; i < level; i++)
            t = sr.ReadLine();

        string[] s = t.Split(' ');

        nRow = int.Parse(s[0]);
        nColumn = int.Parse(s[1]);

        playerPower = int.Parse(s[2]);

        nodes = new int[(nRow + 2) * (nColumn + 2)];

        source = ToIndex0(int.Parse(s[3]));

        target = ToIndex0(int.Parse(s[4]));

        //int nSpecials = 0;// int.Parse(s[4]);
        //specialsPosition = new int[0];
        //specialsValue = new int[0];

        //specialsPosition = new int[nSpecials];
        //specialsValue = new int[nSpecials];

        //for (int i = 0; i < nSpecials; i++)
        //{
        //    specialsPosition[i] = ToIndex0(int.Parse(s[5 + i * 2]));
        //    specialsValue[i] = (-1) * int.Parse(s[6 + i * 2]);
        //}

        int k = 5;// + nSpecials * 2;
        for (int row = 1; row <= nRow; row++)
        {
            for (int column = 1; column <= nColumn; column++)
            {
                nodes[ToIndex(row, column)] = int.Parse(s[k++]);
            }
        }

        //for (int i = 0; i < nodes.Length; i++)
        //    Debug.Log(nodes[i]);

        sr.Close();

    }

    void GenerateMap(int _nRow, int _nColumn)
    {
        nRow = _nRow;
        nColumn = _nColumn;

        // add 2 rows and 2 columns of wall
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

        SaveMap("_map");
    }

    void ShowMap()
    {
        float height = Camera.main.orthographicSize * 2f;
        float width = height * Screen.width / Screen.height;
        spacing = Mathf.Min(height / (nRow + 2), width / (nColumn + 2));

        // Show player
        if (player)
            Destroy(player);

        player = SpawnTile(playerPrefabs, ToRow0(source) + 1, ToColumn0(source) + 1, Vector3.back);


        // Show exit door
        GameObject exitDoor = SpawnTile(exitDoorPrefabs, ToRow0(target) + 1, ToColumn0(target) + 1, 0.5f * Vector3.back);

        map = new GameObject[nRow + 2, nColumn + 2];

        // Maze Tiles
        for (int row = 1; row <= nRow; row++)
        {
            for (int column = 1; column <= nColumn; column++)
            {
                map[row, column] = SpawnTile(tilePrefabs[nodes[ToIndex(row, column)]], row, column, Vector3.zero);
            }
        }

        // LEFT and RIGHT walls
        for (int row = 1; row < nRow + 1; row++)
        {
            map[row, 0] = SpawnTile(wall_L, row, 0, Vector3.zero);
            map[row, nColumn + 1] = SpawnTile(wall_R, row, nColumn + 1, Vector3.zero);
        }

        // UP and DOWN walls
        for (int column = 1; column < nColumn + 1; column++)
        {
            map[0, column] = SpawnTile(wall_U, 0, column, Vector3.zero);
            map[nRow + 1, column] = SpawnTile(wall_D, nRow + 1, column, Vector3.zero);
        }

        // corner wall
        map[0, 0] = SpawnTile(wall_UL, 0, 0, Vector3.zero);
        map[0, nColumn + 1] = SpawnTile(wall_UR, 0, nColumn + 1, Vector3.zero);
        map[nRow + 1, 0] = SpawnTile(wall_DL, nRow + 1, 0, Vector3.zero);
        map[nRow + 1, nColumn + 1] = SpawnTile(wall_DR, nRow + 1, nColumn + 1, Vector3.zero);
    }

    public int ToIndex0(int row, int column)
    {
        return row * nColumn + column;
    }

    public int ToIndex0(int index)
    {
        return ToIndex0(ToRow(index) - 1, ToColumn(index) - 1);
    }

    public int ToRow0(int index)
    {
        return index / nColumn;
    }

    public int ToColumn0(int index)
    {
        return index % nColumn;
    }

    void BuildGraph()
    {
        adjacencyMatrix = new int[nRow * nColumn, nRow * nColumn];
        // init
        for (int i = 0; i < nRow * nColumn; i++)
        {
            for (int j = 0; j < nRow * nColumn; j++)
            {
                if (i == j)
                    adjacencyMatrix[i, j] = 0;
                else
                    adjacencyMatrix[i, j] = int.MaxValue;
            }
        }

        adjacencyList = new List<int>[nRow * nColumn];

        for (int i = 0; i < nRow * nColumn; i++)
        {
            adjacencyList[i] = new List<int>();
            //Debug.Log(i);

            if (ToRow0(i) > 0 && GetUp(nodes[ToIndex(i)]) == 1)
            {
                int up = ToIndex0(ToRow0(i) - 1, ToColumn0(i));
                adjacencyList[i].Add(up);
                adjacencyMatrix[i, up] = 1;
                //Debug.Log("U");
            }

            if (ToRow0(i) < nRow - 1 && GetDown(nodes[ToIndex(i)]) == 1)
            {
                int down = ToIndex0(ToRow0(i) + 1, ToColumn0(i));

                adjacencyList[i].Add(down);
                adjacencyMatrix[i, down] = 1;
                //Debug.Log("D");
            }

            if (ToColumn0(i) > 0 && GetLeft(nodes[ToIndex(i)]) == 1)
            {
                int left = ToIndex0(ToRow0(i), ToColumn0(i) - 1);
                adjacencyList[i].Add(left);
                adjacencyMatrix[i, left] = 1;
                //Debug.Log("L");
            }

            if (ToColumn0(i) < nColumn - 1 && GetRight(nodes[ToIndex(i)]) == 1)
            {
                int right = ToIndex0(ToRow0(i), ToColumn0(i) + 1);
                adjacencyList[i].Add(right);
                adjacencyMatrix[i, right] = 1;
                //Debug.Log("R");
            }
        }

        //for (int i = 0; i < specialsPosition.Length; i++)
        //{
        //    int u = specialsPosition[i];
        //    foreach (int v in adjacencyList[u])
        //    {
        //        adjacencyMatrix[v, u] = specialsValue[i];
        //    }
        //}

        //for (int i = 0; i < nRow * nColumn; i++)
        //{
        //    foreach (int x in adjacencyList[i])
        //    {
        //        Debug.Log(i + " " + x + " " + adjacencyMatrix[i, x]);
        //    }
        //}

    }

    // Dijkstra Algorithm
    void FindShortestPath()
    {
        int numberNodes = nRow * nColumn;

        bool[] free = new bool[numberNodes];

        distance = new int[numberNodes];
        trace = new int[numberNodes];

        // init
        for (int i = 0; i < numberNodes; i++)
        {
            distance[i] = int.MaxValue;
            free[i] = true;
        }
        distance[source] = 0;

        while (true)
        {
            // find node u in free nodes which has minimum distance
            int u = 0;
            int dmin = int.MaxValue;
            for (int i = 0; i < numberNodes; i++)
                if (free[i] && distance[i] < dmin)
                {
                    dmin = distance[i];
                    u = i;
                }

            // found shortest path from source to targer -> break
            if (u == target)
                break;

            free[u] = false;

            // use node u to optimize nodes v which neighbour with u
            foreach (int v in adjacencyList[u])
                if (free[v] && distance[u] + adjacencyMatrix[u, v] < distance[v])
                {
                    distance[v] = distance[u] + adjacencyMatrix[u, v];
                    trace[v] = u;
                }
        }
    }

    // Use this for initialization
    void Start()
    {
        //GenerateMap(10, 10);

        LoadMap("map", LevelManager.level);

        BuildGraph();

        isWait = false;

        //FindShortestPath();

        //playerPower = distance[target];
        playerPowerText.text = playerPower.ToString();
        gameResultText.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        menuButton.gameObject.SetActive(false);
        nextLevelButton.gameObject.SetActive(false);

        ShowMap();
    }

    void NextLevel()
    {
        isWait = true;
        LevelManager.level++;
        Debug.Log(LevelManager.level);
        Application.LoadLevel("PlayScene");
    }

    // Update is called once per frame
    void Update()
    {
        if (playerPower <= 0)
        {
            //Debug.Log(player);
            player.GetComponent<PlayerController>().SetGamePause(true);
            gameResultText.gameObject.SetActive(true);
            restartButton.gameObject.SetActive(true);
            menuButton.gameObject.SetActive(true);

            //if (LevelManager.level < LevelManager.levelsLength)
                nextLevelButton.gameObject.SetActive(true);

            if (player.GetComponent<PlayerController>().GetPosition() == target)
                gameResultText.text = "YOU WIN";
            else
                gameResultText.text = "GAME OVER";
        }

        if (!isWait)
        {
            nextLevelButton.onClick.AddListener(NextLevel);
            restartButton.onClick.AddListener(() => Application.LoadLevel("PlayScene"));
            menuButton.onClick.AddListener(() => Application.LoadLevel("Menu"));
        }
    }
}