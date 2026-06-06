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
        Hide();
        if (closeButton != null)
            closeButton.onClick.AddListener(Hide);
    }

    public void Show()
    {
        paperPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Hide()
    {
        paperPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
