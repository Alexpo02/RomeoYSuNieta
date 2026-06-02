using System.Text;
using TMPro;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    public TextMeshProUGUI questText;

    void Start()
    {
        //UpdateUI();
    }

    void Update() { }

    void UpdateUI()
    {
        // Guardia por si QuestManager aún no existe
        if (QuestManager.Instance == null)
        {
            Debug.Log("[QuestUI] QuestManager.Instance es null en UpdateUI");
            return;
        }
        if (questText == null)
            return;

        var quests = QuestManager.Instance.GetActiveQuests();
        StringBuilder sb = new StringBuilder();
        foreach (var quest in quests)
        {
            sb.AppendLine($"<color=yellow><b>{quest.data.title}</b></color>\n");
            if (quest.completed)
            {
                sb.AppendLine("<color=green>COMPLETADA</color>\n");
                continue;
            }
            var obj = quest.GetCurrentObjective();
            Debug.Log(
                $"[QuestUI] Actualizando UI para misión '{quest.data.title}', objetivo actual: {(obj != null ? obj.objetive : "Ninguno")}"
            );
            if (obj != null)
            {
                sb.AppendLine($"{obj.objetive}");
                sb.AppendLine($"Progreso: {quest.progress}/{obj.amount}\n");
            }
        }
        questText.text = sb.ToString();
    }

    private System.Collections.IEnumerator InitWhenReady()
    {
        // Espera hasta que QuestManager exista
        yield return new WaitUntil(() => QuestManager.Instance != null);
        UpdateUI();
    }

    void OnEnable()
    {
        Debug.Log("[QuestUI] OnEnable INICIO");
        QuestManager.OnQuestUpdated += UpdateUI;
        Debug.Log("[QuestUI] Suscrito a OnQuestUpdated");
        UpdateUI();
        Debug.Log("[QuestUI] OnEnable FIN");
        StartCoroutine(InitWhenReady());
    }

    void OnDisable()
    {
        QuestManager.OnQuestUpdated -= UpdateUI;
    }
}
