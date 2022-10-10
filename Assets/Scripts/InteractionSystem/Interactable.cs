using System;
using System.Collections.Generic;
using UnityEngine;

public enum InteractionType
{
    Collision,
    CollisionStart,
    CollisionEnd
}

public static class InteractableCollisionManager { public static List<Collider> colliders = new List<Collider>(); }

public class Interactable : MonoDodge
{
    public EditorLabel Listens_to_types_DOTDOT; //Please do not mind this ugly hacky alternative to [Header("text")]
    public bool collisions = false; //This selection is for performance reasons, so that the collision does not have to run when it isn't used.
    
    private readonly Dictionary<InteractionType, List<InteractionListener>> listenerDatabase = new Dictionary<InteractionType, List<InteractionListener>>();
    private bool isInitialized = false;
    
    //Variables for collision
    private Collider myCollider;
    private List<Collider> currentlyCollidingWith = new List<Collider>();

    public override void Awake()
    {
        if(!isInitialized) Initialize();
    }

    public void Initialize()
    {
        isInitialized = true;
        foreach (InteractionType val in Enum.GetValues(typeof(InteractionType)))
        {
            listenerDatabase.Add(val,new List<InteractionListener>());
        }
        if (collisions)
        {
            myCollider = GetComponent<Collider>();
            InteractableCollisionManager.colliders.Add(myCollider);
        }
    }

    public void RegisterListener(InteractionType type, InteractionListener il)
    {
        if(!isInitialized) Initialize();
        if (listenerDatabase.ContainsKey(type))
        {
            listenerDatabase[type].Add(il);
        }
        else
        {
            Debug.LogError("Please register listeners in the start function, not in the awake function!");
        }
    }

    public override void Update()
    {
        #region CollsionCode
        if(collisions)
            foreach (Collider c in InteractableCollisionManager.colliders) //Loop through all colliders once
            {
                if(c != myCollider && c.enabled && c.gameObject.activeSelf && !currentlyCollidingWith.Contains(c)) //Collider is not colliding already and its active?
                    if (myCollider.bounds.Intersects(c.bounds))
                    {
                        foreach(InteractionListener li in listenerDatabase[InteractionType.CollisionStart])
                            li.OnInteraction(new CollisionStartContext(c.gameObject.GetDodge<Interactable>(),c));
                        currentlyCollidingWith.Add(c);
                    }
                if (currentlyCollidingWith.Contains(c)) //Collider is currently colliding
                {
                    if (!(c == myCollider || !c.enabled || !c.gameObject.activeSelf ||
                        !myCollider.bounds.Intersects(c.bounds))) //Collider is still colliding and active?
                    {
                        foreach (InteractionListener li in listenerDatabase[InteractionType.Collision])
                            li.OnInteraction(new CollisionContext(c.gameObject.GetDodge<Interactable>(), c));
                    }
                    else
                    {
                        foreach (InteractionListener li in listenerDatabase[InteractionType.CollisionEnd])
                            li.OnInteraction(new CollisionEndContext(c.gameObject.GetDodge<Interactable>(), c));
                        currentlyCollidingWith.Remove(c);
                    }
                }
            }
        #endregion
    }
}
