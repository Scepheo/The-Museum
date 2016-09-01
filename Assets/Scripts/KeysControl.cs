using UnityEngine;

public class KeysControl : MonoBehaviour
{
    public float AnimationTime = 1f;
    public Material KeyUpMaterial;
    public Material KeyDownMaterial;

    private Transform keycapDown;
    private Transform keycapLeft;
    private Transform keycapRight;
    private Transform keycapUp;
    private Transform keycapAttach;

    private Transform animating;
    private bool keyUp;
    private float currentTime;

    private void Start()
    {
        keycapUp = transform.Find("Keycap Up");
        keycapLeft = transform.Find("Keycap Left");
        keycapDown = transform.Find("Keycap Down");
        keycapRight = transform.Find("Keycap Right");
        keycapAttach = transform.Find("Keycap Attach");
        animating = null;
    }

    private void Update()
    {
        if (animating != null)
        {
            currentTime += Time.deltaTime;

            if (currentTime > AnimationTime)
            {
                currentTime -= AnimationTime;

                if (keyUp)
                {
                    SetKeyDown(animating);
                }
                else
                {
                    SetKeyUp(animating);
                }
            }
        }
    }

    private void SetKeyUp(Transform keycap)
    {
        keyUp = true;
        var renderer = keycap.GetComponent<MeshRenderer>();
        renderer.material = KeyUpMaterial;
        keycap.localScale = new Vector3(1f, 1f, 1f);
    }

    private void SetKeyDown(Transform keycap)
    {
        keyUp = false;
        var renderer = keycap.GetComponent<MeshRenderer>();
        renderer.material = KeyDownMaterial;
        keycap.localScale = new Vector3(1f, 1f, 0.5f);
    }

    public void StopAnimation()
    {
        if (animating != null)
        {
            SetKeyUp(animating);
        }

        animating = null;
        keyUp = true;
        currentTime = 0;
    }

    private void Animate(Transform keycap)
    {
        StopAnimation();
        animating = keycap;
        SetKeyDown(keycap);
    }

    public void AnimateUp()
    {
        Animate(keycapUp);
    }

    public void AnimateLeft()
    {
        Animate(keycapLeft);
    }

    public void AnimateDown()
    {
        Animate(keycapDown);
    }

    public void AnimateRight()
    {
        Animate(keycapRight);
    }

    public void AnimateAttach()
    {
        Animate(keycapAttach);
    }
}
