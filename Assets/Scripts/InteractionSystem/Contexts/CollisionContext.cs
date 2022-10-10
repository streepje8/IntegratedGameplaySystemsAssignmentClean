using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionContext : InteractionContext
{
    public readonly Interactable interactable;
    public readonly Collider collider;
    public CollisionContext(Interactable interactable, Collider collider)
    {
        this.interactable = interactable;
        this.collider = collider;
    }
}
