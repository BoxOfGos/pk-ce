using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AbilityNode
{
    public AbilityTreeEditor editor;
    public string name;
    public string description;
    public Rect rect { get {
            return new Rect(
                position.x * editor.scale - 50f * editor.scale, position.y * editor.scale - 50f * editor.scale, 
                100 * editor.scale, 100 * editor.scale); } }
    public Vector2 position;


    public AbilityNode(AbilityTreeEditor editor, Vector2 position)
    {
        this.editor = editor;
        this.position = position;
        name = "Name";
        description = "Description";
    }

    public void Draw(int id)
    {
        GUI.Window(id, rect, WindowFunction, name);
    }

    private void WindowFunction(int id)
    {
        GUILayout.BeginVertical();
        GUILayout.Label(description);
        GUILayout.EndVertical();
    }

    public void ProcessEvents(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
                editor.forbidDeselectionFlag = false;

                if (e.button == 0)
                {
                    editor.draggedNode = this;
                    if (!e.shift)
                        editor.selectedNodes = new List<AbilityNode>();

                    foreach (AbilityNode node in editor.selectedNodes)
                        editor.dragOrigins.Add(node.position);
                    editor.dragOrigins.Add(position);
                }

                e.Use();
                GUI.changed = true;
                break;

            case EventType.MouseUp:
                switch(e.button)
                {
                    case 0:
                        if (editor.draggedNode == this)
                        {
                            if (e.shift)
                            {
                                editor.selectedNodes.Remove(this);
                                editor.selectedNodes.Add(this);
                            }
                            else if (editor.selectedNodes.Contains(this))
                            {
                                if (editor.selectedNodes.IndexOf(this) == (editor.selectedNodes.Count - 1))
                                {
                                    if (editor.draggedDistance < 5)
                                        editor.selectedNodes.Remove(this);
                                }
                                else
                                {
                                    editor.selectedNodes = new List<AbilityNode>() { this };
                                }
                            }
                            else
                            {
                                editor.selectedNodes = new List<AbilityNode>() { this };
                            }
                        }
                        break;

                    case 1:
                        GenericMenu myMenu = new GenericMenu();
                        myMenu.AddItem(new GUIContent("Delete node"), false, () => editor.DeleteNode(this));
                        myMenu.ShowAsContext();
                        break;
                }

                GUI.changed = true;
                break;
        }
    }
}
