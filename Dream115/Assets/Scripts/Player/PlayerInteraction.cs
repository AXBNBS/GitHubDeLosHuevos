
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class PlayerInteraction : MonoBehaviour
{
    public static PlayerInteraction Instance;
    public string[] text;

    [SerializeField] private Text shownText;
    [SerializeField] private CameraMovement cameraRef;
    [SerializeField] private Renderer normalMod, invisibleMod;
    //[SerializeField] private Renderer normalPlayerRnd, invisiblePlayerRnd, actualPlayerRnd;
    private GameObject panel;
    private int activeParagraph, lettersRead;
    private bool paragraphEnd, textEnd;
    private char currentLetter;
    private NewPlayerMovement movementRef;
    //private Material[] normalMat, invisibleMat, actualMat;


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
        activeParagraph = 0;
        lettersRead = 0;
        paragraphEnd = false;
        textEnd = false;
        movementRef = this.GetComponent<NewPlayerMovement> ();

        normalMod.enabled = true;
        invisibleMod.enabled = false;
    }


    // Update is called once per frame.
    private void Update ()
    {
        if (Input.GetButtonDown("Interact"))
        {
            PlayerLife.Instance.TakeDamage(25.0f);
        }


        /*if (changeVisibility == true)
        {
            ChangePlayerVisibility ();
        }*/

        if (text != null) //Input.GetButtonDown("Interact") == true && 
        {
            if (panel.activeSelf == false)
            {
                //cameraRef.allowInput = false;
                //movementRef.allowInput = false;
                //movementRef.transform.LookAt (this.transform);

                panel.SetActive (true);

                InvokeRepeating ("ShowText", 0f, 0.05f);
            }
            else
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

                    lettersRead = text[activeParagraph].Length;
                }
            }
        }
    }


    public void HidePanelText ()
    {
        shownText.text = "";
        textEnd = true;
        activeParagraph = 0;
        cameraRef.allowInput = true;
        movementRef.allowInput = true;

        panel.SetActive (false);
    }


    // This function will be called repeatedly while a text box is being shown to the player, making a new letter appear every time it's called (unless the current paragraph has already been completely 
    //shown). It will only cease to be called once the whole text assigned to an interactable object has been shown.
    private void ShowText ()
    {
        if (textEnd == false)
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
        else
        {
            textEnd = false;

            CancelInvoke ("ShowText");
        }
    }


    // We remove the currently shown text and we immediatly show the full text of the paragraph, taking into account the possibility of finding and asterisk, which in this case is used to represent a 
    //change of line.
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
    }


    // We make changes in order to cause the player to start invisibilizing.
    public void DalsyCatched ()
    {
        ChangePlayerVisibility (true);
    }


    // Function that makes the player model interpolate between its normal and transparent versions. When the player is completely invisibilized, a coroutine will be called in order to wait a certain 
    //amount of time before making the player become visible again.
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

        PlayerStats.Instance.playerInvisible = invisible;
        /*for (int i = 0; i < actualMat.Length; i += 1)
        {
            if (invisibilize == true)
            {
                actualMat[i].Lerp (actualMat[i], invisibleMat[i], Time.deltaTime);
            }
            else
            {
                actualMat[i].Lerp (actualMat[i], normalMat[i], Time.deltaTime);
            }
            print(actualMat[i].color.a);
        }

        if ((invisibilize == true && actualMat[0].color.a <= 0.55f) || (invisibilize == false && actualMat[0].color.a > 0.95f))
        {
            if (invisibilize == true)
            {
                PlayerStats.Instance.playerInvisible = true;

                StartCoroutine (RemainInvisible ());
            }
            else
            {
                PlayerStats.Instance.playerInvisible = false;
            }

            changeVisibility = false;
        }*/
    }
    

    // Coroutine that will wait for 5 seconds before making the player start becoming visible again.
    IEnumerator RemainInvisible ()
    {
        yield return new WaitForSeconds (5);

        ChangePlayerVisibility (false);
    }
}