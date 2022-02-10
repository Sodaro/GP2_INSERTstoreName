using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PathFollowAI)), CanEditMultipleObjects]
public class AIShopperInspector : Editor
{
	private PathFollowAI _ai;
	private List<Vector3> _points;
	private Transform _handleTransform;
	private Quaternion _handleRotation;

	public override void OnInspectorGUI()
    {
		_ai = target as PathFollowAI;
		DrawDefaultInspector();

		EditorGUI.BeginChangeCheck();

		
		if (GUILayout.Button("Add Path Point"))
		{
			Undo.RecordObject(_ai, "Add Point");
			_ai.AddPathPoint();
			EditorUtility.SetDirty(_ai);
		}
	}



    private void OnSceneGUI()
    {
        _ai = target as PathFollowAI;
        if (_ai == null)
            return;

        _handleTransform = _ai.transform;
        _handleRotation = Tools.pivotRotation == PivotRotation.Local ?
            _handleTransform.rotation : Quaternion.identity;
        _points = _ai.Path;

        GUIStyle style = new GUIStyle();
        style.fontSize = 18;
        for (int i = 0; i < _ai.Path.Count; i++)
        {
            Handles.BeginGUI();
            Vector3 pos = _points[i] + Vector3.up;
            Handles.Label(pos, i.ToString(), style);
            MovePoint(i);
        }
    }


    private void MovePoint(int index)
	{
        Vector3 point = _points[index];
        EditorGUI.BeginChangeCheck();
        point = Handles.DoPositionHandle(point, Quaternion.identity);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(_ai, "Move Point");
            EditorUtility.SetDirty(_ai);
            _ai.SetPositionOfIndex(index, point);
        }
    }
}
