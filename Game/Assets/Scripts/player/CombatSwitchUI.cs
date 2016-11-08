﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CombatSwitchUI : MonoBehaviour {
    [Header("Header Text")]
    public Text LeftTitleText;
    public Text MiddleTitleText;
    public Text RightTitleText;
    [Header("Body Image")]
    public Image LeftImage;
    public Image MiddleImage;
    public Image RightImage;
    // Use this for initialization
    //void Start () {

    //}//   

    // Update is called once per frame
    //void Update () {

    //}

    public void SwitchStyles(int switchingto)
    {
        string tempText = "";
        Image tempImage;
        Sprite tempSprite;

        switch (switchingto)
        {
            case 1:
                tempText = LeftTitleText.text;
                LeftTitleText.text = MiddleTitleText.text;
                MiddleTitleText.text = tempText;
                

                tempImage = LeftImage.GetComponent<Image>();
                tempSprite = tempImage.sprite;


                LeftImage.GetComponent<SpriteRenderer>().sprite = MiddleImage.sprite;
                MiddleImage.GetComponent<SpriteRenderer>().sprite = tempSprite;
                break;
            case 3:
                tempText = RightTitleText.text;
                RightTitleText.text = MiddleTitleText.text;
                MiddleTitleText.text = tempText;
                break;
        }
    }
}
