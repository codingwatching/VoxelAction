[System.Serializable]
public class CharacterData
{
    public int health;
    public int ammo;
    public int coin;
    // �ʿ��� �ٸ� �����͸� �߰��մϴ�.

    public CharacterData(CharacterControllerCS character)
    {
        health = character.health;
        ammo = character.ammo;
        coin = character.coin;
        // �ٸ� �ʿ��� �����͸� ���⿡ �����մϴ�.
    }
}