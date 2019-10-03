using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLife : MonoBehaviour
{
    private float actualLife;
    private float maxLife = 100f;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        actualLife = maxLife;
        animator = this.GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage)
    {
        actualLife = Mathf.Clamp(actualLife - damage, 0f, maxLife);

        if (actualLife < 0.5f)
        {
            animator.SetTrigger("Dead");
        } else
        {
            animator.SetTrigger("Damaged");
        }
    }
}
