using UnityEngine;

[CreateAssetMenu(fileName = "NewCardData", menuName = "Data/Card Data")]
public class CardData : ScriptableObject
{
    public int id;
    public Sprite frontSprite;
}