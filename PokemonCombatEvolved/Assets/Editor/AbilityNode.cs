using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AbilityNode
{
    public AbilityTreeEditor editor;
    public string name;
    public string description;
    public Rect rect;


    public AbilityNode(AbilityTreeEditor editor, Vector2 position)
    {
        this.editor = editor;
        rect = new Rect(position.x, position.y, 100, 100);
        name = "Name";
        description = "Description";
    }

    public void Draw(int id)
    {
        rect = GUI.Window(id, rect, WindowFunction, name);
    }

    private void WindowFunction(int id)
    {
        GUILayout.BeginVertical();
        GUILayout.Label(description);
        GUILayout.EndVertical();
    }

    public void ProcessEvents(Event e)
    {
        switch(e.type)
        {
            case EventType.MouseDown:
                editor.selectionFlag = false;

                if (e.shift)
                {
                    if (!editor.selectedNodes.Contains(this))
                    {
                        editor.selectedNodes.Add(this);
                        editor.selectionFlag = true;
                    }
                    else if (editor.selectedNodes.Count != 0)
                    {
                        if (editor.selectedNodes[editor.selectedNodes.Count - 1] != this)
                        {
                            editor.selectedNodes.Remove(this);
                            editor.selectedNodes.Add(this);
                            editor.selectionFlag = true;
                        }
                    }
                }
                else
                {
                    editor.selectedNodes = new List<AbilityNode>() { this };
                    editor.selectionFlag = true;
                }

                switch (e.button)
                {
                    case 0:
                        editor.draggedNode = this;
                        break;
                    case 1:
                        GenericMenu myMenu = new GenericMenu();
                        myMenu.AddItem(new GUIContent("Delete node"), false, () => editor.DeleteNode(this));
                        myMenu.ShowAsContext();
                        break;
                }

                e.Use();
                GUI.changed = true;
                break;

            case EventType.MouseUp:
                if (editor.draggedNode != null)
                {
                    if (editor.draggedDistance < 5 && !editor.selectionFlag)
                        editor.selectedNodes.Remove(editor.draggedNode);

                    editor.draggedNode = null;
                    editor.draggedDistance = 0f;

                    GUI.changed = true;
                }
                break;
        }
    }
}
