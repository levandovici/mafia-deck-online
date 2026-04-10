using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Camera _camera;

    [SerializeField]
    private CameraPoint[] _points;

    [SerializeField]
    private int _current = -1;



    public void Setup(CameraPoint[] points, int currentPoint = 0)
    {
        _points = points;

        _current = currentPoint;

        ChangePoint(currentPoint);
    }



    public void NextPoint()
    {
        ChangePoint(_current + 1 < _points.Length ? _current + 1 : 0);
    }

    public void PreviousPoint()
    {
        ChangePoint((_current > 0 ? _current : _points.Length) - 1);
    }



    private void ChangePoint(int index)
    {
        _current = index;

        _camera.transform.position = _points[index].point.position;

        if (_points[index].target != null)
        {
            _camera.transform.LookAt(_points[index].target.position + Vector3.up * 2f);
        }
    }
}

public class CameraPoint
{
    public Transform point;

    public Transform target;



    public CameraPoint(Transform point, Transform target = null)
    {
        this.point = point;

        this.target = target;
    }
}
