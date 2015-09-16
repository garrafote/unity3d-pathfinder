using System;
using UnityEngine;
using System.Collections;
using System.Reflection;
using Assets.Scripts;

public class ClearConsoleOnMouseDown : MonoBehaviour {

	// Update is called once per frame
	void Update () {
	    if (Input.GetMouseButtonDown(0))
	    {
            var assembly = Assembly.GetAssembly(typeof(UnityEditor.ActiveEditorTracker));
            var type = assembly.GetType("UnityEditorInternal.LogEntries");
            var method = type.GetMethod("Clear");
            ResearchData.WriteLine();
            method.Invoke(new object(), null);
        }
	}
}
