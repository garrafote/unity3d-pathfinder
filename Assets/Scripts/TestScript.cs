using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PathFinder;
using PathFinder.Grid2D;
using UnityEngine.Assertions.Must;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using UnityEditorInternal.VersionControl;

public class TestScript : MonoBehaviour {
    #region Search API

    #endregion

    RectTransform gridSpace;
    Image[,] tiles;

    protected void Awake()
    {
        gridSpace = transform.Find("GridSpace") as RectTransform;

        var size = 100;
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

        gridSpace.gameObject.AddComponent<Selectable>();


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
        var searchA = new AStar(Grid2D.HeuristicManhattan2);
        var start = grid[14, 5];
        var end = grid[90, 90];

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

        // astar


        var beforea = System.DateTime.Now;
        var pathA = searchA.FindPath(start, end);
        var aftera = System.DateTime.Now;

        Debug.Log((aftera - beforea).Milliseconds);

        //foreach (Node2D node in searchA.mappedNodes.Keys)
        //    if (node != start && node != end)
        //        tiles[node.X, node.Y].color = Color.red + Color.white * .5f;

        //foreach (Node2D node in searchA.frontier)
        //    if (node != start && node != end)
        //        tiles[node.X, node.Y].color = Color.green + Color.white * .5f;

        //foreach (Node2D node in pathA)
        //    if (node != start && node != end)
        //        tiles[node.X, node.Y].color = Color.magenta + Color.white * .5f;

        // fringe

        var before = System.DateTime.Now;
        var path = search.FindPath(start, end);
        var after = System.DateTime.Now;

        Debug.Log((after - before).Milliseconds);

        foreach (Node2D node in search.cache.Keys)
            if (node != start && node != end)
                tiles[node.X, node.Y].color = Color.red + Color.white * .5f;

        foreach (Node2D node in search.fringe)
            if (node != start && node != end)
                tiles[node.X, node.Y].color = Color.green + Color.white * .5f;

        foreach (Node2D node in path)
            if (node != start && node != end)
                tiles[node.X, node.Y].color = Color.magenta + Color.white * .5f;

        //StartCoroutine(search.FindInteractive(start, end, () =>
        //{
        //    foreach (Node2D node in search.cache.Keys)
        //        if (node != start && node != end)
        //            tiles[node.X, node.Y].color = Color.red + Color.white * .5f;

        //    foreach (Node2D node in search.fringe)
        //        if (node != start && node != end)
        //            tiles[node.X, node.Y].color = Color.green + Color.white * .5f;

        //    foreach (Node2D node in search.pathInteractive)
        //        if (node != start && node != end)
        //            tiles[node.X, node.Y].color = Color.magenta + Color.white * .5f;

        //    return StartCoroutine(WaitKeyDown(KeyCode.None));
        //}));


    }

    void Update()
    {
        var res = new List<RaycastResult>();
        var pem = new PointerEventData(EventSystem.current);
        pem.position = Input.mousePosition;

        EventSystem.current.RaycastAll(pem, res);

        if (res.Count == 0)
            return;

        var pos = res[0].worldPosition;

        var corners = new Vector3[4];
        gridSpace.GetWorldCorners(corners);

        var min = corners[0];
        var max = corners[2];

        var width = max.x - min.x;
        var height = max.y - min.y;

        var i = (pos.x - min.x) * 25 / width;
        var j = (pos.y - min.y) * 25 / height;

        Debug.Log(i + " " + j);
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
