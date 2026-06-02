using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [SerializeField]
    private QuestDatabase questDatabase;

    private List<QuestInstance> activeQuests = new List<QuestInstance>();
    private List<string> completedQuestIDs = new List<string>();
    private List<string> savedActiveQuestIDs = new List<string>();

    public static event System.Action OnQuestUpdated;
    public string SaveID => "quests";

    public List<QuestInstance> GetCompletedQuests()
    {
        List<QuestInstance> completed = new List<QuestInstance>();
        foreach (var quest in activeQuests)
        {
            if (quest.completed)
                completed.Add(quest);
        }
        return completed;
    }

    // ─── ISaveable ────────────────────────────────────────────────────────────

    /*public string OnSave()
    {
        QuestSaveData data = new QuestSaveData { completedQuests = completedQuestIDs };

        foreach (var q in activeQuests)
        {
            data.activeQuests.Add(
                new QuestInstanceSaveData
                {
                    id = q.data.id,
                    currentObjective = q.currentObjective,
                    progress = q.progress,
                    completed = q.completed,
                }
            );
        }

        return JsonUtility.ToJson(data);
    }*/

    /*public void OnLoad(string json)
    {
        //  CASO: no hay save (partida nueva)
        if (string.IsNullOrEmpty(json))
        {
            Debug.Log("[QuestManager] Nueva partida - inicializando quests");

            completedQuestIDs = new List<string>();
            activeQuests.Clear();

            InitializeAutoQuestsOnly();

            OnQuestUpdated?.Invoke();
            return;
        }

        //  CASO: hay save
        //QuestSaveData data = JsonUtility.FromJson<QuestSaveData>(json);

        completedQuestIDs = data.completedQuests ?? new List<string>();

        activeQuests.Clear();

        if (data.activeQuests != null)
        {
            foreach (var saved in data.activeQuests)
            {
                var questData = questDatabase.quests.Find(q => q.id == saved.id);
                if (questData == null)
                    continue;

                if (saved.completed)
                    continue;

                QuestInstance instance = new QuestInstance(questData);
                instance.currentObjective = saved.currentObjective;
                instance.progress = saved.progress;
                instance.completed = saved.completed;

                activeQuests.Add(instance);
            }
        }

        InitializeAutoQuestsOnly();

        OnQuestUpdated?.Invoke();
    }*/

    // ─── Inicialización ───────────────────────────────────────────────────────

    /// <summary>
    /// Llamado por SaveManager una vez cargados los datos.
    /// Activa únicamente las misiones automáticas cuyos requisitos ya se cumplen.
    /// </summary>
    public void InitializeQuests()
    {
        activeQuests.Clear();

        foreach (var questData in questDatabase.quests)
        {
            // Solo misiones sin NPC asignado (auto-start)
            if (!string.IsNullOrEmpty(questData.giverNpcID))
                continue;

            TryActivateQuest(questData);
        }

        foreach (var id in savedActiveQuestIDs)
        {
            var questData = questDatabase.quests.Find(q => q.id == id);
            if (questData == null)
                continue;
            if (completedQuestIDs.Contains(id))
                continue;
            if (activeQuests.Exists(q => q.data.id == id))
                continue;

            activeQuests.Add(new QuestInstance(questData));
        }
        savedActiveQuestIDs.Clear();

        OnQuestUpdated?.Invoke();
    }

    private void InitializeAutoQuestsOnly()
    {
        foreach (var questData in questDatabase.quests)
        {
            if (!string.IsNullOrEmpty(questData.giverNpcID))
                continue;

            if (completedQuestIDs.Contains(questData.id))
                continue;

            if (activeQuests.Exists(q => q.data.id == questData.id))
                continue;

            TryActivateQuest(questData);
        }
    }

    // ─── Concesión de misiones por NPC ────────────────────────────────────────

    /// <summary>
    /// Llamado desde NPCInteraction cuando el jugador habla con un NPC.
    /// Activa la primera misión disponible de ese NPC (si la hay).
    /// </summary>
    // Cambia el tipo de retorno de void a QuestInstance
    // para que NPCInteraction sepa si se acaba de activar una misión nueva

    public QuestInstance TryGrantQuests(string npcID)
    {
        foreach (var questData in questDatabase.quests)
        {
            if (questData.giverNpcID != npcID)
                continue;

            if (TryActivateQuest(questData))
            {
                OnQuestUpdated?.Invoke();
                //SaveManager.Instance.Save();
                // Devuelve la instancia recién activada
                return activeQuests.Find(q => q.data.id == questData.id);
            }
        }
        return null;
    }

    /// <summary>
    /// Comprueba si una misión puede activarse y, si es así, la añade a activeQuests.
    /// Retorna true si se activó.
    /// </summary>
    private bool TryActivateQuest(QuestData questData)
    {
        // Ya completada
        if (completedQuestIDs.Contains(questData.id))
            return false;

        // Ya activa
        if (activeQuests.Exists(q => q.data.id == questData.id))
            return false;

        // Requisitos no cumplidos
        foreach (var requiredID in questData.requiredQuestIDs)
        {
            if (!completedQuestIDs.Contains(requiredID))
                return false;
        }

        activeQuests.Add(new QuestInstance(questData));
        Debug.Log($"[QuestManager] Misión activada: {questData.title}");
        return true;
    }

    /// <summary>
    /// Devuelve true si el NPC con ese ID tiene alguna misión disponible para dar.
    /// Útil para el indicador visual "!".
    /// </summary>
    public bool HasAvailableQuestFor(string npcID)
    {
        foreach (var questData in questDatabase.quests)
        {
            if (questData.giverNpcID != npcID)
                continue;

            if (completedQuestIDs.Contains(questData.id))
                continue;

            if (activeQuests.Exists(q => q.data.id == questData.id))
                continue;

            bool requisitesMet = true;
            foreach (var requiredID in questData.requiredQuestIDs)
            {
                if (!completedQuestIDs.Contains(requiredID))
                {
                    requisitesMet = false;
                    break;
                }
            }

            if (requisitesMet)
                return true;
        }

        return false;
    }

    // ─── Unity lifecycle ──────────────────────────────────────────────────────

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        //SaveManager.Instance.Register(this);
    }

    void Start() { }

    void OnEnable()
    {
        /*QuestArea.OnZoneEntered += OnZoneEntered;
        QuestCollectable.OnQuestItemCollected += OnQuestItemCollected;
        QuestPlantable.OnQuestItemPlanted += OnQuestItemPlanted;
        QuestTalkable.OnNPCTalked += OnNPCTalked;
        QuestWaterable.OnQuestItemWatered += OnQuestItemWatered;
        QuestTillable.OnGroundTilled += OnGroundTilled;
        QuestGardenUnlockable.OnGardenUnlockedQuest += OnGardenUnlocked;*/
    }

    void OnDestroy()
    {
        /*SaveManager.Instance?.Unregister(this);
        QuestArea.OnZoneEntered -= OnZoneEntered;
        QuestCollectable.OnQuestItemCollected -= OnQuestItemCollected;
        QuestPlantable.OnQuestItemPlanted -= OnQuestItemPlanted;
        QuestTalkable.OnNPCTalked -= OnNPCTalked;
        QuestWaterable.OnQuestItemWatered -= OnQuestItemWatered;
        QuestTillable.OnGroundTilled -= OnGroundTilled;
        QuestGardenUnlockable.OnGardenUnlockedQuest -= OnGardenUnlocked;*/
    }

    // ─── Progreso ─────────────────────────────────────────────────────────────

    private void OnNPCTalked(QuestType type, string objectiveID) => Progress(type, objectiveID, 1);

    private void OnQuestItemPlanted(QuestType type, string objectiveID) =>
        Progress(type, objectiveID, 1);

    private void OnQuestItemCollected(QuestType type, string objectiveID) =>
        Progress(type, objectiveID, 1);

    private void OnZoneEntered(QuestType type, string objectiveID) =>
        Progress(type, objectiveID, 1);

    private void OnQuestItemWatered(QuestType type, string objectiveID) =>
        Progress(type, objectiveID, 1);

    private void OnGroundTilled(QuestType type, string objectiveID) =>
        Progress(type, objectiveID, 1);

    private void OnGardenUnlocked(QuestType type, string objectiveID) =>
        Progress(type, objectiveID, 1);

    public void Progress(QuestType type, string objectiveID, int cantidad)
    {
        foreach (var quest in activeQuests)
        {
            if (quest.completed)
                continue;

            var objective = quest.GetCurrentObjective();
            if (objective == null || objective.type != type || objective.objectiveID != objectiveID)
                continue;

            quest.AddProgress(cantidad);
            //SaveManager.Instance.Save();

            if (quest.completed)
            {
                if (!completedQuestIDs.Contains(quest.data.id))
                {
                    quest.justCompleted = true;
                    completedQuestIDs.Add(quest.data.id);
                }
                //SaveManager.Instance.Save();
                StartCoroutine(RemoveQuestAfterDelay(quest, 3f));
                TryUnlockAutoQuests();
            }

            OnQuestUpdated?.Invoke();
        }
    }

    /// <summary>
    /// Después de completar una misión, comprueba si alguna misión automática
    /// (sin NPC) tiene ahora sus requisitos cumplidos y la activa.
    /// </summary>
    private void TryUnlockAutoQuests()
    {
        bool anyNew = false;
        foreach (var questData in questDatabase.quests)
        {
            if (!string.IsNullOrEmpty(questData.giverNpcID))
                continue;

            if (TryActivateQuest(questData))
                anyNew = true;
        }

        if (anyNew)
            OnQuestUpdated?.Invoke();
    }

    private System.Collections.IEnumerator RemoveQuestAfterDelay(QuestInstance quest, float delay)
    {
        yield return new WaitForSeconds(delay);
        activeQuests.Remove(quest);
        Debug.Log($"[QuestManager] Misión eliminada: {quest.data.title}");
        OnQuestUpdated?.Invoke();
    }

    public List<QuestInstance> GetActiveQuests() => activeQuests;
}
