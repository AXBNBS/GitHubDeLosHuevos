using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool Paused = false;

    [SerializeField] private GameObject pauseMenuUI, options;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Dropdown resolutionDrD, graphicsDrD;
    [SerializeField] private Toggle fullscreenTgl;
    [SerializeField] private Slider volumeSld;
    private float volume;


    // Start is called before the first frame update.
    private void Start ()
    {
        audioMixer.GetFloat ("volume", out volume);

        graphicsDrD.value = QualitySettings.GetQualityLevel ();
        fullscreenTgl.isOn = Screen.fullScreen;
        volumeSld.value = volume;

        pauseMenuUI.SetActive (false);
        options.SetActive (false);
    }


    // Update is called once per frame.
    private void Update ()
    {
        if (Input.GetKeyDown (KeyCode.Escape) == true)
        {
            if (Paused == true)
            {
                Resume ();
            }
            else
            {
                Pause ();
            }
        }
    }


    public void Resume ()
    {
        pauseMenuUI.SetActive (false);
        options.SetActive (false);
        Time.timeScale = 1f;
        Paused = false;
    }


    private void Pause ()
    {
        pauseMenuUI.SetActive (true);
        Time.timeScale = 0f;
        Paused = true;
    }


    public void Options ()
    {
        pauseMenuUI.SetActive (false);
        options.SetActive (true);
    }


    public void LoadMenu ()
    {
        SceneManager.LoadScene (0);
    }


    public void Quit ()
    {
        Application.Quit ();
    }
}