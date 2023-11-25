using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    bool settingsread;
    public Slider soundv;
    public Slider musicv;
    public TMPro.TMP_InputField playerinput;
    public GameManager gameManager;
    void Start()
    {
        settingsread = false;
    }

    void Update()
    {
        if (!settingsread)
        {
            if (gameManager.settingsread)
            {
                settingsread = true;
                soundv.value = gameManager.getSoundVolume();
                musicv.value = gameManager.getMusicVolume();
                playerinput.text = gameManager.getPlayername();
            }
        }
    }
    public void PlayGame()
    {
        SceneManager.LoadScene("sandbox_test");
    }

    public void QuitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }
}
