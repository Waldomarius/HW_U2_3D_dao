using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PedestrianSystem
{
    public class PedestrianSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _pedestrianPrefab;
        [SerializeField] private int _pedestrianToSpawn;

        private void Start()
        {
            StartCoroutine(Spawn());
        }

        IEnumerator Spawn()
        {
            int counter = 0;
            while (counter < _pedestrianToSpawn)
            {
                GameObject obj = Instantiate(_pedestrianPrefab);
                Transform child = transform.GetChild(Random.Range(0, transform.childCount));
                obj.GetComponent<WaypointNavigation>().currentWaypoint = child.GetComponent<Waypoint>();
                obj.transform.position = child.position;
                
                yield return new WaitForEndOfFrame();
                
                counter++;
            }
        }
    }
}