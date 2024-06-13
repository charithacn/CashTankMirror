using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Garadge : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private Transform scroll;

    // Start is called before the first frame update
    [SerializeField] private Button gunButton;
    [SerializeField] private Button bodyButton;
    [SerializeField] private Button tyreButton;

    private Sprite clickableSprite;
    private AssetsObject assetObject;
    static Sets selectionType;

    [Header("Vehicle parts")]
    [SerializeField] private Image frontTyre;
    [SerializeField] private Image backTyre;
    [SerializeField] private Image body;
    [SerializeField] private Image bodyBack;
    [SerializeField] private Image gun;
    [SerializeField] private Image previewImage;

    [SerializeField] private Image frontTyreMain;
    [SerializeField] private Image backTyreMain;
    [SerializeField] private Image bodyMain;
    [SerializeField] private Image bodyBackMain;
    [SerializeField] private Image gunMain;
    //[SerializeField] private Image previewImageMain;



    private void Start()
    {
        gunButton.onClick.AddListener(GunAction);
        bodyButton.onClick.AddListener(BodyAction);
        tyreButton.onClick.AddListener(tyreAction);

        assetObject = Resources.Load<AssetsObject>("Object");

        frontTyre.sprite = assetObject.selectionFrontTyre;
        backTyre.sprite = assetObject.selectionRealTyre;
        body.sprite = assetObject.SelectionBody;
        bodyBack.sprite = assetObject.selectionBodyBack;
        gun.sprite = assetObject.selectionGun;

        frontTyreMain.sprite = assetObject.selectionFrontTyre;
        backTyreMain.sprite = assetObject.selectionRealTyre;
        bodyMain.sprite = assetObject.SelectionBody;
        bodyBackMain.sprite = assetObject.selectionBodyBack;
        gunMain.sprite = assetObject.selectionGun;
        //scroll.gameObject.SetActive(false);
    }

    private void tyreAction()
    {
        selectionType = Sets.tyre;
        //referNumber = 0;
        previewImage.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
        previewImage.sprite = frontTyre.sprite;
    }
    int referNumber = 0;
    public void Next()
    {
        
        switch (selectionType)
        {
            case Sets.tyre:
                Resources.Load<AssetsObject>("Object").selectionFrontTyre = Resources.Load<AssetsObject>("Object").realTyre[referNumber];
                Resources.Load<AssetsObject>("Object").selectionRealTyre = Resources.Load<AssetsObject>("Object").realTyre[referNumber];
                frontTyre.sprite = Resources.Load<AssetsObject>("Object").realTyre[referNumber]; 
                backTyre.sprite = Resources.Load<AssetsObject>("Object").realTyre[referNumber];

                frontTyreMain.sprite = Resources.Load<AssetsObject>("Object").realTyre[referNumber];
                backTyreMain.sprite = Resources.Load<AssetsObject>("Object").realTyre[referNumber];

                previewImage.sprite = Resources.Load<AssetsObject>("Object").realTyre[referNumber];

                previewImage.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
                break;
            case Sets.body:
                Resources.Load<AssetsObject>("Object").SelectionBody = Resources.Load<AssetsObject>("Object").bodyBack[referNumber];
                body.sprite = Resources.Load<AssetsObject>("Object").body[referNumber];
                bodyMain.sprite = Resources.Load<AssetsObject>("Object").body[referNumber];

                previewImage.sprite = Resources.Load<AssetsObject>("Object").body[referNumber];
                previewImage.GetComponent<RectTransform>().sizeDelta = new Vector2(100*645/362, 100);
                break;
            case Sets.gun:
                Resources.Load<AssetsObject>("Object").selectionGun = Resources.Load<AssetsObject>("Object").gun[referNumber];

                gun.sprite = Resources.Load<AssetsObject>("Object").gun[referNumber];
                gunMain.sprite = Resources.Load<AssetsObject>("Object").gun[referNumber];

                previewImage.sprite = Resources.Load<AssetsObject>("Object").gun[referNumber];
                previewImage.GetComponent<RectTransform>().sizeDelta = new Vector2(50 * 542 / 86, 50);
                break;
        }
        if(referNumber < Resources.Load<AssetsObject>("Object").realTyre.Length-1)
        {
            ++referNumber;
        } else
        {
            referNumber = 0;
        }
        
    }
    public void Previous()
    {
        switch (selectionType)
        {
            case Sets.tyre:
                Resources.Load<AssetsObject>("Object").selectionFrontTyre = Resources.Load<AssetsObject>("Object").realTyre[referNumber];
                Resources.Load<AssetsObject>("Object").selectionRealTyre = Resources.Load<AssetsObject>("Object").realTyre[referNumber];
                frontTyre.sprite = Resources.Load<AssetsObject>("Object").realTyre[referNumber];
                backTyre.sprite = Resources.Load<AssetsObject>("Object").realTyre[referNumber];
                previewImage.sprite = Resources.Load<AssetsObject>("Object").realTyre[referNumber];
                previewImage.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
                break;
            case Sets.body:
                Resources.Load<AssetsObject>("Object").SelectionBody = Resources.Load<AssetsObject>("Object").body[referNumber];
                body.sprite = Resources.Load<AssetsObject>("Object").body[referNumber];
                previewImage.sprite = Resources.Load<AssetsObject>("Object").body[referNumber];
                previewImage.GetComponent<RectTransform>().sizeDelta = new Vector2(100 * 645 / 362, 100);
                break;
            case Sets.gun:
                Resources.Load<AssetsObject>("Object").selectionGun = Resources.Load<AssetsObject>("Object").gun[referNumber];
                gun.sprite = Resources.Load<AssetsObject>("Object").gun[referNumber];
                previewImage.sprite = Resources.Load<AssetsObject>("Object").gun[referNumber];
                previewImage.GetComponent<RectTransform>().sizeDelta = new Vector2(50 * 542 / 86, 50);
                break;
        }
        if(referNumber>0)
        {
            --referNumber;
        } else
        {
            referNumber = Resources.Load<AssetsObject>("Object").realTyre.Length-1;
        }
        
    }
    private void BodyAction()
    {
       // referNumber = 0;
        selectionType = Sets.body;
        previewImage.GetComponent<RectTransform>().sizeDelta = new Vector2(100 * 645 / 362, 100);
        previewImage.sprite = body.sprite;
    }

    private void GunAction()
    {
        selectionType = Sets.gun;
       // referNumber = 0;
        previewImage.GetComponent<RectTransform>().sizeDelta = new Vector2(50 * 542 / 86, 50);
        previewImage.sprite = gun.sprite;
    }
    
    public void ContentAction()
    {
        //clickableSprite = sprite.sprite;
        //switch(selectionType)
        //{
        //    case Sets.tyre:
        //        Resources.Load<AssetsObject>("Object").selectionFrontTyre = sprite.sprite;
        //        Resources.Load<AssetsObject>("Object").selectionRealTyre = sprite.sprite;
        //       // frontTyre.sprite = sprite.sprite;
        //        //backTyre.sprite = sprite.sprite;
        //        break;
        //    case Sets.gun:
        //        Resources.Load<AssetsObject>("Object").selectionGun = sprite.sprite;
        //        gun.sprite = sprite.sprite;
        //        break;
        //    case Sets.body:
        //        Resources.Load<AssetsObject>("Object").SelectionBody = sprite.sprite;
        //        body.sprite = sprite.sprite;
        //        bodyBack.sprite = sprite.sprite;
        //        break;
            
        //}
    }
    public enum Sets{
        body ,
        tyre ,
        gun
    }
}
