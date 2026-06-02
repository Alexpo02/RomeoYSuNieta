using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QuestObjective
{
    public QuestType type;
    public string objectiveID;
    public int amount;
    public string objetive;
    public string dialogueNpcID;
    public string dialogueID;
}

[CreateAssetMenu(fileName = "NuevaMisión", menuName = "Quests/Quest")]
public class QuestData : ScriptableObject
{
    public string id;
    public string title;
    public string description;
    public QuestObjective[] objetives;

    [Header("Entrega")]
    public string giverNpcID;
    public string turninNpcID;

    [Header("Requisitos")]
    public List<string> requiredQuestIDs = new List<string>();

    [Header("Diálogos de misión")]
    public string dialogueNpcID;
    public string startDialogue;
    public string turninDialogue;
}
