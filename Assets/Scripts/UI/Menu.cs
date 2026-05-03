using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject menuPanel;

    [Header("Input Settings")]
    [SerializeField] private InputActionReference toggleMenuAction;

    private bool isPaused = false;

    private void OnEnable()
    {
        toggleMenuAction.action.Enable();
        toggleMenuAction.action.performed += OnToggleMenu;
    }

    private void OnDisable()
    {
        toggleMenuAction.action.performed -= OnToggleMenu;
    }

    private void Start()
    {
        ResumeGame();
    }

    private void OnToggleMenu(InputAction.CallbackContext context)
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        isPaused = true;
        menuPanel.SetActive(true);
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void QuitGame()
    {
        Application.Quit();
    }   

    public void ResumeGame()
    {
        isPaused = false;
        menuPanel.SetActive(false);
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}