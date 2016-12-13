
using Assets.VREF.Scripts.Logic;
using Assets.VREF.Scripts;
using UnityEditor;
using UnityEngine;

namespace Assets.VREF.EditorExtensions.Scripts.Logic
{
	[CustomEditor(typeof(Timer))]
	public class TimerInspector : Editor
	{
		private static readonly GUIContent startButtonContent = new GUIContent("|>", "Start the Timer");
		private static readonly GUIContent pauseButtonContent = new GUIContent("||", "Pause the Timer");
		private static readonly GUIContent resetButtonContent = new GUIContent("[]", "Reset the Timer");
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			var t = target as Timer;

			EditorGUILayout.BeginHorizontal();

			if (GUILayout.Button(startButtonContent))
			{
				t.StartTimer();
			}
			
			if (GUILayout.Button(resetButtonContent))
			{
				t.ResetTimer();
			}

			EditorGUILayout.EndHorizontal();
		}

		[DrawGizmo(GizmoType.NotInSelectionHierarchy | GizmoType.Active)]
		static void renderGizmo(Timer t, GizmoType type){

			var durationAsAngle = t.GetDurationTilEvent().Remap(0, t.Duration, 360, 0);
			var pos = t.transform.position;
			var normal = -Camera.current.transform.forward - Camera.current.transform.forward;
			var handleSize = HandleUtility.GetHandleSize(pos);
			float radius = 0.2f * handleSize;
			Handles.DrawWireDisc(pos, normal, radius + 0.01f);
			Handles.DrawSolidArc (pos, normal, Camera.current.transform.up, durationAsAngle, radius);
			var textPos = pos + new Vector3(-radius, -radius, 0);
			Handles.Label(textPos, t.aName);
			var labelPos = pos + new Vector3(-radius, 2.2f * radius, 0);
			Handles.Label(labelPos, "Timer");
		}
		 
	}

}