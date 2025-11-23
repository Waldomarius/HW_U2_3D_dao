using System;
using UnityEditor;
using UnityEngine;

namespace FOV
{
    [CustomEditor(typeof(FieldOfView))]
    public class FieldsOfViewEditor : Editor
    {
        private void OnSceneGUI()
        {
            FieldOfView fov = (FieldOfView)target;
            Handles.color = Color.white;
            
            // Отрисуем круг в 3D пространстве
            Handles.DrawWireArc(
                fov.transform.position, 
                Vector3.up,
                Vector3.forward,
                360,
                fov._viewRadius
                );
            
            // Получаем векторы углов обзора слева и справа
            Vector3 viewAngleLeft = fov.DirFromAngle(-fov._viewAngle / 2, false);
            Vector3 viewAngleRight = fov.DirFromAngle(fov._viewAngle / 2, false);
            
            // Отрисовка каждого луча
            Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleLeft * fov._viewRadius);
            Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleRight * fov._viewRadius);
            
        }
    }
}