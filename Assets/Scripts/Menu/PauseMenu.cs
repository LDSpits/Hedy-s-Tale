﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static bool isGamePaused = false;
    public GameObject pauseMenuUI;
    public GameObject settingsMenuUI;
    public GameObject pauseMenuButtonContainer;
    public GameObject confirmationContainer;
    public Animator animator;

    private const string OptionMainMenu = "MainMenu";
    private const string OptionQuitGame = "QuitGame";

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGamePaused) 
                Resume(); 
            else 
                Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(false);
        isGamePaused = false;
        Time.timeScale = 1f;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        pauseMenuButtonContainer.SetActive(true);
        confirmationContainer.SetActive(false);
        isGamePaused = true;
        Time.timeScale = 0f;
    }

    public void LoadSettingsMenu()
    {
        pauseMenuButtonContainer.SetActive(false);
        settingsMenuUI.SetActive(true);
    }

    public void OpenConfirmationMenu(string option)
    {
        pauseMenuButtonContainer.SetActive(false);
        confirmationContainer.SetActive(true);

        GameObject yesButton = confirmationContainer.transform.GetChild(1).gameObject;

        switch (option)
        {
            case OptionMainMenu:
                yesButton.GetComponent<Button>().onClick.AddListener(LoadMainMenu);
                break;
            case OptionQuitGame:
                yesButton.GetComponent<Button>().onClick.AddListener(QuitGame);
                break;
            default:
                Debug.LogError("Your entered option does not match any of the available options!");
                break;
        }
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // Go back to Pause Menu
    public void No()
    {
        GameObject yesButton = confirmationContainer.transform.GetChild(1).gameObject;
        yesButton.GetComponent<Button>().onClick.RemoveAllListeners();

        confirmationContainer.SetActive(false);
        pauseMenuButtonContainer.SetActive(true);
    }
}
