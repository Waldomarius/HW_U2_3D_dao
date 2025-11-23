using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FOV
{
    public class FieldOfView : MonoBehaviour
    {
        [SerializeField] internal float _viewRadius = 15f;
        
        [Range(0, 120)]
        [SerializeField] internal float _viewAngle = 90f;

        [SerializeField] private LayerMask _targetMask;
        [SerializeField] private LayerMask _obstacleMask;
        
        [SerializeField] internal List<Transform> _visibleTargets;

        [SerializeField] private int _edgeResolvedIterations = 5;
        [SerializeField] private float _edgeDistance = 5;
        
        [SerializeField] internal float _meshResolution;
        [SerializeField] private MeshFilter _meshFilter;
        private Mesh _mesh;
        
        private void Start()
        {
            InitMesh();
            StartCoroutine(nameof(GetTargetWithDelay), 1);
        }
        
        private void LateUpdate()
        {
            DrawFieldOfView();
        }

        private void InitMesh()
        {
            _mesh = new Mesh();
            _mesh.name = "FieldOfView";
            _meshFilter.mesh = _mesh;
        }
        
        public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
        {
            if (!angleIsGlobal)
            {
                angleInDegrees += transform.eulerAngles.y;
            }
            
            return new Vector3(
                Mathf.Sin(angleInDegrees * Mathf.Deg2Rad),
                0,
                Mathf.Cos(angleInDegrees * Mathf.Deg2Rad)
                );
        }

        IEnumerator GetTargetWithDelay(float delay)
        {
            while (true)
            {
                yield return new WaitForSeconds(delay);
                GetVisibleTargets();
            }
        }

        
        private void GetVisibleTargets()
        {
            _visibleTargets.Clear();
            
            // Все коллаидеры в данной сфере
            Collider[] colliders = Physics.OverlapSphere(transform.position, _viewRadius, _targetMask);

            for (int i = 0; i < colliders.Length; i++)
            {
                Transform target = colliders[i].transform;
                
                // Необходимо просчитать угол между таргетом и углом fov
                Vector3 dirToTarget = (target.position - transform.position).normalized;

                // Если меньше нашего fov то видим
                if (Vector3.Angle(transform.forward, dirToTarget) < _viewAngle / 2)
                {
                    // Теперь проверим дальность до объекта
                    float disToTarget = Vector3.Distance(transform.position, target.position);
                    
                    // Теперь проверим RayCast и убедимся что объект в прямой видимости (проверяем на наличие препятствий)
                    if (!Physics.Raycast(transform.position, dirToTarget, disToTarget, _obstacleMask))
                    {
                        _visibleTargets.Add(target);
                    }
                }
            }
            
        }

        private ViewCastInfo ViewCast(float globalAngle)
        {
            Vector3 dir =  DirFromAngle(globalAngle, true);
            RaycastHit hit;

            if (Physics.Raycast(transform.position, dir, out hit, _viewRadius, _obstacleMask))
            {
                return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
            }
            else
            {
                return new ViewCastInfo(false, transform.position + dir * _viewRadius, _viewRadius,  globalAngle);
            }
        }
        
        private void DrawFieldOfView()
        {
            int stepCount = Mathf.RoundToInt(_viewAngle * _meshResolution);
            float stepAngleSize = _viewAngle / stepCount;

            List<Vector3> viewPoints = new List<Vector3>();

            ViewCastInfo oldCast = new ViewCastInfo();
            
            for (int i = 0; i <= stepCount; i++)
            {
                float angle = transform.eulerAngles.y - _viewAngle / 2 + stepAngleSize * i;
                
                // Debug.DrawLine(
                //     transform.position,
                //     transform.position + DirFromAngle(angle, true) * _viewRadius,
                //     Color.red
                //     );
                
                ViewCastInfo newViewCast = ViewCast(angle);

                if (i > 0)
                {
                    if (oldCast.Hit != newViewCast.Hit)
                    {
                        EdgeInfo edge = FindEdge(oldCast, newViewCast);
                        if (edge.PointA != Vector3.zero)
                        {
                            viewPoints.Add(edge.PointA);
                        }
                        
                        if (edge.PointB != Vector3.zero)
                        {
                            viewPoints.Add(edge.PointB);
                        }
                    }
                }
                
                viewPoints.Add(newViewCast.Point);
                
                oldCast = newViewCast;
            }
            
            int vertexCount = viewPoints.Count + 1;
            Vector3[] vertices = new Vector3[vertexCount];
            int[] triangles = new int[(vertexCount - 2) * 3];
            
            vertices[0] = Vector3.zero;

            for (int i = 0; i < vertexCount - 1; i++)
            {
                vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

                if (i < vertexCount - 2)
                {
                    // 1я вершина
                    triangles[i * 3] = 0;
                    // 2я вершина
                    triangles[i * 3 + 1] = i + 1;
                    // 3я вершина
                    triangles[i * 3 + 2] = i + 2;
                }
            }
            
            _mesh.Clear();
            
            _mesh.vertices = vertices;
            _mesh.triangles = triangles;
            _mesh.RecalculateNormals();
        }

        private EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
        {
            float minAngle = minViewCast.Angle;
            float maxAngle = maxViewCast.Angle;
            Vector3 minPoint = Vector3.zero;
            Vector3 maxPoint = Vector3.zero;

            for (int i = 0; i < _edgeResolvedIterations; i++)
            {
                float angle = (minAngle + maxAngle) / 2;
                ViewCastInfo newViewCast = ViewCast(angle);
                
                bool edgeDistThresholdExceeded = Mathf.Abs(minViewCast.Distance - newViewCast.Distance) > _edgeDistance;

                if (newViewCast.Hit == minViewCast.Hit && !edgeDistThresholdExceeded)
                {
                    minAngle = angle;
                    minPoint = newViewCast.Point;
                }
                else
                {
                    maxAngle = angle;
                    maxPoint = newViewCast.Point;
                }
            }
            return new EdgeInfo(minPoint, maxPoint);
        }

        public struct ViewCastInfo
        {
            public bool Hit;
            public Vector3 Point;
            public float Distance;
            public float Angle;

            public ViewCastInfo(bool hit, Vector3 point, float distance, float angle)
            {
                Hit = hit;
                Point = point;
                Distance = distance;
                Angle = angle;
            }
        }
        
        public struct EdgeInfo
        {
            public Vector3 PointA;
            public Vector3 PointB;

            public EdgeInfo(Vector3 pointA, Vector3 pointB)
            {
                PointA = pointA;
                PointB = pointB;
            }
        }
    }
}