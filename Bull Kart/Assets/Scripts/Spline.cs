using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Spline : MonoBehaviour {

	public Vector3[] points;

	public int SegmentCount {
		get {
			return points.Length - 1;
		}
	}

	public float TotalLength { get; private set; }

	public void UpdateTotalLength() {
		TotalLength = 0.0f;

		for (int i = 0; i < SegmentCount; i++) {
			TotalLength += Vector3.Distance(points[i], points[i + 1]);
		}
	}

	void Start() {
		UpdateTotalLength();
	}

	public void AddSegment() {
		Vector3 point = points[points.Length - 1];
		Array.Resize(ref points, points.Length + 1);
		point.x += 1.0f;
		points[points.Length - 1] = point;
		TotalLength += 1.0f;
	}

	public void RemoveSegment() {
		if (points.Length > 1) {
			TotalLength -= Vector3.Distance(points[points.Length - 1], points[points.Length - 2]);
			Array.Resize(ref points, points.Length - 1);
		}
	}

	public float GetPosition(Vector3 point) {
		float distance = 0.0f;
		Vector3 position = transform.InverseTransformPoint(GetClosestPoint(point));

		for (int i = 0; i < SegmentCount; i++) {
			Vector3 p0 = points[i];
			Vector3 p1 = points[i + 1];

			float segmentLength = Vector3.Distance(p0, p1);
			float d = Vector3.Distance(p0, position);

			if (segmentLength > d) {
				distance += d;
				break;
			}

			distance += segmentLength;
		}

		return distance / TotalLength;
	}

	public Vector3 GetClosestPoint(Vector3 point) {
		point = transform.InverseTransformPoint(point);

		float minDistance = float.PositiveInfinity;
		Vector3 closestPoint = points[0];

		for (int i = 0; i < SegmentCount; i++) {
			Vector3 p = GetClosestPointToSegment(point, points[i], points[i + 1]);
			float distance = Vector3.Distance(point, p);

			if (minDistance > distance) {
				minDistance = distance;
				closestPoint = p;
			}
		}

		return transform.TransformPoint(closestPoint);
	}

	private Vector3 GetClosestPointToSegment(Vector3 point, Vector3 p0, Vector3 p1) {
		Vector3 u = (p1 - p0).normalized;
		Vector3 v = point - p0;

		float d = Vector3.Distance(p0, p1);
		float t = Vector3.Dot(u, v);

		if (t <= 0) {
			return p0;
		} else if (t >= d) {
			return p1;
		}

		return p0 + (u * t);
	}

	public void Reset() {
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

		if (selectedIndex >= 0 && selectedIndex < spline.points.Length) {
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
			spline.UpdateTotalLength();
		}
	}

	void OnSceneGUI() {
		spline = target as Spline;

		handleTransform = spline.transform;
		handleRotation = (Tools.pivotRotation == PivotRotation.Local) ? handleTransform.rotation : Quaternion.identity;

		Vector3 p0 = ShowPoint(0);

		for (int i = 1; i < spline.points.Length; i++) {
			Vector3 p1 = ShowPoint(i);

			Handles.color = Color.magenta;
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
				spline.UpdateTotalLength();
			}
		}

		return point;
	}
}
