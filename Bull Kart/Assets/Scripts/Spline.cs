using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Spline : MonoBehaviour {

	public Vector3[] points;

	public int LineCount {
		get {
			return points.Length - 1;
		}
	}

	public void AddSegment() {
		Vector3 point = points[points.Length - 1];
		Array.Resize(ref points, points.Length + 1);
		point.x += 1.0f;
		points[points.Length - 1] = point;
	}

	public void RemoveSegment() {
		if (points.Length > 1) {
			Array.Resize(ref points, points.Length - 1);
		}
	}

	public Vector3 GetPoint(float t) {
		int i;

		if (t >= 1.0f) {
			t = 1.0f;
			i = points.Length - 2;
		} else {
			i = (int) (Mathf.Clamp01(t) * LineCount);
			t -= i;
		}

		return transform.TransformPoint(Vector3.Lerp(points[i], points[i + 1], t));
	}

	public void Reset () {
		points = new Vector3[] {
			new Vector3(0.0f, 0.0f, 0.0f),
			new Vector3(1.0f, 0.0f, 0.0f)
		};
	}
}

[CustomEditor(typeof(Spline))]
public class SplineInspector : Editor {

	private Spline spline;
	private Transform handleTransform;
	private Quaternion handleRotation;

	private const float handleSize = 0.04f;
	private const float pickSize = 0.06f;

	private int selectedIndex = -1;

	public override void OnInspectorGUI() {
		spline = target as Spline;

		if (selectedIndex >= 0 && selectedIndex < spline.points.Length - 1) {
			DrawSelectedPointInspector();
		}

		if (GUILayout.Button ("Add Segment")) {
			Undo.RecordObject(spline, "Add Segment");
			spline.AddSegment();
			EditorUtility.SetDirty(spline);
		} else if (GUILayout.Button("Remove Segment")) {
			Undo.RecordObject(spline, "Remove Segment");
			EditorUtility.SetDirty(spline);
			spline.RemoveSegment();
			selectedIndex = -1;
		}
	}

	private void DrawSelectedPointInspector() {
		GUILayout.Label("Selected Point");
		EditorGUI.BeginChangeCheck();
		Vector3 point = EditorGUILayout.Vector3Field("Position", spline.points[selectedIndex]);

		if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(spline, "Move Point");
			EditorUtility.SetDirty(spline);
			spline.points[selectedIndex] = point;
		}
	}

	void OnSceneGUI() {
		spline = target as Spline;

		handleTransform = spline.transform;
		handleRotation = (Tools.pivotRotation == PivotRotation.Local) ? handleTransform.rotation : Quaternion.identity;

		Vector3 p0 = ShowPoint(0);

		for (int i = 1; i < spline.points.Length - 1; i++) {
			Vector3 p1 = ShowPoint(i);

			Handles.color = Color.gray;
			Handles.DrawLine(p0, p1);

			p0 = p1;
		}
	}

	private Vector3 ShowPoint(int index) {
		Vector3 point = handleTransform.TransformPoint(spline.points[index]);

		Handles.color = Color.white;
		float size = HandleUtility.GetHandleSize(point);

		if (Handles.Button(point, handleRotation, size * handleSize, size * pickSize, Handles.DotCap)) {
			selectedIndex = index;
			Repaint();
		}

		if (selectedIndex == index) {
			EditorGUI.BeginChangeCheck();
			point = Handles.DoPositionHandle(point, handleRotation);

			if (EditorGUI.EndChangeCheck()) {
				Undo.RecordObject(spline, "Move Point");
				EditorUtility.SetDirty(spline);
				spline.points[index] = handleTransform.InverseTransformPoint(point);
			}
		}

		return point;
	}
}
