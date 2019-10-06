
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;



public class SaveLoadSettings : MonoBehaviour
{
    [SerializeField] private Dropdown resolutionDrD, graphicsDrD;
    [SerializeField] private Toggle fullscreenTgl;
    [SerializeField] private Slider volumeSld;
    [SerializeField] private AudioMixer audioMixer;


    // Start is called before the first frame update.
    private void Start ()
    {
        resolutionDrD.transform.parent.gameObject.SetActive (false);

        print (PlayerPrefs.HasKey ("resolutionW"));
        if (PlayerPrefs.HasKey ("resolutionW") == false)
        {
            audioMixer.GetFloat ("volume", out float volume);

            PlayerPrefs.SetInt ("resolutionW", Screen.currentResolution.width);
            PlayerPrefs.SetInt ("resolutionH", Screen.currentResolution.height);
            PlayerPrefs.SetString ("fullscreen", Screen.fullScreen.ToString ());
            PlayerPrefs.SetInt ("graphics", QualitySettings.GetQualityLevel ());
            PlayerPrefs.SetFloat ("volume", volume);

            if (PlayerPrefs.GetString ("fullscreen") != fullscreenTgl.isOn.ToString ())
            {
                fullscreenTgl.isOn = !fullscreenTgl.isOn;
            }
            if (PlayerPrefs.GetInt ("graphics") != graphicsDrD.value)
            {
                graphicsDrD.value = PlayerPrefs.GetInt ("graphics");
            }
            if (PlayerPrefs.GetFloat ("volume") != volumeSld.value)
            {
                volumeSld.value = PlayerPrefs.GetFloat ("volume");
            }
        }
        else
        {
            Resolution savedRes = new Resolution ();
            Resolution[] resolutions = Screen.resolutions;
            int indexRes = -1;

            savedRes.width = PlayerPrefs.GetInt ("resolutionW");
            savedRes.height = PlayerPrefs.GetInt ("resolutionH");
            for (int i = 0; i < resolutions.Length; i += 1)
            {
                if (resolutions[i].width == savedRes.width && resolutions[i].height == savedRes.height)
                {
                    indexRes = i;

                    break;
                }
            }

            if (indexRes == -1)
            {
                PlayerPrefs.SetInt ("resolutionW", Screen.currentResolution.width);
                PlayerPrefs.SetInt ("resolutionH", Screen.currentResolution.height);
            }
            else
            {
                Screen.SetResolution (savedRes.width, savedRes.height, PlayerPrefs.GetString ("fullscreen") == "true");
            }

            QualitySettings.SetQualityLevel (PlayerPrefs.GetInt ("graphics"));
            audioMixer.SetFloat ("volume", PlayerPrefs.GetFloat ("volume"));

            print(PlayerPrefs.GetString("fullscreen"));
            fullscreenTgl.isOn = PlayerPrefs.GetString ("fullscreen") == "true";
            print(PlayerPrefs.GetString("fullscreen"));
            graphicsDrD.value = PlayerPrefs.GetInt ("graphics");
            volumeSld.value = PlayerPrefs.GetFloat ("volume");
        }
    }


    // Update is called once per frame.
    private void Update ()
    {
    }
}