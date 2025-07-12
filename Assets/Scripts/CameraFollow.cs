using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraFollowWithMinY : MonoBehaviour
{
    public Transform player;    
    public float minY = 3f;    
    public float offsetZ = -10f; 
    public float smoothSpeed = 5f; 

    private Vector3 velocity = Vector3.zero;
    public float fixedY = 2.5f;   

    private Transform camTransform;

    void Start()
    {
        camTransform = transform; 
        Vector3 startPos = new Vector3(player.position.x, fixedY, camTransform.position.z);
        camTransform.position = startPos;
    }
    void LateUpdate()
    {
        if (player == null)
            return;

        float targetY = Mathf.Max(player.position.y, minY);
        Vector3 targetPos = new Vector3(player.position.x, targetY, offsetZ);

        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, 1f / smoothSpeed);
    }
}
