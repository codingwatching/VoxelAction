using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
// 체력 정보 바뀔 때마다 이벤트 자동 호출
public class HPEvent : UnityEngine.Events.UnityEvent<int, int> { }
public class AmmoEvent : UnityEngine.Events.UnityEvent<int, int> { }
public class CoinEvent : UnityEngine.Events.UnityEvent<int, int> { }

public class Status : MonoBehaviour
{
    [HideInInspector]
    public HPEvent onHPEvent = new HPEvent();
    public AmmoEvent onAmmoEvent = new AmmoEvent();
    public CoinEvent onCoinEvent = new CoinEvent();

    [Header("Walk, Run Speed")]
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;

    [Header("HP")]
    [SerializeField]
    private int maxHP = 100;
    [SerializeField]
    private int currentHP;

    [Header("Ammo")]
    [SerializeField]
    private int maxAmmo = 99;
    [SerializeField]
    public int currentAmmo;

    [Header("Coin")]
    [SerializeField]
    private int maxCoin = 99999;
    [SerializeField]
    private int currentCoin;

    // 외부에서 값 확인하는 Get 속성
    public float WalkSpeed => walkSpeed;
    public float RunSpeed => runSpeed;

    public int CurrentHP => currentHP;
    public int MaxHP => maxHP;

    public int CurrentAmmo => currentAmmo;
    public int MaxAmmo => maxAmmo;

    public int CurrentCoin => currentCoin;
    public int MaxCoin => maxCoin;

    private void Awake()
    {
        currentHP = maxHP;
    }

    public bool DecreaseHP(int damage)
    {
        int previouseHP = currentHP;

        currentHP = currentHP - damage > 0 ? currentHP - damage : 0;

        onHPEvent.Invoke(previouseHP, currentHP);

        if (currentHP == 0)
        {
            return true;
        }
        return false;
    }

    public void IncreaseHP(int heart) 
    {
        int previousHP = currentHP;

        currentHP = currentHP + heart > maxHP ? maxHP : currentHP + heart;

        onHPEvent.Invoke(previousHP, currentHP);
    }

    public void IncreaseCoin(int coin)
    {
        int previousCoin = currentCoin;

        currentCoin = currentCoin + coin > maxCoin ? maxCoin : currentCoin + coin;

        onCoinEvent.Invoke(previousCoin, currentCoin);
    }

    public void IncreaseAmmo(int ammo)
    {
        int previousAmmo = currentAmmo;

        currentAmmo = currentAmmo + ammo > maxAmmo ? maxAmmo : currentAmmo + ammo;

        onAmmoEvent.Invoke(previousAmmo, currentAmmo);
    }

    public void DecreasAmmo_ReloadOut(int currentAmmo)
    {
        int previousAmmo = currentAmmo > 0 ? currentAmmo : 0;

        onAmmoEvent.Invoke(previousAmmo, currentAmmo);
    }
}
