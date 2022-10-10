using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BodyManager : MonoDodge 
{
    //Add to list with event
    public List<GameObject> batch = new List<GameObject>();
    public GameObject playerVisual;   
    public GameObject spawn;
    int currentIndex = 0;

    // Update is called once per frame
    public override void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)) 
        {
            SpawnBod();
        }
    }

    public void SpawnBod() 
    {
        currentIndex = currentIndex + 1 < batch.Count ? currentIndex + 1 : 0;

        //Move these steps to body, so that the manager just tells it to move!
        batch[currentIndex].GetDodge<Body>().Spawn(playerVisual.transform);

        //Let the player know it's time to die! Just like me!
        //These are crimes, we do not talk about these, shhhh, don't show Aaron
        playerVisual.transform.parent.gameObject.GetDodge<PlayerController>().Teleport(spawn.transform.position + Vector3.up);
    }
}
