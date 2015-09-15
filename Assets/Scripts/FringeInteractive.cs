using UnityEngine;
using System.Collections;
using PathFinder.Grid2D;
using PathFinder;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class FringeInteractive : MonoBehaviour {

    public Sprite tileSprite;

    Grid2D grid;
    SpriteRenderer[,] tiles;

    Fringe fringe;

    int gridSize = 50;

    Node2D start;
    Node2D end;

    const float DefaultWeight = 1;
    const float GrassWeight = 2;
    const float WaterWeight = 4;

    Regex nameRegex = new Regex(@"(?<i>\d+),(?<j>\d+)");

    void Awake()
    {
        tiles = new SpriteRenderer[gridSize, gridSize];
        grid = new Grid2D(gridSize, gridSize);
        fringe = new Fringe(Grid2D.HeuristicManhattan2);
    }


    void Start()
    {
        CreateSpriteGrid();
        UpdateSpriteGrid();

        start = grid[2, 2];
        end = grid[48, 48];

        //StartCoroutine(fringe.FindInteractive(start, end, () =>
        //{
        //    foreach (Node2D node in fringe.cache.Keys)
        //        if (node != start && node != end)
        //            tiles[node.X, node.Y].color = Color.red + Color.white * .5f;

        //    foreach (Node2D node in fringe.fringe)
        //        if (node != start && node != end)
        //            tiles[node.X, node.Y].color = Color.green + Color.white * .5f;

        //    foreach (Node2D node in fringe.pathInteractive)
        //        if (node != start && node != end)
        //            tiles[node.X, node.Y].color = Color.magenta + Color.white * .5f;

        //    return StartCoroutine(WaitKeyDown(KeyCode.None));
        //}));

        path = fringe.FindPath(start, end);

        foreach (Node2D node in path)
        {
            tiles[node.X, node.Y].color = Color.magenta + Color.white * .5f;
        }
        
    }

    Node2D nodeToCopy;
    IList<Node2D> dirty = new List<Node2D>();
    private IEnumerable<INode> path;

    void Update()
    {
        dirty.Clear();

        RaycastHit hitInfo;
        if (Physics.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.forward, out hitInfo))
        {
            var match = nameRegex.Match(hitInfo.transform.name);

            var i = System.Convert.ToInt32(match.Groups["i"].Value);
            var j = System.Convert.ToInt32(match.Groups["j"].Value);

            var hoveredNode = grid[i, j];

            if (Input.GetMouseButtonDown(0))
            {
                nodeToCopy = hoveredNode;
            }
            else if (Input.GetMouseButton(0))
            {
                if (hoveredNode.Weight != nodeToCopy.Weight)
                {
                    hoveredNode.Weight = nodeToCopy.Weight;
                    dirty.Add(hoveredNode);
                } 

            }
            else if (Input.GetMouseButtonUp(0) && hoveredNode == nodeToCopy)
            {
                if (hoveredNode.Weight == float.MaxValue)
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
                    hoveredNode.Weight = float.MaxValue;
                }

                dirty.Add(hoveredNode);
            } 
        }

        // repaint

        if (dirty.Count > 0)
        {

            foreach (Node2D node in path)
            {
                tiles[node.X, node.Y].color = GetNodeColor(node);
            }
            
            foreach (var dirtyNode in dirty)
            {
                tiles[dirtyNode.X, dirtyNode.Y].color = GetNodeColor(dirtyNode);
            }

            path = fringe.FindPath(start, end);

            foreach (Node2D node in path)
            {
                tiles[node.X, node.Y].color = Color.magenta + Color.white * .5f;
            }
        }

        // repaint path
        



    }


    private void CreateSpriteGrid()
    {
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
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
        var tileSize = camSize / gridSize;

        Vector3 pos = new Vector3();
        pos.z = 10;
        for (int i = 0; i < gridSize; i++)
        {
            pos.x = ((float)(i + .5f) / gridSize);

            for (int j = 0; j < gridSize; j++)
            {
                pos.y = ((float)(j + .5f) / gridSize);

                var tile = tiles[i, j];
                tile.transform.position = cam.ViewportToWorldPoint(pos);
                tile.transform.localScale = new Vector3(tileSize.x / tile.bounds.size.x, tileSize.y / tile.bounds.size.y, 1);
            }
        }
    }

    public Color GetNodeColor(Node2D node)
    {
        if (node.Weight == float.MaxValue)
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
