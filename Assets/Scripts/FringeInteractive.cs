using UnityEngine;
using System.Collections;
using PathFinder;
using PathFinder.Grid2D;
using UnityEngine.UI;

public class FringeInteractive : MonoBehaviour {
    #region Search API

    #endregion

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

        grid[5, 15].Weight = float.MaxValue;
        grid[6, 15].Weight = float.MaxValue;
        grid[7, 15].Weight = float.MaxValue;
        grid[8, 15].Weight = float.MaxValue;
        grid[9, 15].Weight = float.MaxValue;
        grid[10, 15].Weight = float.MaxValue;
        grid[11, 15].Weight = float.MaxValue;
        grid[12, 15].Weight = float.MaxValue;
        grid[13, 15].Weight = float.MaxValue;
        grid[14, 15].Weight = float.MaxValue;
        grid[15, 15].Weight = float.MaxValue;

        //grid[5,  4].Weight = float.MaxValue;
        //grid[6,  4].Weight = float.MaxValue;
        //grid[7,  4].Weight = float.MaxValue;
        //grid[8,  4].Weight = float.MaxValue;
        //grid[9,  4].Weight = float.MaxValue;
        //grid[10, 4].Weight = float.MaxValue;
        //grid[11, 4].Weight = float.MaxValue;
        //grid[12, 4].Weight = float.MaxValue;
        //grid[13, 4].Weight = float.MaxValue;
        //grid[14, 4].Weight = float.MaxValue;
        //grid[15, 4].Weight = float.MaxValue;


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


        var search = new Fringe(Grid2D.HeuristicManhattan2);
        var start = grid[14, 5];
        var end = grid[7, 24];

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

        //// astar
        //var path = search.FindPath(start, end);
        //foreach (Node node in search.mappedNodes.Keys)
        //    if (node != start && node != end)
        //        tiles[node.X, node.Y].color = Color.red + Color.white * .5f;

        //foreach (Node node in search.frontier)
        //    if (node != start && node != end)
        //        tiles[node.X, node.Y].color = Color.green + Color.white * .5f;

        //foreach (Node node in path)
        //    if (node != start && node != end)
        //        tiles[node.X, node.Y].color = Color.magenta + Color.white * .5f;

        // fringe
        //var path = search.FindPath(start, end);
        //foreach (Node node in search.cache.Keys)
        //    if (node != start && node != end)
        //        tiles[node.X, node.Y].color = Color.red + Color.white * .5f;

        //foreach (Node node in search.fringe)
        //    if (node != start && node != end)
        //        tiles[node.X, node.Y].color = Color.green + Color.white * .5f;

        //foreach (Node node in path)
        //    if (node != start && node != end)
        //        tiles[node.X, node.Y].color = Color.magenta + Color.white * .5f;

        StartCoroutine(search.FindInteractive(start, end, () =>
        {
            foreach (Node2D node in search.cache.Keys)
                if (node != start && node != end)
                    tiles[node.X, node.Y].color = Color.red + Color.white * .5f;

            foreach (Node2D node in search.fringe)
                if (node != start && node != end)
                    tiles[node.X, node.Y].color = Color.green + Color.white * .5f;

            foreach (Node2D node in search.pathInteractive)
                if (node != start && node != end)
                    tiles[node.X, node.Y].color = Color.magenta + Color.white * .5f;

            return StartCoroutine(WaitKeyDown(KeyCode.None));
        }));


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
