using UnityEngine;

namespace CoMZ2
{
	public class KillZombieGuide : IGuideEvent
	{
		private GuideController controller;

		private Vector3 arrowPosition;

		public KillZombieGuide(GuideController controller, Vector3 arrowPosition)
		{
			this.controller = controller;
			this.arrowPosition = arrowPosition;
		}

		public string GuideText()
		{
			return "Kill this zombie to gain power!";
		}

		public IGuideEvent Next()
		{
			return new FireGuide(controller);
		}

		public void DoSomething()
		{
			controller.Arrow.SetActive(true);
			controller.Arrow.transform.localPosition = arrowPosition;
			controller.Event_button.transform.localPosition = new Vector3(0f, 120f, -6f);
		}
	}
}
