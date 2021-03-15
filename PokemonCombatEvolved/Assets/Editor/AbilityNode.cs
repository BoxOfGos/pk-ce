using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AbilityNode
{
    public string name;
    public string description;
    public Rect rect;

    public AbilityTreeEditor editor;

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

        Event e = Event.current;

        switch(e.type)
        {
            case EventType.MouseDown:
                if (e.shift)
                {
                    if (!editor.selectedNodes.Contains(this))
                        editor.selectedNodes.Add(this);
                    else editor.selectedNodes.Remove(this);
                }
                else
                    editor.selectedNodes = new List<AbilityNode> { this };

                if (e.button == 1)
                {
                    GenericMenu myMenu = new GenericMenu();
                    myMenu.AddItem(new GUIContent("Delete"), false, () => editor.DeleteNode(this));
                    myMenu.ShowAsContext();
                    e.Use();
                }
                GUI.changed = true;
                break;
        }

        GUI.DragWindow();
    }
}
