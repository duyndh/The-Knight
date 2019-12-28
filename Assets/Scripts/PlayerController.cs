using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private GameObject mapSpawner;
    private MapSpawner mapSpawnerScript;
    private float spacing;
    private int[] nodes;

    private int[,] adjacencyMatrix;

    private int position, exit;

    private bool gamePause;

    public int GetPosition()
    {
        return position;
    }

    public void SetGamePause(bool x)
    {
        gamePause = x;
    }

    // Use this for initialization
    void Start()
    {

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

        gamePause = false;
    }

    void UpdatePosition()
    {
        gameObject.transform.position = mapSpawner.transform.position
            + spacing * mapSpawnerScript.Coordinate(mapSpawnerScript.ToRow(mapSpawnerScript.ToIndex(position)), mapSpawnerScript.ToColumn(mapSpawnerScript.ToIndex(position)), Vector3.back);
    }

    // Update is called once per frame

    // If the touch is longer than MAX_SWIPE_TIME, we dont consider it a swipe
    public const float MAX_SWIPE_TIME = 0.5f;

    // Factor of the screen width that we consider a swipe
    // 0.17 works well for portrait mode 16:9 phone
    public const float MIN_SWIPE_DISTANCE = 0.17f;

    public static bool swipedRight = false;
    public static bool swipedLeft = false;
    public static bool swipedUp = false;
    public static bool swipedDown = false;

    Vector2 startPos;
    float startTime;


    void Update()
    {
        swipedRight = false;
        swipedLeft = false;
        swipedUp = false;
        swipedDown = false;

        if (Input.touches.Length > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {
                startPos = new Vector2(t.position.x / (float)Screen.width, t.position.y / (float)Screen.width);
                startTime = Time.time;
            }
            if (t.phase == TouchPhase.Ended)
            {
                if (Time.time - startTime > MAX_SWIPE_TIME) // press too long
                    return;

                Vector2 endPos = new Vector2(t.position.x / (float)Screen.width, t.position.y / (float)Screen.width);

                Vector2 swipe = new Vector2(endPos.x - startPos.x, endPos.y - startPos.y);

                if (swipe.magnitude < MIN_SWIPE_DISTANCE) // Too short swipe
                    return;

                if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y))
                { // Horizontal swipe
                    if (swipe.x > 0)
                    {
                        swipedRight = true;
                    }
                    else
                    {
                        swipedLeft = true;
                    }
                }
                else
                { // Vertical swipe
                    if (swipe.y > 0)
                    {
                        swipedUp = true;
                    }
                    else
                    {
                        swipedDown = true;
                    }
                }
            }
        }

        if (!gamePause)
        {
            if ((Input.GetKeyDown(KeyCode.UpArrow) == true || swipedUp == true)
      && (mapSpawnerScript.GetUp(nodes[mapSpawnerScript.ToIndex(position)]) == 1))
            {
                int up = mapSpawnerScript.ToIndex0(mapSpawnerScript.ToRow0(position) - 1, mapSpawnerScript.ToColumn0(position));// mapSpawnerScript.ToIndex(mapSpawnerScript.ToRow(position) - 1, mapSpawnerScript.ToColumn(position));
                //Debug.Log(up);
                mapSpawnerScript.DecreasePower(adjacencyMatrix[position, up]);
                position = up;
                UpdatePosition();
            }

            if ((Input.GetKeyDown(KeyCode.DownArrow) == true || swipedDown == true)
                && (mapSpawnerScript.GetDown(nodes[mapSpawnerScript.ToIndex(position)]) == 1))
            {
                int down = mapSpawnerScript.ToIndex0(mapSpawnerScript.ToRow0(position) + 1, mapSpawnerScript.ToColumn0(position));// mapSpawnerScript.ToIndex(mapSpawnerScript.ToRow(position) + 1, mapSpawnerScript.ToColumn(position));
                //Debug.Log(down);
                mapSpawnerScript.DecreasePower(adjacencyMatrix[position, down]);
                position = down;
                UpdatePosition();
            }

            if ((Input.GetKeyDown(KeyCode.LeftArrow) == true || swipedLeft == true)
                && (mapSpawnerScript.GetLeft(nodes[mapSpawnerScript.ToIndex(position)]) == 1))
            {
                int left = mapSpawnerScript.ToIndex0(mapSpawnerScript.ToRow0(position), mapSpawnerScript.ToColumn0(position) - 1);// mapSpawnerScript.ToIndex(mapSpawnerScript.ToRow(position), mapSpawnerScript.ToColumn(position) - 1);
                //Debug.Log(left);
                mapSpawnerScript.DecreasePower(adjacencyMatrix[position, left]);
                position = left;
                UpdatePosition();
            }

            if ((Input.GetKeyDown(KeyCode.RightArrow) == true || swipedRight == true)
                && (mapSpawnerScript.GetRight(nodes[mapSpawnerScript.ToIndex(position)]) == 1))
            {
                int right = mapSpawnerScript.ToIndex0(mapSpawnerScript.ToRow0(position), mapSpawnerScript.ToColumn0(position) + 1);// mapSpawnerScript.ToIndex(mapSpawnerScript.ToRow(position), mapSpawnerScript.ToColumn(position) + 1);
                //Debug.Log(right);
                mapSpawnerScript.DecreasePower(adjacencyMatrix[position, right]);
                position = right;
                UpdatePosition();
            }
        }
    }
}