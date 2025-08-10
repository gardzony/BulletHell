using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DrawHands : MonoBehaviour
{
    [SerializeField] private GameObject point2;
    [SerializeField] private GameObject point3;

    private LineRenderer _lineRenderer;
    private Transform _point1;

    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _point1 = transform;

        _lineRenderer.positionCount = 3;
        _lineRenderer.startWidth = 0.1f;
        _lineRenderer.endWidth = 0.1f;

        _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        _lineRenderer.startColor = Color.black;
        _lineRenderer.endColor = Color.black;

        _lineRenderer.useWorldSpace = true;
    }

    private void Update()
    {
        if (point2 != null && point3 != null)
        {
            _lineRenderer.SetPosition(0,
                new Vector3(_point1.position.x, _point1.position.y, 0));
            _lineRenderer.SetPosition(1,
                new Vector3(point2.transform.position.x, point2.transform.position.y, 0));
            _lineRenderer.SetPosition(2,
                new Vector3(point3.transform.position.x, point3.transform.position.y, 0));
        }
        else
        {
            Debug.LogWarning("Назначьте point2 и point3 в инспекторе!");
        }
    }
}