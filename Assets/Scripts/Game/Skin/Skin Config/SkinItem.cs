using UnityEngine;
public enum StatusState
{
    Lock, Ads, Own, Equip
}
public enum SkinType
{
    Type1, Type2, Type3, Type4
}
[CreateAssetMenu(fileName = "Skin", menuName = "ScriptableObjects/Skin")]
public class SkinItem : ScriptableObject
{
    [SerializeField] private int id;
    [SerializeField] private int lvlUnlock = -1;
    [SerializeField] private SkinType skinType;
    [SerializeField] private StatusState state;
    [SerializeField] private Sprite image;
    [SerializeField] private bool isSeen = false;
    public SkinItem(int id, StatusState state, SkinType skinType, Sprite image, int lvlUnlock, bool isSeen)
    {
        this.id = id;
        this.state = state;
        this.skinType = skinType;
        this.image = image;
        this.lvlUnlock = lvlUnlock;
        this.isSeen = isSeen;
    }
    public int Id { get => id; set => id = value; }
    public SkinType SkinType { get => skinType; set => skinType = value; }
    public StatusState State { get => state; set => state = value; }
    public Sprite Image { get => image; set => image = value; }
    public int LvlUnlock { get => lvlUnlock; set => lvlUnlock = value; }
    public bool IsSeen { get => isSeen; set => isSeen = value; }
}
