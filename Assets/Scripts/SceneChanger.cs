using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour, IInteractuable
{
    [SerializeField]
    [Tooltip("Nombre de la escena de destino.")]
    public string sceneName;

    [SerializeField]
    [Tooltip("ID del SpawnPoint en la escena destino donde aparecerá el jugador.")]
    public string targetSpawnID;

    /*public void ChangeScene()
    {
        // Asegurarse de que existe el SpawnManager antes de cambiar de escena
        if (SpawnManager.Instance == null)
        {
            GameObject go = new GameObject("SpawnManager");
            go.AddComponent<SpawnManager>();
        }

        SpawnManager.Instance.SetTargetSpawn(targetSpawnID);
        SceneManager.LoadScene(sceneName);
    }*/

    public void ChangeScene()
    {
        SceneManager.LoadScene(sceneName);
        // Verificamos que el manager de transición existe
        /*if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.TransitionToScene(sceneName, targetSpawnID);
        }
        else
        {
            // Fail-safe por si olvidas poner el manager en la escena
            if (SpawnManager.Instance != null)
                SpawnManager.Instance.SetTargetSpawn(targetSpawnID);
            
        }*/
    }

    public void GetInteractionText()
    {
        //throw new System.NotImplementedException();
    }

    public void HideText()
    {
        //throw new System.NotImplementedException();
    }

    public void Interact()
    {
        //ChangeScene();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ChangeScene();
        }
    }
}
