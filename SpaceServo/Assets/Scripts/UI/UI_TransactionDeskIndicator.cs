using System;
using TMPro;
using UnityEngine;

public class UI_TransactionDeskIndicator : MonoBehaviour
{
    [Header("Money Indicator")]
    [SerializeField] TMP_Text moneyIndicator;
    [SerializeField] float startingHeight;
    [SerializeField] float endingHeight;
    [SerializeField] float moveSpeed;
    [SerializeField] RectTransform rectTransform;

    public void Initilize(int moneyAmount, float rotation)
    {
        rectTransform.localEulerAngles = new Vector3( Game.CameraController.Angle, rotation, 0 );
        moneyIndicator.text = "$" + moneyAmount.ToString();
        rectTransform.anchoredPosition3D = new Vector3(
            rectTransform.anchoredPosition3D.x, 
            startingHeight, 
            rectTransform.anchoredPosition3D.z);

        Color color = moneyIndicator.color;
        color.a = 1;
        moneyIndicator.color = color;
    }

    private void CorrectRotation()
    {
        
    }

    public void Update()
    {
        if (rectTransform.anchoredPosition3D.y < endingHeight)
        {
            float height = rectTransform.anchoredPosition3D.y;
            height += Time.deltaTime * moveSpeed;
            rectTransform.anchoredPosition3D = new Vector3(
                rectTransform.anchoredPosition3D.x,
                height,
                rectTransform.anchoredPosition3D.z);

            Color color = moneyIndicator.color;
            float progress = ((height - startingHeight) / (endingHeight - startingHeight));
            if (progress > 0.5)
            {
                color.a = 1 - ((progress - 0.5f)*2);
                if (color.a < 0) color.a *= -1;
                moneyIndicator.color = color;
            }
            
        }

        if (rectTransform.anchoredPosition3D.y >= endingHeight)
        {
            gameObject.SetActive(false);
        }

    }

}
