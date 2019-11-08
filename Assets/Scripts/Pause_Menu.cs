using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Pause_Menu : MonoBehaviour
{   
    public static Pause_Menu _instance;
    // Start is called before the first frame update
    private void Awake()
    {
        _instance = this;
        _instance.Resume();
    }
    public static void Pause()
    {
        _instance.gameObject.SetActive(true);
    }
    public void Resume()
    {
        print("Resume");
        _instance.gameObject.SetActive(false);
    }
    public void QuitToMainMenu()
    {
        new Confirm_Panel("Are you sure you want to quit to the main menu? <color=red>All unsaved progress will be lost</color>", ConfirmQuit);
    }
    public void ConfirmQuit()
    {
        SceneManager.LoadScene("Main Menu");
    }
    public void QuitGame()
    {
        new Confirm_Panel("Are you sure you want to exit the application? <color=red>All unsaved progress will be lost</color>", Application.Quit);
    }



    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Resume();
        }
    }

}
