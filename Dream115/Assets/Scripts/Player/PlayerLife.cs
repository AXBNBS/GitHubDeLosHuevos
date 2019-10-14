﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerLife : MonoBehaviour
{
    //Para el Singletone
    public static PlayerLife Instance;

    bool isAnimationDamageCoroutineRunning;

    //Para la pérdida de vida
    public Scrollbar healthBar;

    private float actualLife;
    private float maxLife = 100f;

    private CharacterController characterCtr;
    private Animator[] animators;

    public GameObject canvas;
    private Animator fadeAnimator;

    private Scene scene;

    // Start is called before the first frame update
    void Start()
    {
        actualLife = maxLife;
        characterCtr = this.GetComponent<CharacterController>();
        animators = this.GetComponentsInChildren<Animator> ();

        fadeAnimator = canvas.GetComponentInChildren<Animator>();

        scene = SceneManager.GetActiveScene();
    }

    private void Awake()
    {
        Instance = this;
    }

    /*
    public void TakeDamage (float damage) //Cuando recibes daño
    {
        actualLife = Mathf.Clamp(actualLife - damage, 0f, maxLife); //Controlas que se mantenga la vida entre los limites

        if (actualLife < 0.5f) //Si mueres
        {
            DamageAnimation (true); //Haces la animacion de morir

            characterCtr.enabled = false; //Desactivas los controles
            fadeAnimator.SetTrigger("Dead");
            StartCoroutine(PlayerDead());
        } else //Si no mueres
        {
            DamageAnimation (false); //Haces la animacion de recibir daño
        }
    }
    */

    private void RestartScene ()
    {
        SceneManager.LoadScene (scene.name);
    }


    // We animate the player taking into account the active model and if it has run out of HP or not.
    private void DamageAnimation (bool dead) 
    {
        int activeModel;

        /*if (animators[0].gameObject.activeSelf == false)
        {
            activeModel = 1;
        }
        else 
        {
            activeModel = 0;
        }*/

        if (dead == true)
        {
            animators[0].SetTrigger ("Dead");
            animators[1].SetTrigger ("Dead");
        }
        else 
        {
            animators[0].SetTrigger ("Damaged");
            animators[1].SetTrigger ("Damaged");
        }
    }


    IEnumerator PlayerDead ()
    {
        yield return new WaitForSeconds (3f);

        RestartScene ();
    }

    public void TakeDamage(float value)
    {
        DamageAnimation(healthBar.size * PlayerStats.Health - value <= 0);

        if(isAnimationDamageCoroutineRunning == false)
        {
             StartCoroutine(HealthBarAnimationDamage(value));
        }

        


    }



    IEnumerator HealthBarAnimationDamage(float value)
    {
        isAnimationDamageCoroutineRunning = true;
        float damageAux = value; //El daño que le quita
        float animationSpeed = 20.0f;
        
        float initialHealth = healthBar.size;

        while(damageAux > 0)
        {
            float tick = animationSpeed * Time.fixedDeltaTime;

            damageAux -= tick;
            healthBar.size = (healthBar.size * PlayerStats.Health - tick) / PlayerStats.Health;

            yield return new WaitForFixedUpdate();
        }

        healthBar.size = (initialHealth * PlayerStats.Health - value) / PlayerStats.Health;

        if (healthBar.size <= 0)
        {
            DamageAnimation(true);
        }

        isAnimationDamageCoroutineRunning = false;
    }
}
