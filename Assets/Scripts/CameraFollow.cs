using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 15, -10);
    public float smoothSpeed = 0.1f;
    public float clampLeftAndRight = 5f;
    public float clampBackward = 3f;
    public float clampForward = 20f;

    public Vector3 getawaySloopPosition = new Vector3(0, 2.7f, 13);
    public bool zoomToGetawaySloop = false;

    private void LateUpdate()
    {
        if (!zoomToGetawaySloop)
        {
            CameraSmoothFollowPirate();
        }
        else
        {
            ZoomToGetawaySloop();
        }
    }

    public void CameraSmoothFollowPirate()
    {
        Vector3 targetPos = target.position + offset;
        targetPos.x = Mathf.Clamp(targetPos.x , -clampLeftAndRight, clampLeftAndRight);
        targetPos.z = Mathf.Clamp(targetPos.z, clampBackward, clampForward);

        Vector3 smoothFollow = Vector3.Lerp(transform.position, targetPos, smoothSpeed);

        transform.position = smoothFollow;
    }

    public void ZoomToGetawaySloop()
    {
        Vector3 smoothFollow = Vector3.Lerp(transform.position, getawaySloopPosition, smoothSpeed);
        transform.position = smoothFollow;
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(30, 0, 0), smoothSpeed);
    }
}
