using UnityEngine;
using System.Collections;

public class CardIdleMotion : MonoBehaviour
{
    private readonly Vector3 offsetRight = new Vector3(0.1f, 0, 0);
    private readonly Vector3 offsetLeft = new Vector3(-0.1f, 0, 0);

    void Start()
    {
        StartCoroutine(Sway());
    }

    void Update()
    {
        float angle = Mathf.Sin(Time.time * 5f) * 2f; 
        transform.localRotation = Quaternion.Euler(0, 0, angle);
    }

    IEnumerator Sway()
    {
        while (true)
        {
            yield return MoveRoutine(offsetRight);

            yield return MoveRoutine(offsetLeft);
        }
    }

    IEnumerator MoveRoutine(Vector3 offset)
    {
        Vector3 startPos = transform.localPosition;
        Vector3 targetPos = startPos + offset;
        
        float speed = 2f; 
        float t = 0f;
        
        while (t < 2f)
        {
            t += Time.deltaTime * speed;
            
            transform.localPosition = Vector3.Lerp(startPos, targetPos, t);
            
            yield return null;
        }
        
        transform.localPosition = targetPos;
    }
}