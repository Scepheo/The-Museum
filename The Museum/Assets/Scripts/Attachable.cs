using UnityEngine;

public class Attachable : MonoBehaviour
{
    public Material AttachedMaterial;
    public Material UnattachedMaterial;

    public bool IsAttached { get; private set; }

    private Renderer _renderer;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

    public void Attach()
    {
        IsAttached = true;
        _renderer.material = AttachedMaterial;
    }

    public void Detach()
    {
        IsAttached = false;
        _renderer.material = UnattachedMaterial;
    }

    public bool IsVisible
    {
        get { return _renderer.isVisible; }
    }
}
