using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Body : MonoDodge 
{
    private float _gravityTime = 25f;
    private float _currentTime = 0f;
    private float _gravity = 0f;
    private bool _isGrounded = false;
    private bool _isActive = false;
    public LayerMask enviroment;
    public float groundCheckDistance = 0.02f;
    public ParticleSystem particles;
    public Animator fade; 

    public void Spawn(Transform target) 
    {
        _gravity = 0f;
        transform.position = target.position;
        transform.rotation = target.rotation;
        particles.Play();
        fade.Play(0);
        _currentTime = 0f;

        //Check if already on ground wooww, problematic when spawning it on a floating statue hmmm...
        //oh well, prototype go brr :D
        _isGrounded = Physics.Raycast(transform.position, -Vector3.up, 0.5f, enviroment);
        _isActive = true;
    }

    public override void Update() 
    {
        if (!_isGrounded && _isActive) 
        {
            _isGrounded = Physics.SphereCast(new Ray(transform.position, Vector3.down), 0.2f, groundCheckDistance, enviroment);
            _currentTime += 1 * Time.deltaTime;

            _isActive = !_isGrounded;

            if (_currentTime >= _gravityTime) 
            {
                transform.position += new Vector3(0, _gravity, 0);
                _gravity -= 0.01f * Time.deltaTime;
            }
        }
    }
}
