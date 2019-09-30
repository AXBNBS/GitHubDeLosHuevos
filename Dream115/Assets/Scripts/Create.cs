using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Create : InteractableItemBase
{
    private bool bebido = false;

    public override void OnInteract()
    {
        InteractText = "Press F to drink";
    }
}
