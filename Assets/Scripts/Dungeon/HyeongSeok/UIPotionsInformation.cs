using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIPotionsInformation : MonoBehaviour
{
    [SerializeField] private GameObject[] potions;

    private const int POTIONS_IDX_OFFSET = 46;
    private const int POTIONS_N = 5;

    private const float COOL_TIME = 5.0f;

    private List<TMP_Text> potionCntList = new List<TMP_Text>();
    private List<GameObject> disableImageList = new List<GameObject>();
    private List<Coroutine> coolTimeList = new List<Coroutine>();

    // Start is called before the first frame update
    void Awake()
    {
        foreach (GameObject potion in potions)
        {
            potionCntList.Add(potion.transform.Find("Image/txtCntValue").GetComponent<TMP_Text>());
            disableImageList.Add(potion.transform.Find("DisableImage").gameObject);
            coolTimeList.Add(null);
        }
    }

    public void SetPotionsUI(Dictionary<int, int> Potions)
    {

        for(int idx = 0; idx < POTIONS_N; idx++)
        {
            if (Potions.TryGetValue(POTIONS_IDX_OFFSET + idx, out int quantity))
            {
                potionCntList[idx].text = quantity.ToString();
            }
            else
            {
                potionCntList[idx].text = "0";
                disableImageList[idx].SetActive(true);
            }
           
        }
    }

    public void usePotion(int idx)
    {
        // 패킷 전송 어쩌구저쩌구
        int quantity = int.Parse(potionCntList[idx].text);

        if (quantity == 0 || coolTimeList[idx] != null)
        {
            Debug.Log("쿨타임 중이거나, 물약이 없습니다");
            return;
        }

        if(quantity - 1 > 0)
        {
            potionCntList[idx].text = (quantity-1).ToString();

            coolTimeList[idx] = StartCoroutine("coolTime", idx);
        }
        else
        {
            potionCntList[idx].text = "0";
            disableImageList[idx].SetActive(true);
        }
    }

    IEnumerator coolTime(int idx)
    {
        disableImageList[idx].SetActive(true);
        yield return new WaitForSeconds(COOL_TIME);
        disableImageList[idx].SetActive(false);
        coolTimeList[idx] = null;
    }

    public void setPotionQuantity(int itemId, int quantity)
    {
        int idx = itemId - POTIONS_IDX_OFFSET;

        if (quantity > 0)
        {
            potionCntList[idx].text = (quantity).ToString();
        }
        else
        {
            potionCntList[idx].text = "0";
            disableImageList[idx].SetActive(true);
        }
    }
}
