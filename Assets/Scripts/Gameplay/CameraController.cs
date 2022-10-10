using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraController : MonoDodge
{
    public float maxDistance = 20f;
    public float padding = 0.1f;
    public float hightPerDistance = 0.1f;
    public LayerMask solidLayer;
    public GameObject firstPivot;
    public GameObject secondPivot;
    public GameObject target;
    public float sensitifity = 30f;
    public float hightMultiplier = 2f;
    public float baseHeight = 0.5f;

    private float currentYRot = 180f;
    private float goalDst = 0f;
    public float playerZoom = 0f;

    public bool requireMouseForRotate = false;

    public override void Update()
    {
        if (!requireMouseForRotate || Input.GetMouseButton(1))
        {
            currentYRot += Input.GetAxis("MouseX") * sensitifity * 100f * Time.deltaTime;
        }

        playerZoom += Input.mouseScrollDelta.y;
        playerZoom = Mathf.Max(playerZoom, 0);

        var rotation = firstPivot.transform.rotation;
        rotation = Quaternion.Slerp(rotation,Quaternion.Euler(0,currentYRot,0), 10f * Time.deltaTime);
        firstPivot.transform.rotation = rotation;
        float diagonalMaxDist = maxDistance;
        goalDst = diagonalMaxDist - padding;
        float finalDst = Mathf.Max(goalDst - playerZoom,1f);
        Vector3 goalpos = rotation * new Vector3(0, Mathf.Log10(finalDst * hightPerDistance) * hightMultiplier + baseHeight, finalDst);
        if (Physics.Raycast(firstPivot.transform.position,
                goalpos.normalized, out RaycastHit hit, maxDistance, solidLayer))
        {
            Vector3 clippedPos = hit.point - (padding * goalpos).normalized;
            if(Vector3.Distance(firstPivot.transform.position,goalpos) > Vector3.Distance(firstPivot.transform.position,clippedPos))
                secondPivot.transform.position = Vector3.Lerp(secondPivot.transform.position,clippedPos, 10f * Time.deltaTime);
        }
        else
        {
            secondPivot.transform.position = Vector3.Lerp(secondPivot.transform.position,firstPivot.transform.position + goalpos, 30f * Time.deltaTime);
        }
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation((target.transform.position - transform.position).normalized),30f * Time.deltaTime);
        
    }
}
