
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class PlayerInteraction : MonoBehaviour
{
    public static PlayerInteraction Instance;
    public string[] text;
    public GameObject canvas;
    public Vector3 lastKnownPos;

    [SerializeField] private Text shownText;
    [SerializeField] private CameraMovement cameraRef;
    [SerializeField] private Renderer normalMod, invisibleMod;
    [SerializeField] private int invisibilityTime;
    private GameObject panel, objectToDisable;
    private int activeParagraph, lettersRead;
    private bool paragraphEnd;
    private char currentLetter;
    private NewPlayerMovement movementRef;
    private string sceneName;
    private Animator fadeAnimator;
    private float timeElapsed;


    // Awake is always called before any Start function and after every object has been initialized.
    private void Awake ()
    {
        Instance = this; //Se guarda la instancia de esto
    }


    // Start is called before the first frame update.
    private void Start ()
    {
        text = null;
        shownText.text = "";
        panel = shownText.transform.parent.gameObject;
        objectToDisable = null;
        activeParagraph = 0;
        lettersRead = 0;
        paragraphEnd = false;
        movementRef = this.GetComponent<NewPlayerMovement> ();
        normalMod.enabled = true;
        invisibleMod.enabled = false;
        sceneName = SceneManager.GetActiveScene().name;
        fadeAnimator = canvas.GetComponentInChildren<Animator> ();
    }


    // Update is called once per frame.
    private void Update ()
    {
        timeElapsed += Time.unscaledDeltaTime;

        if (panel.activeSelf == true) 
        {
            if (timeElapsed > 0.03f)
            {
                ShowText ();

                timeElapsed = 0;
            }

            if (Input.GetButtonDown ("Interact") == true)
            {
                if (paragraphEnd == true)
                {
                    shownText.text = "";
                    paragraphEnd = false;
                    activeParagraph += 1;
                    lettersRead = 0;

                    if (activeParagraph == text.Length)
                    {
                        HidePanelText ();
                    }
                }
                else
                {
                    SkipToCompleteText ();
                }
            }
        }
    }


    private void OnTriggerEnter (Collider other)
    {
        switch (other.tag)
        {
            case "LevelChange":
                if (PlayerStats.Instance.RegalosRecogidos == PlayerStats.Instance.RegalosACoger)
                {
                    fadeAnimator.SetTrigger ("Dead");

                    StartCoroutine (ChangeLevel ());
                }

                break;
            case "Interactable":
                cameraRef.allowInput = false;
                movementRef.allowInput = false;
                objectToDisable = other.gameObject;

                movementRef.transform.LookAt (this.transform);
                panel.SetActive (true);

                Time.timeScale = 0;

                break;
        }
        /*if (other.tag == "LevelChange")
        {
            if (PlayerStats.Instance.RegalosRecogidos == PlayerStats.Instance.RegalosACoger)
            {
                fadeAnimator.SetTrigger ("Dead");
                StartCoroutine (ChangeLevel ());
            }
        }*/
    }


    // This function will be called repeatedly while a text box is being shown to the player, making a new letter appear every time it's called (unless the current paragraph has already been completely shown). It will only cease to be called once the 
    //whole text assigned to an interactable object has been shown.
    private void ShowText ()
    {
        if (lettersRead < text[activeParagraph].Length)
        {
            currentLetter = text[activeParagraph][lettersRead];
            if (currentLetter == '*')
            {
                shownText.text += '\n';
            }
            else
            {
                shownText.text += currentLetter;
            }
            lettersRead += 1;
        }
        else
        {
            paragraphEnd = true;
        }
    }


    // We remove the currently shown text and we immediatly show the full text of the paragraph, taking into account the possibility of finding and asterisk, which in this case is used to represent a change of line.
    private void SkipToCompleteText ()
    {
        shownText.text = "";
        foreach (char c in text[activeParagraph])
        {
            if (c == '*')
            {
                shownText.text += '\n';
            }
            else
            {
                shownText.text += c;
            }
        }
        lettersRead = text[activeParagraph].Length;
    }


    public void HidePanelText ()
    {
        shownText.text = "";
        //textEnd = true;
        activeParagraph = 0;
        cameraRef.allowInput = true;
        movementRef.allowInput = true;
        Time.timeScale = 1;

        panel.SetActive (false);
        objectToDisable.SetActive (false);

        objectToDisable = null;
    }


    // We make changes in order to cause the player to start invisibilizing.
    public void DalsyCatched ()
    {
        ChangePlayerVisibility (true);
    }


    // Function that makes the player model interpolate between its normal and transparent versions. When the player is completely invisibilized, a coroutine will be called in order to wait a certain amount of time before making the player become 
    //visible again.
    private void ChangePlayerVisibility (bool invisible)
    {
        if (invisible == false)
        {
            normalMod.enabled = true;
            invisibleMod.enabled = false;
        }
        else 
        {
            invisibleMod.enabled = true;
            normalMod.enabled = false;

            StartCoroutine (RemainInvisible ());
        }

        if (PlayerStats.Instance.playerInvisible != invisible) 
        {
            lastKnownPos = this.transform.position;
            PlayerStats.Instance.playerInvisible = invisible;
        }
    }
    

    // Coroutine that will wait for 5 seconds before making the player start becoming visible again.
    IEnumerator RemainInvisible ()
    {
        yield return new WaitForSeconds (invisibilityTime);

        ChangePlayerVisibility (false);
    }


    IEnumerator ChangeLevel ()
    {
        yield return new WaitForSeconds (3);

        switch (sceneName)
        {
            case "Level1":
                SceneManager.LoadScene ("Level2");

                break;
            case "Level2":
                SceneManager.LoadScene ("Level3");

                break;
            case "Level3":
                SceneManager.LoadScene ("Level4");

                break;
            case "Level4":
                SceneManager.LoadScene ("MainMenu");

                break;
        }
    }
}