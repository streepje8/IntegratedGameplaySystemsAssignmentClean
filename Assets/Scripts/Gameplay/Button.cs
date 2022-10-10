using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoDodge
{
    private Interactable buttonmaybe;

    public Animator Animatorthing;

    // Start is called before the first frame update
    public override void Start()
    {
        buttonmaybe = GetDodge<Interactable>();
        InteractionListener IL = new InteractionListener();
        InteractionListener IL2 = new InteractionListener();
        IL.SetOnInteraction<CollisionStartContext>(Activate);
        IL2.SetOnInteraction<CollisionEndContext>(Deactivate);
        buttonmaybe.RegisterListener(InteractionType.CollisionStart, IL);
        buttonmaybe.RegisterListener(InteractionType.CollisionEnd, IL2);
    }

    public void Activate(CollisionStartContext cont) {
        Animatorthing.SetBool("UpDown", true);
        Debug.Log("wee");
    }

    public void Deactivate(CollisionEndContext cont) {
        Animatorthing.SetBool("UpDown", false);
        Debug.Log("woo");
    }



}
