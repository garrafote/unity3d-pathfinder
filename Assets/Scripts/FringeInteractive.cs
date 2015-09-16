using UnityEngine;
using System.Collections;
using PathFinder.Grid2D;
using PathFinder;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using Assets.Profile;
using Assets.Scripts;

public class FringeInteractive : MonoBehaviour
{
    public enum Engine
    {
        Fringe,
        AStar,
    }


    public Sprite tileSprite;
    public Vector2 gridSize = new Vector2(50, 25);
    public Rect gridRect = new Rect(0, 0, 1, 1);
    public Engine searchEngine;

    Grid2D grid;
    SpriteRenderer[,] tiles;

    IPathFinder pathFinder;

    Node2D start;
    Node2D end;

    private int screenWidth;
    private int screenHeight;

    const float DefaultWeight = 1;
    const float GrassWeight = 2;
    const float WaterWeight = 4;
    const float WallWeight = 0x0fff;

    Regex nameRegex = new Regex(@"(?<i>\d+),(?<j>\d+)");

    void Awake()
    {
        tiles = new SpriteRenderer[(int)gridSize.x, (int)gridSize.y];
        grid = new Grid2D((int)gridSize.x, (int)gridSize.y);

        switch (searchEngine)
        {
            case Engine.Fringe:
                pathFinder = new Fringe(Grid2D.HeuristicManhattan2);
                break;
            case Engine.AStar:
                pathFinder = new AStar(Grid2D.HeuristicManhattan2);
                break;
        }

    }


    void Start()
    {
        CreateSpriteGrid();
        UpdateSpriteGrid();

        screenHeight = Screen.height;
        screenWidth = Screen.width;
    }

    Node2D nodeToCopy;
    IList<Node2D> dirty = new List<Node2D>();
    private IEnumerable<INode> path;

    void Update()
    {
        if (screenHeight != Screen.height || screenWidth != Screen.width)
        {
            UpdateSpriteGrid();

            screenHeight = Screen.height;
            screenWidth = Screen.width;
        }

        dirty.Clear();

        RaycastHit hitInfo;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
        {
            var match = nameRegex.Match(hitInfo.transform.name);

            var i = System.Convert.ToInt32(match.Groups["i"].Value);
            var j = System.Convert.ToInt32(match.Groups["j"].Value);

            var hoveredNode = grid[i, j];

            if (Input.GetMouseButtonDown(0))
            {
                nodeToCopy = null;

                if (hoveredNode == start)
                {
                    if (path != null)
                    {
                        foreach (Node2D node in path)
                        {
                            dirty.Add(node);
                        }
                    }
                    else
                    {
                        dirty.Add(start);
                    }
                    start = null;
                    path = null;
                }
                else if (hoveredNode == end)
                {
                    if (path != null)
                    {
                        foreach (Node2D node in path)
                        {
                            dirty.Add(node);
                        }
                    }
                    else
                    {
                        dirty.Add(end);
                    }
                    end = null;
                    path = null;
                }
                else if (start == null)
                {
                    start = hoveredNode;
                    dirty.Add(hoveredNode);
                }
                else if (end == null)
                {
                    end = hoveredNode;
                    dirty.Add(hoveredNode);
                }
                else
                {
                    nodeToCopy = hoveredNode;
                }
            }
            else if (Input.GetMouseButton(0))
            {
                if (nodeToCopy != null && hoveredNode.Weight != nodeToCopy.Weight)
                {
                    hoveredNode.Weight = nodeToCopy.Weight;
                    dirty.Add(hoveredNode);
                }

            }
            else if (Input.GetMouseButtonUp(0) && hoveredNode == nodeToCopy)
            {
                if (hoveredNode.Weight == WallWeight)
                {
                    hoveredNode.Weight = WaterWeight;
                }
                else if (hoveredNode.Weight == WaterWeight)
                {
                    hoveredNode.Weight = GrassWeight;
                }
                else if (hoveredNode.Weight == GrassWeight)
                {
                    hoveredNode.Weight = DefaultWeight;
                }
                else
                {
                    hoveredNode.Weight = WallWeight;
                }

                dirty.Add(hoveredNode);
            }
        }

        // repaint

        if (dirty.Count > 0)
        {

            if (path != null)
            {
                foreach (Node2D node in path)
                {
                    tiles[node.X, node.Y].color = GetNodeColor(node);
                }
            }

            foreach (var dirtyNode in dirty)
            {
                tiles[dirtyNode.X, dirtyNode.Y].color = GetNodeColor(dirtyNode);
            }

            if (start != null && end != null)
            {
                Chrono.Run(searchEngine.ToString(), () => {
                    path = pathFinder.FindPath(start, end);
                });

                if (searchEngine == Engine.Fringe)
                    ResearchData.FringeMem = path.Count();
                else
                    ResearchData.AStarMem = path.Count();

                Debug.LogFormat("{0} - Path Size: {1}, Path Cost: {2}, Visited Nodes {3}", 
                    searchEngine, 
                    path.Count(), 
                    pathFinder.PathCost,
                    pathFinder.VisitedNodes);

                foreach (Node2D node in path)
                {
                    if (node == start || node == end)
                    {
                        continue;
                    }

                    tiles[node.X, node.Y].color = Color.magenta + Color.white * .5f;
                }
            }
        }
        // repaint path
    }


    private void CreateSpriteGrid()
    {
        for (int i = 0,leni = (int)gridSize.x; i < leni; i++)
        {
            for (int j = 0, lenj = (int)gridSize.y; j < lenj; j++)
            {
                var obj = new GameObject(string.Format("Node [{0},{1}]", i, j));
                obj.transform.SetParent(transform);

                var tile = obj.AddComponent<SpriteRenderer>();
                tile.sprite = tileSprite;

                obj.AddComponent<BoxCollider>();

                tiles[i, j] = tile;
            }
        }
    }


    private void UpdateSpriteGrid()
    {
        var cam = Camera.main;

        var camSize = new Vector2(cam.orthographicSize * Screen.width / Screen.height, cam.orthographicSize) * 2;
        var tileSize = Vector2.Scale(camSize, new Vector2(1 / gridSize.x, 1 / gridSize.y));

        var pos = new Vector3 { z = 10 };
        for (int i = 0, leni = (int)gridSize.x; i < leni; i++)
        {
            pos.x = ((i + .5f) * gridRect.width / gridSize.x) + gridRect.x;

            for (int j = 0, lenj = (int)gridSize.y; j < lenj; j++)
            {
                pos.y = ((j + .5f) * gridRect.height / gridSize.y) + gridRect.y;

                var tile = tiles[i, j];
                tile.transform.localPosition = cam.ViewportToWorldPoint(pos);
                tile.transform.localScale = new Vector3
                {
                    x = tileSize.x * gridRect.width / tile.bounds.size.x,
                    y = tileSize.y * gridRect.height / tile.bounds.size.y,
                    z = 1
                };
            }
        }
    }

    public Color GetNodeColor(Node2D node)
    {
        if (node == start)
        {
            return Color.white * .5f + Color.red;
        }
        if (node == end)
        {
            return Color.white * .2f + Color.yellow;
        }

        if (node.Weight == WallWeight)
        {
            return Color.black;
        }

        if (node.Weight == GrassWeight)
        {
            return Color.white * .5f + Color.green;
        }

        if (node.Weight == WaterWeight)
        {
            return Color.white * .5f + Color.blue;
        }

        return Color.white;
    }

    public IEnumerator WaitKeyDown(KeyCode key)
    {
        do
        {
            yield return null;
        }
        while (key != KeyCode.None && !Input.GetKey(key));

        Debug.LogFormat("Key {0} pressed", key);
    }
}
