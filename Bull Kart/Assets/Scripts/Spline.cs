using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Spline : MonoBehaviour {

	public Vector3[] points;

	public bool circuit;

	public int SegmentCount {
		get {
			return circuit ? points.Length : points.Length - 1;
		}
	}

	public float TotalLength { get; private set; }

	public Vector3 GetPoint(int index) {
		return points[index % points.Length];
	}

	public void SetPoint(int index, Vector3 point) {
		points[index % points.Length] = point;
	}

	public void UpdateTotalLength() {
		TotalLength = 0.0f;

		for (int i = 0; i < SegmentCount; i++) {
			TotalLength += Vector3.Distance(GetPoint(i), GetPoint(i + 1));
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
			Vector3 p0 = GetPoint(i);
			Vector3 p1 = GetPoint(i + 1);

			float segmentLength = Vector3.Distance(p0, p1);
			float d = Vector3.Distance(p0, position);

			if (Mathf.Abs(segmentLength - (d + Vector3.Distance(position, p1))) < 0.05f) {
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
			Vector3 p = GetClosestPointToSegment(point, GetPoint(i), GetPoint(i + 1));
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
		circuit = false;
		points = new Vector3[] {
			new Vector3(0.0f, 0.0f, 0.0f)
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

		if (GUILayout.Button("Toggle Circuit")) {
			Undo.RecordObject(spline, "Toggle Circuit");
			spline.circuit = !spline.circuit;
			EditorUtility.SetDirty(spline);
		} else if (GUILayout.Button("Add Segment")) {
			Undo.RecordObject(spline, "Add Segment");
			EditorUtility.SetDirty(spline);
			selectedIndex = spline.points.Length;
			spline.AddSegment();
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
		Vector3 point = EditorGUILayout.Vector3Field("Position", spline.GetPoint(selectedIndex));

		if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(spline, "Move Point");
			EditorUtility.SetDirty(spline);
			spline.SetPoint(selectedIndex, point);
			spline.UpdateTotalLength();
		}
	}

	void OnSceneGUI() {
		spline = target as Spline;

		handleTransform = spline.transform;
		handleRotation = (Tools.pivotRotation == PivotRotation.Local) ? handleTransform.rotation : Quaternion.identity;

		Vector3 p0 = ShowPoint(0);

		for (int i = 1; i <= spline.SegmentCount; i++) {
			Vector3 p1 = ShowPoint(i);

			Handles.color = Color.magenta;
			Handles.DrawLine(p0, p1);

			p0 = p1;
		}
	}

	private Vector3 ShowPoint(int index) {
		Vector3 point = handleTransform.TransformPoint(spline.GetPoint(index));

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
				spline.SetPoint(index, handleTransform.InverseTransformPoint(point));
				spline.UpdateTotalLength();
			}
		}

		return point;
	}
}
