using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class AbilityTreeEditor : EditorWindow
{
    public float gridSize = 2000f;
    public Vector2 gridScroll = new Vector2();
    public Rect gridRect;
    public Vector2 dataScroll = new Vector2();
    public Rect dataRect;

    public string treeName = "My Ability Tree";
    public List<AbilityNode> nodes = new List<AbilityNode>();
    public List<AbilityNode> selectedNodes = new List<AbilityNode>();

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
        EditorGUILayout.BeginHorizontal();

        // The node grid layout
        gridScroll = EditorGUILayout.BeginScrollView(gridScroll, true, true);
        DrawNodeGrid(2000, 100);
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
            Debug.Log(selectedNodes.Count);
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

    private void DrawMap(float size)
    {
        Vector2 mapSize = new Vector2(size, size);
        GUILayout.BeginArea(new Rect(5, position.height - size - 20, size, size));
        GUI.backgroundColor = new Color(0, 0, 0, 0.5f);
        GUI.Box(new Rect(0, 0, size, size), "");
        GUI.backgroundColor = new Color(1, 1, 1, 0.5f);
        GUI.Box(new Rect(2, 2, size - 4, size - 4), "");
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
                (nodes[i].rect.position.x / 2000f * size, nodes[i].rect.position.y / 2000f * size);
            GUI.Box(new Rect(nodePosition.x, nodePosition.y, 13, 13), (Texture)Resources.Load("Editor/circle"));
        }
        GUI.contentColor = GUI.backgroundColor = Color.white;
        GUILayout.EndArea();
    }

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

    // Draws the grid and all the nodes inside the currently open ability tree
    private void DrawNodeGrid(int scrollSize, int gridSize)
    {
        GUILayout.Box("", GUILayout.Width(scrollSize), GUILayout.Height(scrollSize));

        Handles.BeginGUI();
        Handles.color = Color.black;
        for (int i = 0; i < scrollSize / gridSize; i++)
        {
            Handles.DrawLine(new Vector3(0, gridSize * i, 0), new Vector3(scrollSize, gridSize * i, 0));
            Handles.DrawLine(new Vector3(gridSize * i, 0, 0), new Vector3(gridSize * i, scrollSize, 0));
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
    }

    private void ProcessEvents(Event e)
    {
        if (gridRect.Contains(e.mousePosition))
        {
            switch(e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 1)
                    {
                        GenericMenu myMenu = new GenericMenu();
                        myMenu.AddItem(new GUIContent("New node"), false, () => CreateNewNode(e.mousePosition + gridScroll));
                        myMenu.ShowAsContext();
                        e.Use();
                    }
                    break;
                case EventType.MouseDrag:
                    if (e.button == 0)
                    {
                        gridScroll -= e.delta;
                        GUI.changed = true;
                        e.Use();
                    }
                    break;
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
            nodePositions[i] = nodes[i].rect.position / editor.gridSize;
            nodeNames[i] = nodes[i].name;
            nodeDescriptions[i] = nodes[i].description;
        }
    }
}