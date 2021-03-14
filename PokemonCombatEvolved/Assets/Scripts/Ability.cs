using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Esto debe ser serializable en el futuro
public class Ability
{
    public readonly string name, description;
    public readonly bool treeRoot;
    public List<Ability> parents { get; }
    public List<Ability> children { get; }

    public Ability(string name, string description, bool treeRoot)
    {
        this.name = name;
        this.description = description;
        this.treeRoot = treeRoot;

        parents = new List<Ability>();
        children = new List<Ability>();
    }

    public void AddParent(Ability parentAbility)
    {
        parents.Add(parentAbility);
        parentAbility.children.Add(this);
    }
    public void AddChild(Ability childAbility)
    {
        children.Add(childAbility);
        childAbility.parents.Add(this);
    }

    public AbilityJson ToJson()
    {
        List<string> parentNames = new List<string>();
        List<string> childrenNames = new List<string>();

        foreach (Ability parentAbility in parents)
            parentNames.Add(parentAbility.name);
        foreach (Ability childAbility in children)
            childrenNames.Add(childAbility.name);

        AbilityJson abilityJson = new AbilityJson(name, description, parentNames.ToArray(), childrenNames.ToArray());
        return abilityJson;
    }
}


public class AbilityJson
{
    public string name, description;
    public string[] parentNames;
    public string[] childNames;

    public AbilityJson(string name, string description, string[] parentNames, string[] childNames)
    {
        this.name = name;
        this.description = description;
        this.parentNames = parentNames;
        this.childNames = childNames;
    }

    public override string ToString()
    {
        return JsonUtility.ToJson(this, true);
    }
}