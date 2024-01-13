using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public static SceneManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // �� ��ȯ �ÿ��� �� ��ü�� �ı����� �ʵ��� ����
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

}
