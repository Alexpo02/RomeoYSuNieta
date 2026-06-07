using UnityEngine;
using UnityEngine.UI;

public class PaperUI : MonoBehaviour
{
    [Header("Panel")]
    public GameObject paperPanel;

    [SerializeField]
    private Button closeButton;

    private void Awake()
    {
        // Solo un registro, quita la referencia del OnClick en el Inspector
        if (closeButton != null)
            closeButton.onClick.AddListener(Hide);
    }

    private void Start()
    {
        Hide();
    }

    public void Show()
    {
        paperPanel.SetActive(true);
        // NO toques el closeButton aquí, ya está activo dentro del panel
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Hide()
    {
        paperPanel.SetActive(false);
        // NO desactives el closeButton por separado
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
