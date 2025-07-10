using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraFollowWithMinY : MonoBehaviour
{
    public Transform player;    // שחקן לעקוב אחריו
    public float minY = 3f;     // הגובה המינימלי של המצלמה (כשעל הרצפה)
    public float offsetZ = -10f; // המרחק במישור Z (למצלמה תלת מימדית)
    public float smoothSpeed = 5f; // מהירות החלקה

    private Vector3 velocity = Vector3.zero;
    public float fixedY = 2.5f;   // הגובה הקבוע של המצלמה בציר Y

    private Transform camTransform;

    void Start()
    {
        camTransform = transform; // המיקום של אובייקט המצלמה (הוירטואלית)
        Vector3 startPos = new Vector3(player.position.x, fixedY, camTransform.position.z);
        camTransform.position = startPos;
    }
    void LateUpdate()
    {
        if (player == null)
            return;

        float targetY = Mathf.Max(player.position.y, minY);
        Vector3 targetPos = new Vector3(player.position.x, targetY, offsetZ);

        // החלקה חלקה למיקום החדש
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, 1f / smoothSpeed);
    }
}
