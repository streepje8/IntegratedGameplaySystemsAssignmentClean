using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionStartContext : CollisionContext
{
    public CollisionStartContext(Interactable interactable, Collider collider) : base(interactable, collider) { }
}
