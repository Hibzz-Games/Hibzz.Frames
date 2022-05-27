using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hibzz.Frames
{
	// A component that helps the Frames system to analyze the gameobject it's
	// attached to and record any property or field marked as Recordable
	public class FrameRecorder : MonoBehaviour
	{
		[Tooltip("Should the FrameRecorder record transforms?")]
		public bool ShouldRecordTransform = true;

		[Tooltip("How frequently should the recorder record data?")]
		public float RecordInterval = 0;

		/* What is data?  
		 * 
		 */

		protected void Start()
		{
			MonoBehaviour[] monos = gameObject.GetComponents<MonoBehaviour>();
			foreach(var mono in monos)
			{
				// List<string> datas = Frames.RequestRecordableInfo(mono);
				List<string> data = new List<string>();

			}
		}

		protected void OnDestroy()
		{
			// Frames.ReportDestroyed(this);
		}
	}
}
