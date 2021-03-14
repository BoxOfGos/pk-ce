using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Este objeto sólo existe para facilitar la accesibilidad y transformación de data a UI
public class AbilityTree
{
    public readonly List<List<Ability>> ranks;
    public readonly HashSet<Ability> allAbilities;

    public AbilityTree(Ability rootAbility)
    {
        ranks = new List<List<Ability>>();
        allAbilities = new HashSet<Ability>();

        FillAbilityTree(rootAbility, 0);
    }

    // Se rellena el árbol de habilidades empezando por su habilidad raíz e iterando de hijo a hijo
    private void FillAbilityTree(Ability currentAbility, int currentRank)
    {
        while (ranks.Count <= currentRank)
            ranks.Add(new List<Ability>());

        if (allAbilities.Add(currentAbility))
        {
            ranks[currentRank].Add(currentAbility);
            foreach (Ability child in currentAbility.children)
            {
                FillAbilityTree(child, currentRank + 1);
            }
        }
    }
}