using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuHandler : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject cardManger;

    public void NewGame()
    {
        PlayerPrefs.SetInt("LoadGame", 0);
        menuPanel.SetActive(false);
        cardManger.SetActive(true);
        SoundManager.Instance.PlayButtonClick();
    }

    public void LoadGame()
    {
        PlayerPrefs.SetInt("LoadGame", 1);
        menuPanel.SetActive(false);
        cardManger.SetActive(true);
        SoundManager.Instance.PlayButtonClick();
    }

}
