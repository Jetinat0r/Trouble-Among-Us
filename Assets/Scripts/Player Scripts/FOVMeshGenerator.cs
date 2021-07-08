using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVMeshGenerator : MonoBehaviour
{
    [SerializeField]
    private float viewRadius;
    [SerializeField]
    [Range(0, 360)]
    private float viewAngle;
    [SerializeField]
    private int edgeResolveIterations;
    [SerializeField]
    private float edgeDistThreshold;

    [SerializeField]
    private float meshResolution;

    [SerializeField]
    private LayerMask opaqueObstacleMask;

    [SerializeField]
    private MeshFilter fovMeshFilter;
    private Mesh fovMesh;

    private Vector3 origin = Vector3.zero;
    private float curAngle = 90f;

    private void Start()
    {
        fovMesh = new Mesh();
        fovMesh.name = "FOV Mesh";
        fovMeshFilter.mesh = fovMesh;
    }

    private void LateUpdate()
    {
        DrawFieldOfView();
    }

    private Vector3 DirFromAngle(float _angleInDegrees)
    {
        return new Vector3(Mathf.Sin(_angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(_angleInDegrees * Mathf.Deg2Rad), 0);
    }

    public void SetPosRot(Vector3 _origin, float _angle)
    {
        origin = _origin;
        curAngle = _angle;
    }

    public void SetViewRadius(float _newViewRadius)
    {
        viewRadius = _newViewRadius;
    }

    public void DrawFieldOfView()
    {
        int _stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float _stepAngleSize = viewAngle / _stepCount;

        List<Vector3> _viewPoints = new List<Vector3>();

        ViewCastInfo _oldViewCast = new ViewCastInfo();

        for (int i = 0; i <= _stepCount; i++)
        {
            //Debug.Log(curAngle);
            float _angle = (-curAngle + 90) - (viewAngle / 2) + (_stepAngleSize * i);

            ViewCastInfo _newViewCast = ViewCast(_angle);

            if (i > 0)
            {
                bool _edgeDistThresholdExceeded = Mathf.Abs(_oldViewCast.dist - _newViewCast.dist) > edgeDistThreshold;
                if (_oldViewCast.hit != _newViewCast.hit || (_oldViewCast.hit && _newViewCast.hit && _edgeDistThresholdExceeded))
                {
                    EdgeInfo _edge = FindEdge(_oldViewCast, _newViewCast);
                    if (_edge.pointA != Vector3.zero)
                    {
                        _viewPoints.Add(_edge.pointA);
                    }
                    if (_edge.pointB != Vector3.zero)
                    {
                        _viewPoints.Add(_edge.pointB);
                    }
                }
            }

            _viewPoints.Add(_newViewCast.point);

            _oldViewCast = _newViewCast;
        }

        int _vertexCount = _viewPoints.Count + 1;

        Vector3[] _vertices = new Vector3[_vertexCount];
        Vector2[] _uv = new Vector2[_vertexCount - 2];
        int[] _triangles = new int[(_vertexCount - 2) * 3];

        _vertices[0] = origin;
        for (int i = 0; i < _vertexCount - 1; i++)
        {
            _vertices[i + 1] = _viewPoints[i];

            if (i < _vertexCount - 2)
            {
                _triangles[i * 3] = 0;
                _triangles[(i * 3) + 1] = i + 1;
                _triangles[(i * 3) + 2] = i + 2;
            }

        }

        fovMesh.Clear();
        fovMesh.vertices = _vertices;
        fovMesh.triangles = _triangles;
        fovMesh.uv = new Vector2[_vertices.Length];
        fovMesh.RecalculateNormals();
    }

    private EdgeInfo FindEdge(ViewCastInfo _minViewCast, ViewCastInfo _maxViewCast)
    {
        float _minAngle = _minViewCast.angle;
        float _maxAngle = _maxViewCast.angle;
        Vector3 _minPoint = Vector3.zero;
        Vector3 _maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float _angle = (_minAngle + _maxAngle) / 2;
            ViewCastInfo _newViewCast = ViewCast(_angle);

            bool _edgeDistThresholdExceeded = Mathf.Abs(_minViewCast.dist - _newViewCast.dist) > edgeDistThreshold;
            if (_newViewCast.hit == _minViewCast.hit && !_edgeDistThresholdExceeded)
            {
                _minAngle = _angle;
                _minPoint = _newViewCast.point;
            }
            else
            {
                _maxAngle = _angle;
                _maxPoint = _newViewCast.point;
            }
        }

        return new EdgeInfo(_minPoint, _maxPoint);
    }

    private ViewCastInfo ViewCast(float _angle)
    {
        Vector3 _dir = DirFromAngle(_angle);
        RaycastHit2D _hit2D = Physics2D.Raycast(origin, _dir, viewRadius, opaqueObstacleMask);

        //Wrong???? idek, idec
        //Debug.DrawLine(origin, (_dir /*+ origin*/) * viewRadius, Color.red);

        if (_hit2D.collider == null)
        {
            return new ViewCastInfo(false, origin + (_dir * viewRadius), viewRadius, _angle);
        }
        else
        {
            return new ViewCastInfo(true, _hit2D.point, _hit2D.distance, _angle);
        }
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dist;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dist, float _angle)
        {
            hit = _hit;
            point = _point;
            dist = _dist;
            angle = _angle;
        }
    }

    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }
}
