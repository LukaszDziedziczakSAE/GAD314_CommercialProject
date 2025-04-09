using System.Collections.Generic;
using UnityEngine;

public class UI_Objectives : MonoBehaviour
{
    [SerializeField] UI_ObjectivesItem objectivesItemPrefab;

    List<UI_ObjectivesItem> list = new List<UI_ObjectivesItem>();

    public void UpdateObjectiveUI()
    {
        ClearList();

        if (Game.Tutorial.IsRunning)
        {
            if (Game.Tutorial.CurrentPart.ShowNextObjective)
            {
                UI_ObjectivesItem objectiveItem = Instantiate(objectivesItemPrefab, transform);
                objectiveItem.Initilize(Game.Tutorial.NextObjective);
                list.Add(objectiveItem);
            }
        }
        else
        {
            foreach (Objective objective in Game.ObjectiveSystem.Objectives)
            {
                UI_ObjectivesItem objectiveItem = Instantiate(objectivesItemPrefab, transform);
                objectiveItem.Initilize(objective);
                list.Add(objectiveItem);
            }
        }
            
    }

    private void ClearList()
    {
        foreach(UI_ObjectivesItem item in list)
        {
            if (item != null) Destroy(item.gameObject);
        }
        list.Clear();
    }
}
