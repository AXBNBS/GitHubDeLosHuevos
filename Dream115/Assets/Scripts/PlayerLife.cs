using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLife : MonoBehaviour
{
    private float actualLife;
    private float maxLife = 100f;

    private CharacterController characterCtr;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        actualLife = maxLife;
        characterCtr = this.GetComponent<CharacterController>();
        animator = this.GetComponentInChildren<Animator>();
    }

    public void TakeDamage(float damage) //Cuando recibes daño
    {
        actualLife = Mathf.Clamp(actualLife - damage, 0f, maxLife); //Controlas que se mantenga la vida entre los limites

        if (actualLife < 0.5f) //Si mueres
        {
            animator.SetTrigger("Dead"); //Haces la animacion de morir
            characterCtr.enabled = false; //Desactivas los controles
        } else //Si no mueres
        {
            animator.SetTrigger("Damaged"); //Haces la animacion de recibir daño
        }
    }
}
