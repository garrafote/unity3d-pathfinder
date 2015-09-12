using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class TestScript : MonoBehaviour {
    #region Search API

    interface INode
    {
        float Weight { get; }
        IEnumerable<INode> Connections { get; }

    }

    interface ISearchSpace
    {
        IEnumerable<INode> Nodes { get; }
    }

    interface IPathFinder
    {

    }

    #endregion

    #region 2D Grid search

    class Grid2D : ISearchSpace
    {
        readonly Node[,] nodes;

        public Node this[int x, int y]
        {
            get
            {
                return nodes[x, y];
            }
        }

        public IEnumerable<INode> Nodes
        {
            get
            {
                // cast will convert the 2D array into IEnumerable ;)
                return nodes.Cast<INode>();
            }
        }

        public Grid2D(int width, int height)
        {
            nodes = new Node[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    nodes[i, j] = new Node(this, i, j);
                }
            }
        }

        public IEnumerable<INode> GetNodeConnections(int x, int y)
        {
            var notLeftEdge = x > 0;
            var notRightEdge = x < nodes.GetLength(0) - 1;
            var notBottomEdge = y > 0;
            var notTopEdge = y < nodes.GetLength(1) - 1;

            var connections = new List<INode>();

            if (notLeftEdge) AddNodeIfValid(connections, nodes[x - 1, y]);
            if (notRightEdge) AddNodeIfValid(connections, nodes[x + 1, y]);
            if (notBottomEdge) AddNodeIfValid(connections, nodes[x, y - 1]);
            if (notTopEdge) AddNodeIfValid(connections, nodes[x, y + 1]);

            if (notLeftEdge && notBottomEdge) AddNodeIfValid(connections, nodes[x - 1, y - 1]);
            if (notLeftEdge && notTopEdge) AddNodeIfValid(connections, nodes[x - 1, y + 1]);
            if (notRightEdge && notBottomEdge) AddNodeIfValid(connections, nodes[x + 1, y - 1]);
            if (notRightEdge && notTopEdge) AddNodeIfValid(connections, nodes[x + 1, y + 1]);

            return connections;
        }

        void AddNodeIfValid(List<INode> list, INode node)
        {
            if (node.Weight < float.MaxValue) list.Add(node);
        }

        public static float HeuristicManhattan(INode from, INode to)
        {
            var nFrom = from as Node;
            var nTo = to as Node;

            // Manhattan distance on a square grid
            return Mathf.Abs(nFrom.X - nTo.X) + Mathf.Abs(nFrom.Y - nTo.Y);
        }
    }

    class Node : INode
    {
        public readonly Grid2D Grid;

        public readonly int X;
        public readonly int Y;

        public float Weight { get; set; }

        public IEnumerable<INode> Connections
        {
            get
            {
                return Grid.GetNodeConnections(X, Y);
            }
        }

        public Node(Grid2D grid, int x, int y, float weight = 1)
        {
            Grid = grid;
            X = x;
            Y = y;
            Weight = weight;
        }
    }

    #endregion

    #region A*

    class AStar : IPathFinder
    {
        public delegate float HeuristicFunc(INode from, INode to);

        public PriorityQueue<INode> frontier = new PriorityQueue<INode>(100);
        public Dictionary<INode, INode> mappedNodes = new Dictionary<INode, INode>();
        public Dictionary<INode, float> mappedNodeCosts = new Dictionary<INode, float>();

        public HeuristicFunc Heuristic { get; set; }

        public AStar(HeuristicFunc heuristic)
        {
            Heuristic = heuristic;
        }

        public IEnumerable<INode> FindPath(INode startNode, INode endNode)
        {
            var path = new LinkedList<INode>();
            var currentNode = endNode;

            while (AdvanceFrontier(startNode, endNode, null).MoveNext()) ;

            INode nextNode;
            while ((nextNode = mappedNodes[currentNode]) != null)
            {
                path.AddFirst(nextNode);
                currentNode = nextNode;
            }

            foreach (Node n in path)
            {
                Debug.Log(n.X + " " + n.Y);
            }

            return path;
        }

        public IEnumerator AdvanceFrontier(INode startNode, INode endNode, System.Func<Coroutine> coroutine = null)
        {
            frontier.Enqueue(startNode, 0);
            mappedNodes.Add(startNode, null);
            mappedNodeCosts.Add(startNode, 0);

            while (frontier.Count > 0)
            {
                var current = frontier.Dequeue();

                var @break = false;
                foreach (var connection in current.Value.Connections)
                {
                    var newCost = mappedNodeCosts[current.Value] + connection.Weight;
                    if (!mappedNodes.ContainsKey(connection) || newCost < mappedNodeCosts[connection])
                    {
                        // connection came from current
                        if (mappedNodes.ContainsKey(connection))
                        {
                            mappedNodes[connection] = current.Value;
                        }
                        else
                        {
                            mappedNodes.Add(connection, current.Value);
                        }

                        if (mappedNodeCosts.ContainsKey(connection))
                        {
                            mappedNodeCosts[connection] = newCost;
                        }
                        else
                        {
                            mappedNodeCosts.Add(connection, newCost);
                        }


                        var priority = newCost + Heuristic(endNode, connection);
                        frontier.Enqueue(new PQNode<INode>(connection), priority);

                        if (connection == endNode)
                        {
                            @break = true;
                            break;
                        }

                    }
                }

                if (coroutine != null)
                {
                    yield return coroutine();
                }

                if (@break) break;
            }
        }

    }

    #endregion

    #region Unity Visual Code

    RectTransform gridSpace;
    Image[,] tiles;

    void Awake()
    {
        gridSpace = transform.Find("GridSpace") as RectTransform;

        var size = 25;

        var grid = new Grid2D(size, size);


        grid[5, 5].Weight = float.MaxValue;
        grid[5, 6].Weight = float.MaxValue;
        grid[5, 7].Weight = float.MaxValue;
        grid[5, 8].Weight = float.MaxValue;
        grid[5, 9].Weight = float.MaxValue;
        grid[5, 10].Weight = float.MaxValue;
        grid[5, 11].Weight = float.MaxValue;
        grid[5, 12].Weight = float.MaxValue;
        grid[5, 13].Weight = float.MaxValue;
        grid[5, 14].Weight = float.MaxValue;

        grid[15, 5].Weight = float.MaxValue;
        grid[15, 6].Weight = float.MaxValue;
        grid[15, 7].Weight = float.MaxValue;
        grid[15, 8].Weight = float.MaxValue;
        grid[15, 9].Weight = float.MaxValue;
        grid[15, 10].Weight = float.MaxValue;
        grid[15, 11].Weight = float.MaxValue;
        grid[15, 12].Weight = float.MaxValue;
        grid[15, 13].Weight = float.MaxValue;
        grid[15, 14].Weight = float.MaxValue;

        grid[5,  15].Weight = float.MaxValue;
        grid[6,  15].Weight = float.MaxValue;
        grid[7,  15].Weight = float.MaxValue;
        grid[8,  15].Weight = float.MaxValue;
        grid[9,  15].Weight = float.MaxValue;
        grid[10, 15].Weight = float.MaxValue;
        grid[11, 15].Weight = float.MaxValue;
        grid[12, 15].Weight = float.MaxValue;
        grid[13, 15].Weight = float.MaxValue;
        grid[14, 15].Weight = float.MaxValue;
        grid[15, 15].Weight = float.MaxValue;

        tiles = new Image[size, size];

        Vector2 anchorMin, anchorMax;
        for (int i = 0; i < size; i++)
        {
            anchorMin.x = ((float)i / size);
            anchorMax.x = ((float)(i + 1) / size);

            for (int j = 0; j < size; j++)
            {
                anchorMin.y = ((float)j / size);
                anchorMax.y = ((float)(j + 1) / size);

                var obj = new GameObject(string.Format("Node [{0},{1}]", i, j));
                obj.transform.SetParent(gridSpace);

                var trans = obj.AddComponent<RectTransform>();
                trans.anchorMin = anchorMin;
                trans.anchorMax = anchorMax;
                trans.offsetMax = Vector2.zero;
                trans.offsetMin = Vector2.zero;
                trans.localScale = Vector3.one;

                var image = obj.AddComponent<Image>();
                image.color = (Color.blue * anchorMax.x + Color.red * (1 - anchorMax.x)) * anchorMax.y +
                              (Color.yellow * anchorMax.x + Color.green * (1 - anchorMax.x)) * (1 - anchorMax.y);

                var c = 1f / (grid[i, j].Weight);
                image.color = new Color(c, c, c, 1);

                tiles[i, j] = image;
            }
        }


        var search = new AStar(Grid2D.HeuristicManhattan);
        var start = grid[14, 3];
        var end = grid[7, 18];

        tiles[start.X, start.Y].color = Color.blue + Color.white * .5f;
        tiles[end.X, end.Y].color = Color.yellow + Color.white * .5f;

        //StartCoroutine(search.AdvanceFrontier(start, end, () => {
        //    foreach (Node node in search.mappedNodes.Keys)
        //        if (node != start && node != end)
        //            tiles[node.X, node.Y].color = Color.red + Color.white * .5f;

        //    foreach (Node node in search.frontier)
        //        if (node != start && node != end)
        //            tiles[node.X, node.Y].color = Color.green + Color.white * .5f;

        //    return StartCoroutine(WaitKeyDown(KeyCode.None));
        //}));

        var path = search.FindPath(start, end);
        foreach (Node node in search.mappedNodes.Keys)
            if (node != start && node != end)
                tiles[node.X, node.Y].color = Color.red + Color.white * .5f;

        foreach (Node node in search.frontier)
            if (node != start && node != end)
                tiles[node.X, node.Y].color = Color.green + Color.white * .5f;

        foreach (Node node in path)
            if (node != start && node != end)
                tiles[node.X, node.Y].color = Color.magenta + Color.white * .5f;


    }

    public IEnumerator WaitKeyDown(KeyCode key)
    {
        do
        {
            yield return new WaitForSeconds(.05f);
        }
        while (key != KeyCode.None && !Input.GetKey(key));

        Debug.LogFormat("Key {0} pressed", key);
    }

    #endregion
}
