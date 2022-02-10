using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmissionToggler : MonoBehaviour
{
    [SerializeField] private Material _emissionMaterial;
    [SerializeField] private Material _noEmissionMaterial;
    [SerializeField] private MeshRenderer[] _renderersToToggle;
    [SerializeField] private bool _emissionOn;

    [ContextMenu("Swap Materials")]
    private void SwapMaterials()
    {
        foreach (var renderer in _renderersToToggle)
        {
            renderer.material = _emissionOn ? _emissionMaterial : _noEmissionMaterial;
        }
    }
}
