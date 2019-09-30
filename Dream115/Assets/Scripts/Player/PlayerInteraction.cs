
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class PlayerInteraction : MonoBehaviour
{
    public static PlayerInteraction Instance;
    public GameObject player3DModel;
    public string[] text;
    [SerializeField] private Text shownText;
    [SerializeField] private CameraMovement cameraRef;
    private GameObject panel;
    private int activeParagraph, lettersRead;
    private bool paragraphEnd, textEnd;
    private char currentLetter;
    private NewPlayerMovement movementRef;


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
    }


    // Update is called once per frame.
    private void Update ()
    {
        if (Input.GetButtonDown ("Interact") == true && text != null)
        {
            if (panel.activeSelf == false)
            {
                cameraRef.allowInput = false;
                movementRef.allowInput = false;
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
                        textEnd = true;
                        activeParagraph = 0;
                        cameraRef.allowInput = true;
                        movementRef.allowInput = true;

                        panel.SetActive (false);
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


    // This function will be called repeatedly while a text box is being shown to the player, making a new letter appear every time it's called (unless the current paragraph has already been completely shown). 
    // It will only cease to be called once the whole text assigned to an interactable object has been shown.
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
    }


    // Llama a la corrutina.
    public void DalsyCatched ()
    {
        StartCoroutine (InvisibilizePlayer ());
    }


    // Corrutina.
    IEnumerator InvisibilizePlayer ()
    {
        float timeInvisible = 5.0f;
        int parpadeosAux = 0;
        int numeroParpadeos = 10;
        //playerInvisible = true; //Para los enemigos

        while (parpadeosAux < numeroParpadeos)
        {
            player3DModel.SetActive (parpadeosAux % 2 == 0);

            yield return new WaitForSeconds (timeInvisible / numeroParpadeos);

            parpadeosAux++;
        }

        //playerInvisible = false; //Para los enemigos

        player3DModel.SetActive (true);
    }
}