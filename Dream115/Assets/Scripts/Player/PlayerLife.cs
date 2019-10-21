
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class PlayerLife : MonoBehaviour
{
    //Para el Singletone
    public static PlayerLife Instance;
    public Slider healthBar;
    public GameObject canvas;

    private bool isAnimationDamageCoroutineRunning;
    private float actualLife;
    private CharacterController characterCtr;
    private Animator[] animators;    
    private Animator fadeAnimator;
    private Scene scene;
    private Transform cameraTrf;

    private float maxLife = 100f;
    private bool die = false; //para que no este todo el rato muriendo


    
    private void Awake ()
    {
        Instance = this;
    }


    // Start is called before the first frame update
    private void Start ()
    {
        actualLife = maxLife;
        characterCtr = this.GetComponent<CharacterController> ();
        animators = this.GetComponentsInChildren<Animator> ();
        fadeAnimator = canvas.GetComponentInChildren<Animator> ();
        scene = SceneManager.GetActiveScene ();
        cameraTrf = Camera.main.transform;
    }


    // Update is called once per frame.
    private void Update ()
    {
        healthBar.transform.LookAt (cameraTrf);
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
        if (dead == true && die == false)
        {
            die = true;
            animators[0].SetTrigger ("Dead");
            animators[1].SetTrigger ("Dead");
            fadeAnimator.SetTrigger ("Dead");
            StartCoroutine (PlayerDead ());
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


    public void TakeDamage (float value)
    {
        DamageAnimation (healthBar.value * PlayerStats.Health - value <= 0);

        if(isAnimationDamageCoroutineRunning == false)
        {
             StartCoroutine (HealthBarAnimationDamage (value));
        }
    }


    IEnumerator HealthBarAnimationDamage (float value)
    {
        isAnimationDamageCoroutineRunning = true;
        float damageAux = value; //El daño que le quita
        float animationSpeed = 20.0f;
        
        float initialHealth = healthBar.value;

        while(damageAux > 0)
        {
            float tick = animationSpeed * Time.fixedDeltaTime;

            damageAux -= tick;
            healthBar.value = (healthBar.value * PlayerStats.Health - tick) / PlayerStats.Health;

            yield return new WaitForFixedUpdate();
        }

        healthBar.value = (initialHealth * PlayerStats.Health - value) / PlayerStats.Health;

        if (healthBar.value <= 0)
        {
            DamageAnimation (true);
        }

        isAnimationDamageCoroutineRunning = false;
    }
}