using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is used to manage the platform at the end
/// </summary>
public class Button : MonoDodge
{
    //Quick and easy solution, the animator!
    public Animator theAnimator;
    public string toggleParam = "UpDown";

    private Interactable button;

    //Setup for the interactions between the interactables and the button itself
    public override void Start()
    {
        button = GetDodge<Interactable>();
        InteractionListener IL = new InteractionListener();
        InteractionListener IL2 = new InteractionListener();
        IL.SetOnInteraction<CollisionStartContext>(Activate);
        IL2.SetOnInteraction<CollisionEndContext>(Deactivate);
        button.RegisterListener(InteractionType.CollisionStart, IL);
        button.RegisterListener(InteractionType.CollisionEnd, IL2);
    }

    //Manages the up an down of the platform
    public void Activate(CollisionStartContext cont) 
    {
        theAnimator.SetBool(toggleParam, true);
    }

    public void Deactivate(CollisionEndContext cont) 
    {
        theAnimator.SetBool(toggleParam, false);
    }
}
