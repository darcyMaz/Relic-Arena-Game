using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class ItemSO : ScriptableObject
{
    [SerializeField] private string Name;

    [SerializeField] private Sprite Sprite;

    public string GetName()
    {
        return Name;
    }
    public Sprite GetSprite()
    {
        return Sprite;
    }
}
