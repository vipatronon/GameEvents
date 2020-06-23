using UnityEngine;

public class CubeMovement : MonoBehaviour
{
    [Header("Targets")]
    [SerializeField] private Transform right;
    [SerializeField] private Transform left;


    [Header("GameEvents")]
    [SerializeField] private GameEvent onReachRight;
    [SerializeField] private GameEvent onReachLeft;


    [Header("Config")]
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private Transform target;

    private void Start()
    {
        if (target == null)
        {
            target = right;
        }
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * speed);

        if (Vector3.Distance(transform.position, right.position) <= 0.2)
        {
            onReachRight.Raise();
            target = left;
        }
        else if (Vector3.Distance(transform.position, left.position) <= 0.2)
        {
            onReachLeft.Raise();
            target = right;
        }
    }
}
