using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TestEditor : EditorWindow
{
    private Rect rect = new Rect(0, 0, 100, 150);

    [MenuItem("Window/Pokemon/TestEditor")]
    public static void ShowEditor()
    {
        TestEditor editor = GetWindow<TestEditor>();
    }

    public void OnGUI()
    {
        BeginWindows();
        rect = GUI.Window(1, rect, DoWindow, "placeholder");
        EndWindows();
    }

    private void DoWindow(int id)
    {
        

        GUI.DragWindow();
    }
}
