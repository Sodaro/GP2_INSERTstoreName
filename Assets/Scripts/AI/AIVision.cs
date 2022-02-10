using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIVision : MonoBehaviour
{
    [Header("Sight")]
    [SerializeField] private Transform sightPoint; // If you want the raycast to be fired from a specific point instead of from the middle
    [SerializeField] private Transform sightPointCrouched;
    [SerializeField] private float sightDistance;
    [Range(0,360)]
    [SerializeField] private float sightAngle;
    [SerializeField] private float sightUpdateTimer = 0.2f; // How often to update vision


    private Transform playerPos;
    private PlayerMovement playerMovement;

    [SerializeField] private bool canSeePlayer;

    public bool CanSeePlayer { get { return canSeePlayer; } }

    public Transform PlayerTransform => playerPos;

    private void Start()
    {
        playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        playerMovement = playerPos.GetComponent<PlayerMovement>();
        StartCoroutine(SightCooldown());
    }

    IEnumerator SightCooldown()
    {
        while(true)
        {
            yield return new WaitForSeconds(sightUpdateTimer);
            CheckForPlayer();

        }

    }

    private void CheckForPlayer()
    {
        Vector3 dir = playerPos.position - transform.position;
        float angle = Vector3.Angle(dir, transform.forward);
        float distance = Vector3.Distance(transform.position, playerPos.position);

        if (angle <= sightAngle / 2 && distance <= sightDistance)
        {


            if (sightPoint && !playerMovement.GetCrouchStatus)
            {
                Raycast(sightPoint.position ,dir, distance);
            }
            else if(sightPointCrouched && playerMovement.GetCrouchStatus)
            {
                Raycast(sightPointCrouched.position, dir, distance);
            }
            else
            {
                Raycast(transform.position, dir, distance);
            }

        }
        else
            canSeePlayer = false;
    }

    private void Raycast(Vector3 pos ,Vector3 dir, float distance)
    {
        RaycastHit hit;
        if (Physics.Raycast(pos, dir.normalized, out hit, distance))
        {
            if (hit.collider.CompareTag("Player") || hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                canSeePlayer = true;
            }
            else
                canSeePlayer = false;

        }
    }
}
