
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerStats : MonoBehaviour
{
    public int RegalosRecogidos;
    public int RegalosACoger = 1;
    public const float Health = 99; //Vida del personaje
    public bool playerInvisible;
    public static PlayerStats Instance;

    private string sceneName;


    private void Awake ()
    {
        Instance = this;
    }
}