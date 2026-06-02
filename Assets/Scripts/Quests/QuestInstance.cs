using System;

public class QuestInstance
{
    public QuestData data;
    public int currentObjective = 0; // Índice del objetivo activo
    public int progress;
    public bool completed;
    public bool justCompleted = false;

    public QuestInstance(QuestData data)
    {
        this.data = data;
        progress = 0;
        completed = false;
        currentObjective = 0;
    }

    public void AddProgress(int cantidad)
    {
        if (completed)
            return;

        // Avanza el objetivo actual
        progress += cantidad;
        var objetivo = data.objetives[currentObjective];
        if (progress >= objetivo.amount)
        {
            progress = 0;
            currentObjective++;

            if (currentObjective >= data.objetives.Length)
            {
                completed = true;
                UnityEngine.Debug.Log("¡Misión completada: " + data.title + "!");
            }
            else
            {
                UnityEngine.Debug.Log(
                    "Objetivo completado, siguiente: "
                        + data.objetives[currentObjective].objectiveID
                );
            }
        }
    }

    public QuestObjective GetCurrentObjective()
    {
        if (completed)
            return null;

        if (data.objetives == null || data.objetives.Length == 0)
            return null;

        if (currentObjective >= data.objetives.Length)
            return null;

        return data.objetives[currentObjective];
    }
}
