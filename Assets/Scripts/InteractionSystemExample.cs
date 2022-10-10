using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionSystemExample : MonoDodge
{
    private Interactable me;

    public override void Start()
    {
        //Inline Registratie
        me = GetDodge<Interactable>();
        me.RegisterListener(InteractionType.Collision, new InteractionListener()
            .SetOnInteraction((CollisionContext cc) =>
            {
                Debug.Log("Just had an epic collision with: " + cc.interactable.gameObject.name, cc.collider.gameObject);
            }));


        //Niet Inline Registratie
        me = GetDodge<Interactable>();
        InteractionListener listener = new InteractionListener();
        listener.SetOnInteraction<CollisionContext>(OnCollision);
        me.RegisterListener(InteractionType.Collision, listener);
    }

    public void OnCollision(CollisionContext cc)
    {
        Debug.Log("Just had an epic collision with: " + cc.interactable.gameObject.name, cc.collider.gameObject);
    }
}
