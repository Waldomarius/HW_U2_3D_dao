using UnityEditor;
using UnityEngine;

namespace PedestrianSystem.Editor
{
    public class WaypointEditorComponent : EditorWindow
    {
        [MenuItem("Tools/Waypoint Editor")]
        public static void OnOpen()
        {
            GetWindow<WaypointEditorComponent>();
        }

        public Transform rootWaypoint;
        
        private void OnGUI()
        {
            SerializedObject serializedObject = new SerializedObject(this);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("rootWaypoint"));
            serializedObject.ApplyModifiedProperties();
            
            if (rootWaypoint == null)
            {
                EditorGUILayout.HelpBox("Waypoint root is nill", MessageType.Error);
            }
            else
            {
                EditorGUILayout.HelpBox("Pedestrian System:", MessageType.Info);
                EditorGUILayout.BeginVertical("box");
                // Создадим метод описывающий кнопки
                DrawButton();
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawButton()
        {
            if (GUILayout.Button("Create Waypoint"))
            {
                CreateWaypoint();
            }

            if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Waypoint>() != null)
            {
                if (GUILayout.Button("Create next Waypoint"))
                {
                    CreateNextWaypoint();
                }
                
                if (GUILayout.Button("Create previous Waypoint"))
                {
                    CreatePreviousWaypoint();
                }
                
                if (GUILayout.Button("Delete Waypoint"))
                {
                    DeletetWaypoint();
                }
                
                if (GUILayout.Button("CreateBranch Waypoint"))
                {
                    CreateBranch();
                }
            }
        }

        private void CreateBranch()
        {
            GameObject waypointObject = new GameObject("Waypoint_" + rootWaypoint.childCount, typeof(Waypoint));
            waypointObject.transform.SetParent(rootWaypoint, false);

            Waypoint waypoint = waypointObject.GetComponent<Waypoint>();
            Waypoint brancesFrom = Selection.activeGameObject.GetComponent<Waypoint>();
            
            brancesFrom.branch.Add(waypoint);
            
            waypoint.transform.position = brancesFrom.transform.position;
            waypoint.transform.forward = brancesFrom.transform.forward;
            
            Selection.activeGameObject = waypoint.gameObject;
        }

        private void DeletetWaypoint()
        {
            Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

            if (selectedWaypoint.nextWaypoint)
            {
                selectedWaypoint.nextWaypoint.previousWaypoint = selectedWaypoint.previousWaypoint;
            }

            if (selectedWaypoint.previousWaypoint)
            {
                selectedWaypoint.previousWaypoint.nextWaypoint = selectedWaypoint.nextWaypoint;
                Selection.activeGameObject = selectedWaypoint.previousWaypoint.gameObject;
            }
            
            DestroyImmediate(selectedWaypoint.gameObject);
        }

        private void CreatePreviousWaypoint()
        {
            GameObject waypointObject = new GameObject("Waypoint_" + rootWaypoint.childCount, typeof(Waypoint));
            waypointObject.transform.SetParent(rootWaypoint, false);

            Waypoint newWaypoint = waypointObject.GetComponent<Waypoint>();
            Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();
            waypointObject.transform.position = selectedWaypoint.transform.position;
            waypointObject.transform.forward = selectedWaypoint.transform.forward;

            if (selectedWaypoint.previousWaypoint)
            {
                newWaypoint.previousWaypoint = selectedWaypoint.previousWaypoint;
                selectedWaypoint.previousWaypoint.nextWaypoint = newWaypoint;
            }

            newWaypoint.nextWaypoint = selectedWaypoint;
            selectedWaypoint.previousWaypoint = newWaypoint;
            newWaypoint.transform.SetSiblingIndex(selectedWaypoint.transform.GetSiblingIndex());
            
            Selection.activeGameObject = newWaypoint.gameObject;
        }

        private void CreateNextWaypoint()
        {
            GameObject waypointObject = new GameObject("Waypoint_" + rootWaypoint.childCount, typeof(Waypoint));
            waypointObject.transform.SetParent(rootWaypoint, false);

            Waypoint newWaypoint = waypointObject.GetComponent<Waypoint>();
            Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();
            waypointObject.transform.position = selectedWaypoint.transform.position;
            waypointObject.transform.forward = selectedWaypoint.transform.forward;

            newWaypoint.previousWaypoint = selectedWaypoint;

            if (selectedWaypoint != null)
            {
                selectedWaypoint.nextWaypoint.previousWaypoint = newWaypoint;
                newWaypoint.nextWaypoint = selectedWaypoint.nextWaypoint;
            }

            selectedWaypoint.nextWaypoint = newWaypoint;
            newWaypoint.transform.SetSiblingIndex(selectedWaypoint.transform.GetSiblingIndex());
            
            Selection.activeGameObject = newWaypoint.gameObject;
        }

        private void CreateWaypoint()
        {
            GameObject waypointObject = new GameObject("Waypoint_" + rootWaypoint.childCount, typeof(Waypoint));
            waypointObject.transform.SetParent(rootWaypoint, false);
            
            Waypoint waypoint = waypointObject.GetComponent<Waypoint>();
            if (rootWaypoint.childCount > 1)
            {
                waypoint.previousWaypoint = rootWaypoint.GetChild(rootWaypoint.childCount - 2).GetComponent<Waypoint>();
                waypoint.previousWaypoint.nextWaypoint = waypoint;
                
                waypoint.transform.position = waypoint.previousWaypoint.transform.position;
                waypoint.transform.forward = waypoint.previousWaypoint.transform.forward;
            }

            // Выделение на объект
            Selection.activeGameObject = waypoint.gameObject;
        }
    }
}
