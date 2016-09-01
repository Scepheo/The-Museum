using UnityEngine;

public class Attachable : MonoBehaviour
{
    public Material AttachedMaterial;
    public Material UnattachedMaterial;

    public bool IsAttached { get; private set; }

    private MeshRenderer meshRenderer;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void Attach()
    {
        IsAttached = true;
        meshRenderer.material = AttachedMaterial;
    }

    public void Detach()
    {
        IsAttached = false;
        meshRenderer.material = UnattachedMaterial;
    }
}
