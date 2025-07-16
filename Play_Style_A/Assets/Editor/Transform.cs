using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Transform))]       
public class WorldPosCS : Editor
{
    Transform _t = null;

    private void OnEnable()
    {
        _t = target as Transform;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        
        Vector3 newWorldPosition = EditorGUILayout.Vector3Field("World Position", _t.position);

        
        if (newWorldPosition != _t.position)
        {
            Undo.RecordObject(_t, "Move Transform");
            _t.position = newWorldPosition;
        }
    }
}
