
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;



public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private GameObject main;
    [SerializeField] private Dropdown resolutionDrD, graphicsDrD;
    [SerializeField] private Toggle fullscreenTgl;
    [SerializeField] private Slider volumeSld;
    [SerializeField] private AudioMixer audioMixer;
    private Resolution[] resolutions;


    // Start is called before the first frame update.
    private void Start ()
    {
        print (PlayerPrefs.GetInt ("resolutionW"));
        resolutions = Screen.resolutions;

        string option;

        int resolutionIndex = 0;
        List<string> resolutionOptions = new List<string> ();

        resolutionDrD.ClearOptions ();

        for (int i = 0; i < resolutions.Length; i += 1)
        {
            option = resolutions[i].width + "x" + resolutions[i].height;
            if (resolutions[i].width == PlayerPrefs.GetInt ("resolutionW") && resolutions[i].height == PlayerPrefs.GetInt ("resolutionH"))
            {
                resolutionIndex = i;
            }

            resolutionOptions.Add (option);
        }
        resolutionDrD.AddOptions (resolutionOptions);

        resolutionDrD.value = resolutionIndex;
        fullscreenTgl.isOn = PlayerPrefs.GetString ("fullscreen") == "true";

        resolutionDrD.RefreshShownValue ();
    }


    // Update is called once per frame.
    private void Update ()
    {
        if (Input.GetKeyDown (KeyCode.Escape) == true || Input.GetKeyDown (KeyCode.Backspace) == true)
        {
            BackToMain ();
        }
    }


    // Deactivate the options menu and go back to the main menu.
    public void BackToMain ()
    {
        main.SetActive (true);
        this.gameObject.SetActive (false);
    }


    // Once the value on the resolution dropdown is modified, the same will happen for the resolution of the game.
    public void SetResolution ()
    {
        Resolution resolution = resolutions[resolutionDrD.value];

        Screen.SetResolution (resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt ("resolutionW", resolution.width);
        PlayerPrefs.SetInt ("resolutionH", resolution.height);
    }


    // We enable fullscreen mode or not according to the fullscreen toogle value.
    public void SetFullscreen ()
    {
        Screen.fullScreen = fullscreenTgl.isOn;
        PlayerPrefs.SetString ("fullscreen", fullscreenTgl.isOn.ToString ());
    }
    
    
    // Once the value on the graphics dropdown is modified, the same will happen for the graphical settings of the game.
    public void SetGraphics ()
    {
        QualitySettings.SetQualityLevel (graphicsDrD.value);
        PlayerPrefs.SetInt ("graphics", graphicsDrD.value);
    }


    // Once the value on the volume slider is modified, the same will happen for the volume of the game.
    public void SetVolume ()
    {
        audioMixer.SetFloat ("volume", volumeSld.value);

        PlayerPrefs.SetFloat ("volume", volumeSld.value);
    }
}