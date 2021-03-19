using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class AbilityTreeEditor : EditorWindow
{
    private Vector2 _gridScroll = new Vector2();

    public float scale = 1f;
    public float gridSize { get { return 2000f * scale; } }
    public float gridStep { get { return 100f * scale; } }
    public Vector2 gridScroll
    {
        get { return _gridScroll; }
        set { _gridScroll = new Vector2(
            Mathf.Clamp(value.x, 0f, gridSize - gridRect.width), 
            Mathf.Clamp(value.y, 0f, gridSize - gridRect.height)); }
    }
    public Rect gridRect;
    public Vector2 dataScroll = new Vector2();
    public Rect dataRect;

    public string treeName = "My Ability Tree";
    public List<AbilityNode> nodes = new List<AbilityNode>();
    public List<AbilityNode> selectedNodes = new List<AbilityNode>();
    public AbilityNode draggedNode = null;
    public Vector2 mouseDownLocation = new Vector2();
    public Vector2 mouseDragLocation = new Vector2();

    // EVENT HANDLING PARAMETERS
    public float draggedDistance;
    public bool forbidDeselectionFlag = false;
    public bool dragBoxFlag = false;
    public List<Vector2> dragOrigins;
    public Vector2 dragOffset = new Vector2();
    public bool gridSnap = false;

    [MenuItem("Window/Pokemon/AbilityTreeEditor")]
    public static void ShowEditor()
    {
        AbilityTreeEditor editor = GetWindow<AbilityTreeEditor>();
        editor.titleContent = new GUIContent("Ability Tree Editor");
    }

    private void OnEnable()
    {
        ResetEditor();
    }

    private void OnGUI()
    {
        if (Event.current.type == EventType.ScrollWheel)
        {
            scale -= Event.current.delta.y * 0.01f;
            scale = Mathf.Clamp(scale, 0.5f, 2f);


        }
            
        
        EditorGUILayout.BeginHorizontal();

        // The node grid layout
        EditorGUILayout.BeginScrollView(gridScroll, GUIStyle.none, GUIStyle.none);
        DrawNodeGrid();
        EditorGUILayout.EndScrollView();
        gridRect = GUILayoutUtility.GetLastRect();

        // The node data editor
        dataScroll = EditorGUILayout.BeginScrollView(dataScroll, false, true, GUILayout.MinWidth(300));
        DrawNodeData();
        EditorGUILayout.EndScrollView();
        dataRect = GUILayoutUtility.GetLastRect();

        EditorGUILayout.EndHorizontal();



        if (GUI.Button(new Rect(10, 10, 100, 30), new GUIContent("NEW TREE")))
            ResetEditor();

        if (GUI.Button(new Rect(110, 10, 100, 30), new GUIContent("LOAD TREE")))
            LoadTree();

        if (GUI.Button(new Rect(10, 50, 150, 30), new GUIContent("SAVE TREE")))
            SaveTree();

        if (GUI.Button(new Rect(160, 50, 50, 30), new GUIContent("AS...")))
            SaveTreeAs();


        if (GUI.Button(new Rect(10, 90, 200, 30), new GUIContent("Test button")))
        {
            gridSnap = !gridSnap;
            Debug.Log("Grid snap = " + gridSnap);
        }

        DrawMap(120);


        GUIStyle myStyle = new GUIStyle();
        myStyle.fontSize = 30;
        myStyle.alignment = TextAnchor.MiddleCenter;
        GUI.Box(new Rect(position.width / 2 - 100, 10, 200, 30), "");
        GUI.Label(new Rect(position.width / 2 - 100, 10, 200, 30), new GUIContent(treeName), myStyle);

        ProcessEvents(Event.current);
        if (GUI.changed) Repaint();
    }

    // ONGUI DRAWING METHODS
    private void DrawMap(float mapSize)
    {
        Texture circle = Resources.Load<Texture>("Editor/circle");

        // Map outline
        GUILayout.BeginArea(new Rect(5, position.height - mapSize - 20, mapSize, mapSize));
        GUI.backgroundColor = new Color(0, 0, 0, 0.5f);
        GUI.Box(new Rect(0, 0, mapSize, mapSize), "");
        GUI.backgroundColor = new Color(1, 1, 1, 0.5f);
        GUI.Box(new Rect(2, 2, mapSize - 4, mapSize - 4), "");

        // Nodes
        GUI.backgroundColor = Color.clear;
        for (int i = 0; i < nodes.Count; i++)
        {
            int selectedIndex = selectedNodes.IndexOf(nodes[i]);
            if (selectedIndex > -1)
            {
                if (selectedIndex == selectedNodes.Count - 1)
                    GUI.contentColor = Color.green;
                else GUI.contentColor = Color.yellow;
            }
            else GUI.contentColor = Color.black;

            Vector2 nodePosition = new Vector2
                (nodes[i].position.x / gridSize * mapSize, nodes[i].position.y / gridSize * mapSize);
            GUI.Box(new Rect(nodePosition.x * scale, nodePosition.y * scale, 13, 13), circle);
        }

        // White box
        Handles.BeginGUI();
        Handles.color = Color.white;
        Vector3 corner0, corner1;
        corner0 = new Vector3(gridScroll.x, gridScroll.y);
        corner1 = new Vector3(gridRect.width, gridRect.height, 0);
        corner0 *= mapSize / gridSize;
        corner1 *= mapSize / gridSize;
        Handles.DrawLines(new Vector3[]
        {
            corner0, corner0 + Vector3.right * corner1.x,
            corner0, corner0 + Vector3.up * corner1.y,
            corner0 + corner1, corner0 + Vector3.right * corner1.x,
            corner0 + corner1, corner0 + Vector3.up * corner1.y
        });
        Handles.EndGUI();

        Handles.color = Color.black;
        GUI.contentColor = GUI.backgroundColor = Color.white;
        GUILayout.EndArea();
    }

    private void DrawNodeGrid()
    {
        GUILayout.Box("", GUILayout.Width(gridSize), GUILayout.Height(gridSize));

        Handles.BeginGUI();
        Handles.color = Color.black;
        for (int i = 0; i < gridSize / gridStep; i++)
        {
            Handles.DrawLine(new Vector3(0, gridStep * i, 0), new Vector3(gridSize, gridStep * i, 0));
            Handles.DrawLine(new Vector3(gridStep * i, 0, 0), new Vector3(gridStep * i, gridSize, 0));
        }
        Handles.EndGUI();


        BeginWindows();
        for (int i = 0; i < nodes.Count; i++)
        {
            AbilityNode node = nodes[i];

            int selectedIndex = selectedNodes.IndexOf(node);
            if (selectedIndex > -1)
            {
                if (selectedIndex == selectedNodes.Count - 1)
                    GUI.color = Color.green;
                else GUI.color = Color.yellow;
            }
            else GUI.color = Color.white;

            node.Draw(i);
        }
        GUI.color = Color.white;
        EndWindows();


        if (dragBoxFlag)
        {
            Vector2 corner0, corner1;

            corner0 = mouseDownLocation + Vector2.right * (mouseDragLocation - mouseDownLocation).x;
            corner1 = mouseDownLocation + Vector2.up * (mouseDragLocation - mouseDownLocation).y;

            Handles.BeginGUI();
            Handles.DrawDottedLines(new Vector3[]
            {
                mouseDownLocation, corner0,
                mouseDownLocation, corner1,
                mouseDragLocation, corner0,
                mouseDragLocation, corner1
            }, 2f);
            Handles.EndGUI();
        }
    }

    private void DrawNodeData()
    {
        if (selectedNodes.Count > 0)
        {
            AbilityNode dataNode = selectedNodes[selectedNodes.Count - 1];

            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Name", GUILayout.Width(100));
            dataNode.name = EditorGUILayout.DelayedTextField(dataNode.name);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Description", GUILayout.Width(100));
            dataNode.description = EditorGUILayout.DelayedTextField(dataNode.description);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }
        else
        {
            EditorGUILayout.LabelField("Select a node");
        }
    }

    // EVENT METHODS
    private void ProcessEvents(Event e)
    {
        int controlId = GUIUtility.GetControlID(FocusType.Passive);

        if (gridRect.Contains(e.mousePosition))
        {
            foreach(AbilityNode node in nodes)
            {
                Rect nodeRect = new Rect(
                    node.rect.position.x - gridScroll.x, node.rect.position.y - gridScroll.y,
                    100, 100);

                if (nodeRect.Contains(e.mousePosition))
                {
                    node.ProcessEvents(e);
                    break;
                }
            }

            switch(e.GetTypeForControl(controlId))
            {
                case EventType.MouseDown:
                    
                    GUIUtility.hotControl = controlId;

                    switch(e.button)
                    {
                        case 0:
                            BeginBoxDrag(e);
                            break;

                        case 1:
                            ShowGridContextMenu(e);
                            break;
                    }
                    break;

                case EventType.ScrollWheel:
                    Debug.Log(e.delta);
                    break;
            }
        }

        switch(e.GetTypeForControl(controlId))
        {
            case EventType.MouseDrag:
                switch (e.button)
                {
                    case 0:
                        if (draggedNode != null)
                            DragNodes(e);
                        else
                            UpdateBoxDrag(e);
                        break;

                    case 2:
                        DragGridScroll(e);
                        break;
                }
                break;

            case EventType.MouseUp:
                if (dragBoxFlag)
                    EndBoxDrag(e);

                GUIUtility.hotControl = 0;
                draggedNode = null;
                draggedDistance = 0;
                dragOffset = new Vector2();
                dragOrigins = new List<Vector2>();
                break;
        }
    }

    private void EndBoxDrag(Event e)
    {
        Rect boxRect = new Rect(mouseDownLocation, mouseDragLocation - mouseDownLocation);

        if (!e.shift)
            selectedNodes = new List<AbilityNode>();

        foreach (AbilityNode node in nodes)
        {
            if (boxRect.Overlaps(node.rect, true))
            {
                selectedNodes.Remove(node);
                selectedNodes.Add(node);
            }
        }

        dragBoxFlag = false;

        GUI.changed = true;
        e.Use();
    }

    private void DragGridScroll(Event e)
    {
        gridScroll -= e.delta;

        GUI.changed = true;
        e.Use();
    }

    private void UpdateBoxDrag(Event e)
    {
        mouseDragLocation = gridScroll + e.mousePosition;

        GUI.changed = true;
        e.Use();
    }

    private void DragNodes(Event e)
    {
        draggedDistance += e.delta.magnitude;
        dragOffset += e.delta;

        if (gridSnap)
        {
            for (int i = 0; i < dragOrigins.Count; i++)
            {
                Vector2 snappedCoords = new Vector2(dragOrigins[i].x + dragOffset.x, dragOrigins[i].y + dragOffset.y);
                snappedCoords = new Vector2(
                    Mathf.Round(snappedCoords.x / gridStep) * gridStep, 
                    Mathf.Round(snappedCoords.y / gridStep) * gridStep);

                if (i == dragOrigins.Count - 1)
                    draggedNode.position = new Vector2(snappedCoords.x, snappedCoords.y);
                else
                    selectedNodes[i].position = new Vector2(snappedCoords.x, snappedCoords.y);
            }
        }
        else
        {
            foreach (AbilityNode node in selectedNodes)
                if (node != draggedNode)
                    node.position += e.delta / scale;

            draggedNode.position += e.delta / scale;
        }

        GUI.changed = true;
        e.Use();
    }

    private void BeginBoxDrag(Event e)
    {
        mouseDownLocation = mouseDragLocation = gridScroll + e.mousePosition;
        dragBoxFlag = true;

        GUI.changed = true;
        e.Use();
    }

    private void ShowGridContextMenu(Event e)
    {
        GenericMenu myMenu = new GenericMenu();
        myMenu.AddItem(new GUIContent("New node"), false, () => CreateNewNode(e.mousePosition + gridScroll));
        myMenu.ShowAsContext();

        GUI.changed = true;
        e.Use();
    }

    // BUTTON AND OTHER METHODS
    private void SaveTree()
    {
        AbilityTreeSerializable dataTree = new AbilityTreeSerializable(this);
        string jsonTree = JsonUtility.ToJson(dataTree, true);

        File.WriteAllText("Assets/Resources/Data/Ability Trees/" + treeName + ".json", jsonTree);
        AssetDatabase.Refresh();
    }

    private void SaveTreeAs()
    {
        AbilityTreeSerializable dataTree = new AbilityTreeSerializable(this);
        string jsonTree = JsonUtility.ToJson(dataTree, true);

        string saveLocation = EditorUtility.SaveFilePanel
            ("Save an ability tree", Application.dataPath + "/Resources/Data/Ability Trees/", treeName, "json");

        if (saveLocation != "")
        {
            File.WriteAllText(saveLocation, jsonTree);
            AssetDatabase.Refresh();
        }
    }

    private void LoadTree()
    {
        string treePath = EditorUtility.OpenFilePanel
    ("Select ability tree to load", Application.dataPath + "/Resources/Data/Ability Trees/", "json");

        if (treePath != "")
        {
            treePath = treePath.Replace(Application.dataPath + "/Resources/", "");
            treePath = treePath.Replace(".json", "");
            TextAsset treeFile = Resources.Load<TextAsset>(treePath);

            AbilityTreeSerializable tree = new AbilityTreeSerializable();
            JsonUtility.FromJsonOverwrite(treeFile.ToString(), tree);

            // Populate the editor
            treeName = treeFile.name;
            nodes = new List<AbilityNode>();
            for (int i = 0; i < tree.nodeNames.Length; i++)
            {
                Vector2 position = tree.nodePositions[i];
                nodes.Add(new AbilityNode(this, position * gridSize)
                {
                    name = tree.nodeNames[i],
                    description = tree.nodeDescriptions[i]
                });
            }
        }
    }

    private void CreateNewNode(Vector2 position)
    {
        nodes.Add(new AbilityNode(this, position));
    }

    public void DeleteNode(AbilityNode node)
    {
        nodes.Remove(node);
        selectedNodes.Remove(node);
    }

    public void ResetEditor()
    {
        treeName = "My Ability Tree";
        nodes = new List<AbilityNode>();
        selectedNodes = new List<AbilityNode>();
    }
}


public class AbilityTreeSerializable
{
    public string[] nodeNames;
    public string[] nodeDescriptions;
    public Vector2[] nodePositions;

    public AbilityTreeSerializable()
    {

    }

    public AbilityTreeSerializable(AbilityTreeEditor editor)
    {
        List<AbilityNode> nodes = editor.nodes;
        int arraySize = nodes.Count;

        nodeNames = new string[arraySize];
        nodeDescriptions = new string[arraySize];
        nodePositions = new Vector2[arraySize];

        for (int i = 0; i < arraySize; i++)
        {
            nodePositions[i] = nodes[i].position / editor.gridSize;
            nodeNames[i] = nodes[i].name;
            nodeDescriptions[i] = nodes[i].description;
        }
    }
}