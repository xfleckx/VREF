using UnityEngine;
using System.Collections;

namespace Assets.BeMoBI.Paradigms.AbstractParadigm { 

	/// <summary>
	/// Represents all possible interactions for subject regarding the paradigm or trial behaviour
	/// </summary>
	public class SubjectInteractions : MonoBehaviour {

		private const string SUBMIT_INPUT = "Subject_Submit";
		private const string REQUIRE_PAUSE = "Subject_Requires_Break";

		ParadigmController controller;
		
		public bool TestMode = false;

		private bool homeButtonIsDown = false;
		
		void Awake()
		{
			controller = FindObjectOfType<ParadigmController>();
		}

		// Update is called once per frame
		void Update () {
			

			if (Input.GetAxis(SUBMIT_INPUT) > 0)
			{
				controller.SubjectTriesToSubmit();
			}

			if (Input.GetAxis(REQUIRE_PAUSE) > 0)
			{
				controller.ForceABreakInstantly();
			}
		}
	}
}