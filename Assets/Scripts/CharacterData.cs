using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Character Selection/Character Data")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public Color mainColor;
    public Color shoeColor;
    public Animator animator;
    public Collider2D collider;
    public Sprite headSprite;
}