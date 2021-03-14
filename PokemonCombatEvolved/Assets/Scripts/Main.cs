using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public RectTransform abilityTreeContainer;
    public UILineRenderer lineRenderer;
    public RankContainerUI rankContainerUI;
    public AbilityContainerUI abilityContainerUI;

    // Toda el proceso de generación de objetos que actualmente se encuantra en el método Start 
    // debe ser el resultado de deserializar un archivo en el futuro
    void Start()
    {
        GameData data = new GameData();

        AbilityTree myTree = new AbilityTree(data.allAbilities[0]);

        DrawAbilityTree(myTree);

        Debug.Log(JsonConvert.SerializeObject(data.allAbilities[0].ToJson(), Formatting.Indented));
        Debug.Log(JsonConvert.SerializeObject(data.AbilitiesToJson(), Formatting.Indented));
    }


    // Este método sólo debe ser llamado para dibujar la interfaz.
    // ¿Acoplarlo a algún objeto de la UI? ¿Dónde debe ir?
    private void DrawAbilityTree(AbilityTree abilityTree)
    {
        Dictionary<Ability, AbilityContainerUI> abilityUIDictionary = new Dictionary<Ability, AbilityContainerUI>();

        int numberOfRanks = abilityTree.ranks.Count;
        for (int i = 0; i < numberOfRanks; i++)
        {
            RankContainerUI currentRankContainerUI =
                Instantiate(rankContainerUI.gameObject, abilityTreeContainer).GetComponent<RankContainerUI>();

            currentRankContainerUI.rectTransform.anchorMin = new Vector2(0f, i * 1f / numberOfRanks);
            currentRankContainerUI.rectTransform.anchorMax = new Vector2(1f, (i + 1) * 1f / numberOfRanks);
            currentRankContainerUI.text.text = "RANK " + (i + 1);

            int numberOfAbilities = abilityTree.ranks[i].Count;
            for (int j = 0; j < numberOfAbilities; j++)
            {
                AbilityContainerUI currentAbilityContainerUI =
                    Instantiate(abilityContainerUI.gameObject, currentRankContainerUI.rectTransform).GetComponent<AbilityContainerUI>();

                currentAbilityContainerUI.rectTransform.anchorMin = new Vector2(j * 1f / numberOfAbilities, 0f);
                currentAbilityContainerUI.rectTransform.anchorMax = new Vector2((j + 1) * 1f / numberOfAbilities, 1f);
                currentAbilityContainerUI.text.text = abilityTree.ranks[i][j].name;

                abilityUIDictionary.Add(abilityTree.ranks[i][j], currentAbilityContainerUI);
            }
        }

        foreach(Ability ability in abilityTree.allAbilities)
        {
            foreach(Ability parent in ability.parents)
            {
                List<RectTransform> linePoints = new List<RectTransform>();
                linePoints.Add(abilityUIDictionary[ability].rectTransform);
                linePoints.Add(abilityUIDictionary[parent].rectTransform);

                lineRenderer.lines.Add(linePoints);
            }
        }
    }
}
