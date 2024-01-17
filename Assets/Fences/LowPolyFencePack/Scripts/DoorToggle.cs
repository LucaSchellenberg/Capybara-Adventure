using UnityEngine;

namespace BrokenVector.LowPolyFencePack
{
    /// <summary>
    /// This class toggles the door animation.
    /// The gameobject of this script has to have the DoorController script which needs an Animator component
    /// and some kind of Collider which detects your mouse click applied.
    /// </summary>
    [RequireComponent(typeof(DoorController))]
	public class DoorToggle : MonoBehaviour
    {
        private bool doorToggled = false;
        private DoorController doorController;
        public PlayerController playerController;
        private bool keyStolen;

        void Awake()
        {
            doorController = GetComponent<DoorController>();
        }

        void Update()
        {
             keyStolen= playerController.keyStolen;
            if (keyStolen == true && !doorToggled)
            {
                doorController.ToggleDoor();
                doorToggled= true;
            }
        }

	}
}