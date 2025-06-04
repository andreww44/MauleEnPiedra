using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "Card", menuName = "SO/Card")]
public class SO_Cards : ScriptableObject
{
    public new string name;
    public Card type;
    public Sprite image;
    public int Code;
}
