using UnityEngine;

namespace YesChef.Ingredients
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class IngredientInstance : MonoBehaviour
    {
        [SerializeField] private IngredientData data;
        [SerializeField] private IngredientState state = IngredientState.Raw;

        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;

        public IngredientData Data => data;
        public IngredientState State => state;
        public bool IsPrepared => state == IngredientState.Prepared;

        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
            ApplyVisuals();
        }

        public void Initialize(IngredientData newData, IngredientState initialState = IngredientState.Raw)
        {
            data = newData;
            state = initialState;
            ApplyVisuals();
        }

        public void SetPrepared()
        {
            state = IngredientState.Prepared;
            ApplyVisuals();
        }

        public void ApplyVisuals()
        {
            if (data == null) return;
            if (_meshFilter == null) _meshFilter = GetComponent<MeshFilter>();
            if (_meshRenderer == null) _meshRenderer = GetComponent<MeshRenderer>();

            if (state == IngredientState.Raw)
            {
                _meshFilter.sharedMesh = data.RawMesh;
                _meshRenderer.sharedMaterial = data.RawMaterial;
                transform.localScale = data.RawScale;
            }
            else
            {
                _meshFilter.sharedMesh = data.PreparedMesh != null ? data.PreparedMesh : data.RawMesh;
                _meshRenderer.sharedMaterial = data.PreparedMaterial != null ? data.PreparedMaterial : data.RawMaterial;
                transform.localScale = data.PreparedScale;
            }
        }
    }
}