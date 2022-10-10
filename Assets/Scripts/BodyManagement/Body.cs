using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

//These are the bodies, they play effects and fall eventually!
public class Body : MonoDodge 
{
    public LayerMask enviroment;
    public float groundCheckDistance = 0.02f;
    public ParticleSystem particles;
    public Animator fade; 

    private float gravityTime = 25f;
    private float currentTime = 0f;
    private float gravity = 0f;
    private bool isGrounded = false;
    private bool isActive = false;

    // When spawning reset all values and check wether it needs to start falling eventually or not
    public void Spawn(Transform target) 
    {
        gravity = 0f;
        transform.position = target.position;
        transform.rotation = target.rotation;
        particles.Play();
        fade.Play(0);
        currentTime = 0f;

        //Problematic when spawning on top of another statue as that will eventually fall and this won't
        //It's a small detail but this is better on performance like this,
        //Maybe a re-check event could work? But it's fine for a prototype! :D
        isGrounded = Physics.Raycast(transform.position, -Vector3.up, 0.5f, enviroment);
        isActive = true;
    }

    // Check whether has started and not on the ground in order to check the floating time
    public override void Update() 
    {
        if (!isGrounded && isActive) 
        {
            isGrounded = Physics.SphereCast(new Ray(transform.position, Vector3.down), 0.2f, groundCheckDistance, enviroment);
            currentTime += 1 * Time.deltaTime;

            isActive = !isGrounded;

            if (currentTime >= gravityTime) 
            {
                transform.position += new Vector3(0, gravity, 0);
                gravity -= 0.01f * Time.deltaTime;
            }
        }
    }
}
