using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public enum TutorialKey { Up, Left, Down, Right, Attach, None }

    public TutorialKey Key;

    private KeysControl keysControl;
    private bool triggered;

    private void Start()
    {
        var keys = GameObject.Find("Keys");
        keysControl = keys.GetComponent<KeysControl>();
        triggered = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        var player = other.GetComponent<Movement>();

        if (player != null)
        {
            triggered = true;

            switch (Key)
            {
                case TutorialKey.Up:
                    keysControl.AnimateUp();
                    break;
                case TutorialKey.Left:
                    keysControl.AnimateLeft();
                    break;
                case TutorialKey.Down:
                    keysControl.AnimateDown();
                    break;
                case TutorialKey.Right:
                    keysControl.AnimateRight();
                    break;
                case TutorialKey.Attach:
                    keysControl.AnimateAttach();
                    break;
                case TutorialKey.None:
                    keysControl.StopAnimation();
                    break;
            }
        }
    }
}
