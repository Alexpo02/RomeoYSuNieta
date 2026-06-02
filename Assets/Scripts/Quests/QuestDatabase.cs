// QuestDatabase.cs
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestDatabase", menuName = "Quests/Quest Database")]
public class QuestDatabase : ScriptableObject
{
    public List<QuestData> quests;
}
