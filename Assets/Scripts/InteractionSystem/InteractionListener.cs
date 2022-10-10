using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionListener
{
    public Action<InteractionContext> OnInteraction { get; private set; }
    public InteractionListener SetOnInteraction<T>(Action<T> handler) where T : InteractionContext
    {
        OnInteraction = (InteractionContext ic) => handler.Invoke((T)ic); //This cast rebind allows us to store all events in one generic listener.
        return this;
    }
}
