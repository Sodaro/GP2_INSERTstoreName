using UnityEngine;

public class PathNode : MonoBehaviour
{

    //This was deprecated in favor of a custom editor for npcs following paths with points being pure vectors

    [SerializeField] private Material _occupiedMaterial;
    [SerializeField] private Material _freeMaterial;
    private bool _isOccupied = false;
    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshRenderer.enabled = false;
    }
    public void SetOccupied(bool value)
    {
        _isOccupied = value;
    }
    public bool IsOccupied => _isOccupied;
}
