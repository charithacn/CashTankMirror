using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletManager : NetworkBehaviour
{
    [SerializeField] private Image[] imageSet;
    [SerializeField] private RuntimeAnimatorController fillAnimation;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void LoadImage(int i)
    {
        print("Load bullet images" +i);
        if(i==2)
        {
            //imageSet[2].fillAmount = 0;
            imageSet[2].GetComponent<Animator>().runtimeAnimatorController = null;
            imageSet[2].GetComponent<Animator>().runtimeAnimatorController = fillAnimation;
        } else if(i==1)
        {
            imageSet[1].GetComponent<Animator>().runtimeAnimatorController = null;
            imageSet[1].GetComponent<Animator>().runtimeAnimatorController = fillAnimation;
        } else if(i==0)
        {
            imageSet[0].GetComponent<Animator>().runtimeAnimatorController = null;
            imageSet[0].GetComponent<Animator>().runtimeAnimatorController = fillAnimation;
        }
        
    }
    
    IEnumerator FillImageSmoothly(Image image)
    {
        while (image.fillAmount < 1f)
        {
            // Smoothly change the fill amount towards the target fill amount
            image.fillAmount += 10f * Time.deltaTime;

            // Clamp fill amount to prevent overshooting
            image.fillAmount = Mathf.Clamp(image.fillAmount, 0f, 1f);

            yield return null; // Wait for the next frame
        }
    }
}
