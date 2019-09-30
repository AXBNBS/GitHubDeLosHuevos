
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class PlayerInteraction : MonoBehaviour
{
    //Maria
    public static PlayerInteraction Instance;

    public string[] text;
    //public TextElement shownText;
    [SerializeField] private Text shownText;

    //Variables Maria
    public GameObject player3DModel;

    private void Awake()
    {
        Instance = this; //Se guarda la instancia de esto
    }

    // Start is called before the first frame update.
    private void Start ()
    {
        //textPan.GetComponent<TextElement> ();
    }


    // Update is called once per frame.
    private void Update ()
    {/*
        if (Input.GetButtonDown ("Interact") == true)
        {
            /*if ()
            {

            }
            else
            {

            }
        }

        if (text != null)
        {
            shownText.text = text[0];
        }*/
    }

    public void DalsyCatched()
    {
        //Llama a la corrutina
        StartCoroutine(InvisibilizePlayer());
    }

    //Corrutina
    IEnumerator InvisibilizePlayer()
    {
        float timeInvisible = 5.0f;
        int parpadeosAux = 0;
        int numeroParpadeos = 10;

        //playerInvisible = true; //Para los enemigos

        while(parpadeosAux < numeroParpadeos)
        {
            player3DModel.SetActive(parpadeosAux%2 == 0);

            yield return new WaitForSeconds(timeInvisible/numeroParpadeos);

            parpadeosAux++;

        }

        //playerInvisible = false; //Para los enemigos

        player3DModel.SetActive(true);
    }

}