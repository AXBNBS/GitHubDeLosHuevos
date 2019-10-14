using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    
    private int regalosRecogidos;
    public int regalosACoger = 1;

    public const float Health = 100; //Vida del personaje

    string sceneName;

    public bool playerInvisible;
    public static PlayerStats Instance;

    public int RegalosRecogidos
    {
        get
        {
            return regalosRecogidos;
        }

        set
        {
            regalosRecogidos = value;
            //Actualizar canvas con .text
            if (regalosRecogidos == regalosACoger)
            {
                switch (sceneName)
                {
                    case "Level1":
                        SceneManager.LoadScene("Level2");
                        break;
                }
                
            }
        }
    }


    private void Awake ()
    {
        Instance = this;
    }


    // Start is called before the first frame update.
    private void Start ()
    {
        sceneName = SceneManager.GetActiveScene().name;
    }


    // Update is called once per frame.
    private void Update ()
    {
    }
}
