using System.Collections.Generic;
using UnityEngine;

namespace PedestrianSystem
{
    public class Waypoint : MonoBehaviour
    {
        public Waypoint previousWaypoint;
        public Waypoint nextWaypoint;

        [Range(0f, 5f)]
        public float width = 1f;

        public List<Waypoint> branch;
        
        [Range(0f, 1f)]
        public float branchesRatio = 0.5f;
        
        public Vector3 GetPosition()
        {
            Vector3 minBound = transform.position + transform.right * width / 2f;
            Vector3 maxBound = transform.position - transform.right * width / 2f;
            
            // Вернет координаты в пределах minBound и maxBound.
            return Vector3.Lerp(minBound, maxBound, Random.Range(0f, 1f));
        }
    }
}