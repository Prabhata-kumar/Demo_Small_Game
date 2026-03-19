using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntranceScript : UIScreen
{
    public List<Sprite> imageSprites;
    [SerializeField] private GameObject image;
    [SerializeField] private Scrollbar scrollbar;
    [SerializeField] private Text scrollBarValue;

    private Image imgComponent;
    private Vector3 imageScale;
    private bool startAnimation = false;
    private int index = 0;

    private void Awake()
    {
        imageScale = image.transform.localScale;
        imgComponent = image.GetComponent<Image>();
    }

    private void OnEnable()
    {
        startAnimation = true;
        StartCoroutine(WaitForAnimation());
        Animation(0);
    }

    private void OnDisable()
    {
        startAnimation = false;
    }

    public void Animation(int scale)
    {
        if (!startAnimation) return;

        if (scale > 0)
        {
            image.transform.DOScale(imageScale, 0.5f).OnComplete(() => {
                Animation(0);
            });
        }
        else
        {
            image.transform.DOScale(Vector3.zero, 0.5f).OnComplete(() => {
                ChangeImage();
                Animation(1);
            });
        }
    }

    private void ChangeImage()
    {
        if (index >= imageSprites.Count - 1)
            index = 0;
        else
            index++;

        imgComponent.sprite = imageSprites[index];
    }

    IEnumerator WaitForAnimation()
    {
        ScorllBarValue(0.3f);
        yield return new WaitForSeconds(1f);
        ScorllBarValue(0.5f);
        yield return new WaitForSeconds(1f);
        ScorllBarValue(0.8f);
        yield return new WaitForSeconds(1f);
        ScorllBarValue(1f);
    }

    public void ScorllBarValue(float value)
    {
        scrollbar.value = value;
        scrollBarValue.text = (int)(value * 100) + "%";

        if(value >= 1f)
        {
            UIManager.LoadMainMenu?.Invoke();
            gameObject.SetActive(false);
            Debug.Log("Scrollbar reached 100% 1st Load Complete");
        }
    }
}