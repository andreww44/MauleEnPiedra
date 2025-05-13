using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "Card", menuName = "SO/Card")]
public class SO_Cards : ScriptableObject
{
    public new string name;
    public Card type;
    public string description;
    public string region;
    public int part;
    public Image image;
}
