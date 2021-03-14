using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class AbilityTreeEditor : EditorWindow
{
    public Vector2 gridScroll = new Vector2();
    public Rect gridRect;
    public Vector2 dataScroll = new Vector2();
    public Rect dataRect;

    public string treeName = "My Ability Tree";
    public List<AbilityNode> myNodes = new List<AbilityNode>();
    public AbilityNode activeNode;

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



        if (GUI.Button(new Rect(10, position.height - 50, 200, 30), new GUIContent("Test button")))
        {
            
        }

        GUIStyle myStyle = new GUIStyle();
        myStyle.fontSize = 30;
        myStyle.alignment = TextAnchor.MiddleCenter;
        GUI.Box(new Rect(position.width / 2 - 100, 10, 200, 30), "");
        GUI.Label(new Rect(position.width / 2 - 100, 10, 200, 30), new GUIContent(treeName), myStyle);

        ProcessEvents(Event.current);
        if (GUI.changed) Repaint();
    }

    private void SaveTree()
    {
        AbilityTreeSerializable dataTree = new AbilityTreeSerializable(myNodes);
        string jsonTree = JsonUtility.ToJson(dataTree, true);

        File.WriteAllText("Assets/Resources/Data/Ability Trees/" + treeName + ".json", jsonTree);
        AssetDatabase.Refresh();
    }

    private void SaveTreeAs()
    {
        AbilityTreeSerializable dataTree = new AbilityTreeSerializable(myNodes);
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
            myNodes = new List<AbilityNode>();
            for (int i = 0; i < tree.nodeNames.Length; i++)
            {
                myNodes.Add(new AbilityNode(this, tree.nodePositions[i])
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
        for (int i = 0; i < myNodes.Count; i++)
        {
            GUI.color = Color.white;
            AbilityNode node = myNodes[i];

            if (node == activeNode)
                GUI.color = Color.yellow;
            node.Draw(i);
        }
        GUI.color = Color.white;
        EndWindows();
    }

    private void DrawNodeData()
    {
        if (activeNode != null)
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Name", GUILayout.Width(100));
            activeNode.name = EditorGUILayout.DelayedTextField(activeNode.name);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Description", GUILayout.Width(100));
            activeNode.description = EditorGUILayout.DelayedTextField(activeNode.description);
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
        myNodes.Add(new AbilityNode(this, position));
    }

    public void DeleteNode(AbilityNode node)
    {
        myNodes.Remove(node);
        if (activeNode == node)
            activeNode = null;
    }

    public void ResetEditor()
    {
        treeName = "My Ability Tree";
        myNodes = new List<AbilityNode>();
        activeNode = null;
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

    public AbilityTreeSerializable(List<AbilityNode> nodeList)
    {
        int arraySize = nodeList.Count;

        nodeNames = new string[arraySize];
        nodeDescriptions = new string[arraySize];
        nodePositions = new Vector2[arraySize];

        for (int i = 0; i < arraySize; i++)
        {
            nodeNames[i] = nodeList[i].name;
            nodeDescriptions[i] = nodeList[i].description;
            nodePositions[i] = nodeList[i].rect.position;
        }
    }
}