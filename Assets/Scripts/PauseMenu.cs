using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    public GameObject HUDGUI;
    public Worker worker;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        HUDGUI.SetActive(true);
        Time.timeScale = 1f;
        GameIsPaused = false;

        // Set cursor lock state to locked when resuming
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        HUDGUI.SetActive(false);
        Time.timeScale = 0f;
        GameIsPaused = true;

        // Set cursor lock state to none when paused
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Easy()
    {
        worker.chasingSpeed = 1.5f;
        worker.detectionRadius = 5f;
        worker.chaseRadius = 10f;
    }

    public void Medium()
    {
        worker.chasingSpeed = 2f;
        worker.detectionRadius = 7.5f;
        worker.chaseRadius = 12.5f;
    }

    public void Hard()
    {
        worker.chasingSpeed = 2.5f;
        worker.detectionRadius = 10f;
        worker.chaseRadius = 15f;
    }
}
