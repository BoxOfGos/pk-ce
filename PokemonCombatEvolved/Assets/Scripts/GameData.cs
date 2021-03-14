using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    [SerializeField]
    public List<Ability> allAbilities;

    public GameData()
    {
        allAbilities = new List<Ability>();

        for (int i = 0; i < 7; i++)
        {
            allAbilities.Add(new Ability("Ability" + (i + 1), "No description", false));
        }

        allAbilities[0].AddChild(allAbilities[1]);
        allAbilities[0].AddChild(allAbilities[2]);
        allAbilities[1].AddChild(allAbilities[3]);
        allAbilities[1].AddChild(allAbilities[4]);
        allAbilities[2].AddChild(allAbilities[5]);
        allAbilities[3].AddChild(allAbilities[6]);
        allAbilities[4].AddChild(allAbilities[6]);
    }

    public List<AbilityJson> AbilitiesToJson()
    {
        List<AbilityJson> abilitiesJson = new List<AbilityJson>();

        foreach (Ability ability in allAbilities)
            abilitiesJson.Add(ability.ToJson());

        return abilitiesJson;
    }
}
