[System.Serializable]
public class CharacterData
{
    public int health;
    public int ammo;
    public int coin;
    // 필요한 다른 데이터를 추가합니다.

    public CharacterData(CharacterControllerCS character)
    {
        health = character.health;
        ammo = character.ammo;
        coin = character.coin;
        // 다른 필요한 데이터를 여기에 복사합니다.
    }
}