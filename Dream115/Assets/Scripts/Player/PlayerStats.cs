using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    
    public int RegalosRecogidos;
    public int RegalosACoger = 1;

    public const float Health = 100; //Vida del personaje

    string sceneName;

    public bool playerInvisible;
    public static PlayerStats Instance;

    private void Awake ()
    {
        Instance = this;
    }
}
