using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthBar;
    public float Health = 100;

    public void Damage(float value)
    {
        Health -= value;
        healthBar.value = Health / 100f;
    }
}
