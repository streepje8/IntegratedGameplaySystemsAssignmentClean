using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class PlayerAnimator : MonoDodge
{
    public GameObject playerController;

    private PlayerController pc;
    private Animator animator;
    private Quaternion rot;

    public override void Start()
    {
        animator = transform.gameObject.GetComponent<Animator>();
        if(playerController == null) Debug.LogWarning("Player Controller Not Set!", gameObject);
        pc = playerController.GetDodge<PlayerController>();
    }

    public override void Update()
    {
        if (pc.MoveDirection != Vector3.zero) rot =  pc.movementRotation * Quaternion.LookRotation(pc.MoveDirection.normalized, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation,rot, 10f * Time.deltaTime);
        animator.SetInteger("State",(int)pc.state);
    }
}
