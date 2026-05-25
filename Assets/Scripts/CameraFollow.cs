using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;            
    public float smoothSpeed = 5f;      
    public Vector3 offset;              

    void Start()
    {
        if (target != null && offset == Vector3.zero)
        {
            offset = transform.position - target.position;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;
        
        Vector3 desiredPosition = target.position + offset;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }
}