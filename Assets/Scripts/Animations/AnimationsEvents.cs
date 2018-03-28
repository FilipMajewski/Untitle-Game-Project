using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationsEvents : MonoBehaviour
{

    Interaction interaction;

    private void Start()
    {
        interaction = GetComponentInParent<Interaction>();
    }

    public void AEFootR()
    {
    }
    public void AEFootL()
    {
    }
    public void AELand()
    {
    }

    public void AEPickup()
    {
        //interaction.Pickup();
    }
}
