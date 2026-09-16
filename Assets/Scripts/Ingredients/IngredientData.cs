using UnityEngine;

namespace YesChef.Ingredients
{
    [CreateAssetMenu(fileName = "IngredientData", menuName = "YesChef/Ingredient Data")]
    public class IngredientData : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private IngredientType type;
        [SerializeField] private string displayName;
        [SerializeField] private int baseScore;

        [Header("Visuals - Raw")]
        [SerializeField] private Mesh rawMesh;
        [SerializeField] private Material rawMaterial;
        [SerializeField] private Vector3 rawScale = Vector3.one * 0.4f;

        [Header("Visuals - Prepared")]
        [SerializeField] private Mesh preparedMesh;
        [SerializeField] private Material preparedMaterial;
        [SerializeField] private Vector3 preparedScale = Vector3.one * 0.4f;

        public IngredientType Type => type;
        public string DisplayName => displayName;
        public int BaseScore => baseScore;

        public Mesh RawMesh => rawMesh;
        public Material RawMaterial => rawMaterial;
        public Vector3 RawScale => rawScale;

        public Mesh PreparedMesh => preparedMesh;
        public Material PreparedMaterial => preparedMaterial;
        public Vector3 PreparedScale => preparedScale;
    }
}