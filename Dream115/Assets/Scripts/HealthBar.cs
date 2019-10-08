using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Scrollbar healthBar;
    public float Health = 100;

    public void Damage(float value)
    {
        Health -= value;
        healthBar.size = Health / 100f;
    }

}
