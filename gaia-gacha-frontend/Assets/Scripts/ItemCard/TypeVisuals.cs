using UnityEngine;

[CreateAssetMenu(menuName = "GaiaGacha/Type Visuals")]
public class TypeVisuals : ScriptableObject
{
    public Sprite floraIcon;
    public Sprite faunaIcon;

    public Sprite GetIcon(string typeName) => typeName switch
    {
        "Flora" => floraIcon,
        "Fauna" => faunaIcon,
        _       => null
    };
}
