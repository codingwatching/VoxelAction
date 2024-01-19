using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    private Status status; // 플레이어의 상태 (이동 속도, 체력)

    [Header("HP & BloodScreen UI")]
    [SerializeField]
    private TextMeshProUGUI textHP; // 플레이어의 체력을 출력하는 Text
    [SerializeField]
    private Image imageBloodScreen; // 플레이어가 공격받았을 때 화면에 표시되는 Image
    [SerializeField]
    private AnimationCurve curveBloodScreen;

    public static UIManager instance;

    public GameObject menuPanel;
    public GameObject gamePanel;

    public TMP_Text maxScoreText;
    public TMP_Text scoreText;
    public TMP_Text stageText;
    public TMP_Text playTimeText;
    public TMP_Text playerHealthText;
    public TMP_Text playerAmmoText;
    public TMP_Text playerCoinText;

    public Image weapon1Img;
    public Image weapon2Img;
    public Image weapon3Img;
    public Image weapon4Img;
    public Image aimImg;

    public TMP_Text enemyAText;
    public TMP_Text enemyBText;
    public TMP_Text enemyCText;
    public RectTransform bossHealthGroup;
    public RectTransform bossHealthBar;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        GameObject target = GameObject.Find("Character(Clone)");
        status = target.transform.GetComponent<Status>();
        status.onHPEvent.AddListener(UpdateHPHUD);

        TurnOnOffAimImg(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateEnemyCnt();
    }

    private void UpdateEnemyCnt()
    {
        enemyAText.text = "x " + GameManager.instance.enemyCntA.ToString();
    }

    private void UpdateHPHUD(int previous, int current)
    {
        textHP.text = "HP " + current;
        if (current < previous)
        {
            StopCoroutine("OnBloodScreen");
            StartCoroutine("OnBloodScreen");
        }
    }
    private IEnumerator OnBloodScreen()
    {
        float percent = 0;
        while(percent < 1)
        {
            if (imageBloodScreen.IsActive() == false)
            {
                imageBloodScreen.gameObject.SetActive(true);
            }
            percent += Time.deltaTime;
            Color color = imageBloodScreen.color;
            color.a = Mathf.Lerp(1,0,curveBloodScreen.Evaluate(percent));
            imageBloodScreen.color= color;
            yield return null;  
        }
    }

    public void TurnOnOffAimImg(bool turn)
    {
        aimImg.gameObject.SetActive(turn);
    }
}
