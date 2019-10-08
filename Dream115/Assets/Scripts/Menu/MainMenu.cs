
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject options;


    // Start is called before the first frame update.
    private void Start ()
    {
    }


    // Update is called once per frame.
    private void Update ()
    {
    }


    // The first level of the game will be loaded.
    public void Play ()
    {
        SceneManager.LoadScene (1);
    }


    // The main menu will be hidden and the different available settings will be shown.
    public void Options ()
    {
        options.SetActive (true);
        this.gameObject.SetActive (false);
    }


    // The game will be closed.
    public void Quit ()
    {
        print ("Closing game.");
        Application.Quit ();
    }
}