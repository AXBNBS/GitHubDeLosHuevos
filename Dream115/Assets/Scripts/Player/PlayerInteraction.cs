
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class PlayerInteraction : MonoBehaviour
{
    public string[] text;
    //public TextElement shownText;
    [SerializeField] private Text shownText;


    // Start is called before the first frame update.
    private void Start ()
    {
        //textPan.GetComponent<TextElement> ();
    }


    // Update is called once per frame.
    private void Update ()
    {
        if (Input.GetButtonDown ("Interact") == true)
        {
            /*if ()
            {

            }
            else
            {

            }*/
        }

        if (text != null)
        {
            shownText.text = text[0];
        }
    }
}