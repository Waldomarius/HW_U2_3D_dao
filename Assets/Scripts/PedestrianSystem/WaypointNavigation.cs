using UnityEngine;
using Random = UnityEngine.Random;

namespace PedestrianSystem
{
    public class WaypointNavigation : MonoBehaviour
    {
        [SerializeField] internal Waypoint currentWaypoint;
        [SerializeField] private int _direction;
        private CharacterNavigationController _characterNavigationController;
        
        private void Awake()
        {
            _characterNavigationController = GetComponent<CharacterNavigationController>();
        }

        private void Start()
        {
            _direction = Mathf.RoundToInt(Random.Range(0f, 1f));
        }

        private void Update()
        {
            if (_characterNavigationController._isReachedDestination)
            {
                bool isShouldBranch = false;

                if (currentWaypoint.branch != null && currentWaypoint.branch.Count > 0)
                {
                    isShouldBranch = Random.Range(0f, 1f) <= currentWaypoint.branchesRatio ? true : false;
                }

                if (isShouldBranch)
                {
                    currentWaypoint = currentWaypoint.branch[Random.Range(0, currentWaypoint.branch.Count)];
                }
                else
                {
                    if (_direction == 0)
                    {
                        if (currentWaypoint.nextWaypoint)
                        {
                            currentWaypoint = currentWaypoint.nextWaypoint;
                        }
                        else
                        {
                            currentWaypoint = currentWaypoint.previousWaypoint;
                            _direction = 1;
                        }
                    }
                    else if (_direction == 1)
                    {
                        if (currentWaypoint.previousWaypoint)
                        {
                            currentWaypoint = currentWaypoint.previousWaypoint;
                        }
                        else
                        {
                            currentWaypoint = currentWaypoint.nextWaypoint;
                            _direction = 0;
                        }
                    }
                }
                
                _characterNavigationController.SetDestination(currentWaypoint.GetPosition(), currentWaypoint.name);
            }
        }
    }
}