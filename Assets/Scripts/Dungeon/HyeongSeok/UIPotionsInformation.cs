using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPotionsInformation : MonoBehaviour
{
    [SerializeField] private GameObject[] potions;

    private const int POTIONS_IDX_OFFSET = 46;
    private const int POTIONS_N = 5;

    private const float COOL_TIME = 5.0f;

    private List<TMP_Text> potionCntList = new List<TMP_Text>();
    private List<Image> disableImageList = new List<Image>();
    private List<Coroutine> coolTimeList = new List<Coroutine>();

    private float fillHeight = 150;

    private int[] requireLevel = { 1, 1, 1, 5, 10 };

    // Start is called before the first frame update
    void Awake()
    {
        foreach (GameObject potion in potions)
        {
            potionCntList.Add(potion.transform.Find("Image/txtCntValue").GetComponent<TMP_Text>());
            disableImageList.Add(potion.transform.Find("DisableImage").GetComponent<Image>());
            coolTimeList.Add(null);
        }
    }

    public void SetPotionsUI(Dictionary<int, int> Potions, int Level)
    {

        for (int idx = 0; idx < POTIONS_N; idx++)
        {
            if (Potions.TryGetValue(POTIONS_IDX_OFFSET + idx, out int quantity))
            {
                potionCntList[idx].text = quantity.ToString();
            }
            else
            {
                potionCntList[idx].text = "0";
                disableImageList[idx].enabled = true;
            }

            //if(Level < requireLevel[idx]) 
            //    disableImageList[idx].enabled = true;
        }
    }

    public void TryUsePotion(int idx)
    {
        
        int quantity = int.Parse(potionCntList[idx].text);

        if (disableImageList[idx].enabled)
        {
            Debug.Log("쿨타임 중이거나, 사용할 수 없는 물약입니다.");
            return;
        }

        // 패킷 전송 어쩌구저쩌구
        if (quantity - 1 > 0)
        {
            potionCntList[idx].text = (quantity - 1).ToString();

            disableImageList[idx].enabled = true;
            coolTimeList[idx] = StartCoroutine(coolTime(idx, COOL_TIME));
        }
        else
        {
            potionCntList[idx].text = "0";
            disableImageList[idx].enabled = true;
        }
    }

    public void ProcessUsePotionEvent(int itemId, int quantity)
    {

        int idx = itemId - POTIONS_IDX_OFFSET;

        if (quantity > 0)
        {
            potionCntList[idx].text = (quantity).ToString();

            disableImageList[idx].enabled = true;
            coolTimeList[idx] = StartCoroutine(coolTime(idx, COOL_TIME));
        }
        else
        {
            potionCntList[idx].text = "0";
            disableImageList[idx].enabled = true;
        }

    }

    IEnumerator coolTime(int idx, float remainingTime)
    {
        while (remainingTime > 0)
        {
            // 남은 시간 갱신
            remainingTime -= Time.deltaTime;
            disableImageList[idx].rectTransform.sizeDelta = new Vector2(fillHeight * remainingTime / COOL_TIME, 100);
            yield return null; // 다음 프레임까지 대기
        }

        disableImageList[idx].enabled = false;
        disableImageList[idx].rectTransform.sizeDelta = new Vector2(fillHeight, 100);
        coolTimeList[idx] = null;
    }
}
