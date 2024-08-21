using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf.Protocol;

public class HatcheryUIManager : MonoBehaviour
{
    [SerializeField] private UIPlayerInformationInHatchery mPlayerUI; // 본인 UI
    [SerializeField] private UIPotionsInformation mPotionUI; // 본인 UI

    [SerializeField] private GameObject playerUIPrefab;  // UI Prefab 참조
    [SerializeField] private Transform uiParent;         // UI를 배치할 부모 객체

    private List<UIPlayerInformationInHatchery> playerUIList = new List<UIPlayerInformationInHatchery>();

    private const int MAXPEOPLE = 4;

    // 플레이어가 던전에 입장했을 때 호출
    public void OnPlayerEnter(PlayerInfo playerInfo, bool isMine)
    {
        if (isMine)
        {
            mPlayerUI.Set(playerInfo);
            mPlayerUI.SetPotions(playerInfo.Inven, playerInfo.StatInfo.Level);
            return;
        }

        if (playerUIList.Count + 1 > MAXPEOPLE)
        {
            Debug.LogWarning($"HatcheryUIManager error: exceed peoples in hatchery. current people: {playerUIList.Count + 1}");
            return;
        }

        // UI를 생성하고 playerList에 추가한다.
        GameObject playerUIObject = Instantiate(playerUIPrefab, uiParent);
        UIPlayerInformationInHatchery playerUI = playerUIObject.GetComponent<UIPlayerInformationInHatchery>();
        playerUI.Set(playerInfo);
        playerUIObject.SetActive(true);
        playerUIList.Add(playerUI);
    }

    // 플레이어가 던전에서 퇴장했을 때 호출
    public void OnOtherPlayerExit(int playerId)
    {
        // UI를 파괴하고 playerList에서 제거한다.
        UIPlayerInformationInHatchery playerUI = playerUIList.Find(ui => ui.playerId == playerId);
        if (playerUI != null)
        {
            playerUIList.Remove(playerUI);
            Destroy(playerUI.gameObject);
            UpdateUIPositions();
        }
    }

    // 플레이어가 던전에서 HP/MP가 갱신되었을 때 호출
    public bool SetPlayerCurHPMP(int playerId, float updatedHp, float updatedMp, bool isMine)
    {
        bool isAttcked = false;

        if (isMine)
        {
            isAttcked = mPlayerUI.SetCurHP(updatedHp);
            mPlayerUI.SetCurMP(updatedMp);
            return isAttcked;
        }

        UIPlayerInformationInHatchery playerUI = playerUIList.Find(ui => ui.playerId == playerId);
        isAttcked = playerUI.SetCurHP(updatedHp);
        playerUI.SetCurHP(updatedMp);
        return isAttcked;
    }

    public void SetPotion(int itemId, int quantity)
    {
        mPotionUI.ProcessUsePotionEvent(itemId, quantity);
    }

    // UI 패널의 위치를 업데이트 (간격 유지)
    private void UpdateUIPositions()
    {
        for (int i = 0; i < playerUIList.Count; i++)
        {
            playerUIList[i].transform.SetSiblingIndex(i);
        }
    }

}
