using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Google.Protobuf.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class HatcheryManager : MonoBehaviour
{
	private static HatcheryManager _instance = null;
	public static HatcheryManager Instance => _instance;

	[SerializeField] private Transform spawnArea;
	public Character myPlayer { get; private set; }

	[SerializeField] public Enemy monster;
	[SerializeField] private UIMonsterInformation uiMonsterInfo;

	[SerializeField] public BossAttack bossWeapon;

	private Dictionary<int, Character> playerList = new Dictionary<int, Character>();
	private Dictionary<int, string> playerDb = new Dictionary<int, string>();

	private string basePlayerPath = "DungeonPlayer/Character1";

	[SerializeField] private HatcheryUIManager hatcheryUIManager;

	public int phase = 1;

	private void Start()
	{
		// �׽�Ʈ �ڵ�
		/* TransformInfo transform = new TransformInfo { PosX = 0.0f, PosY = 0.0f, PosZ = 0.0f, Rot = 180.0f };
		StatInfo statInfo = new StatInfo {Level = 1, Atk = 10, Def = 10, Hp = 10, Magic = 10, MaxHp = 100, MaxMp = 100, Mp = 100, Speed = 10 };
		PlayerInfo playerInfo = new PlayerInfo { Class = 1001, Nickname = "jaeseok", PlayerId = 1, Transform = transform, StatInfo = statInfo };

		Spawn(playerInfo);
		uiMonsterInfo.SetMainCamera();

		PlayerInfo playerInfo2 = new PlayerInfo { Class = 1002, Nickname = "hyeonseol", PlayerId = 2, Transform = transform, StatInfo = statInfo };
		var spawnPos = spawnArea.position;
		var player2 = CreatePlayer(playerInfo2, spawnPos);
		player2.SetIsMine(false); */
		// ~�׽�Ʈ �ڵ�
	}

	private void Awake()
	{
		_instance = this;

		playerDb.Add(1001, "DungeonPlayer/Character1");
		playerDb.Add(1002, "DungeonPlayer/Character2");
		playerDb.Add(1003, "DungeonPlayer/Character3");
		playerDb.Add(1004, "DungeonPlayer/Character4");
		playerDb.Add(1005, "DungeonPlayer/Character5");

		if (GameManager.Instance.HatcheryEnterPkt != null)
			Set(GameManager.Instance.HatcheryEnterPkt);
		GameManager.Instance.HatcheryEnterPkt = null;

		if (GameManager.Instance.SetBossHpPkt != null)
			SetBossCurHp(GameManager.Instance.SetBossHpPkt.BossCurHp);
		GameManager.Instance.SetBossHpPkt = null;

		if (GameManager.Instance.HatcherySpawnPkt != null)
			OtherPlayerSpawn(GameManager.Instance.HatcherySpawnPkt);
		GameManager.Instance.HatcherySpawnPkt = null;
	}

	public void OtherPlayerSpawn(S_SpawnPlayerHatchery spawnPacket)
	{
		var players = spawnPacket.Players;
		var isMine = false;

		foreach (var playerInfo in players)
		{
			// 새로운 플레이어가 들어왔다면 UI추가
            if (!playerList.ContainsKey(playerInfo.PlayerId)){

				hatcheryUIManager.OnPlayerEnter(playerInfo, isMine);
			}

			var player = CreatePlayer(playerInfo);
			player.SetIsMine(false);
		}

	}

	public void Set(S_EnterHatchery pkt)
	{
		if (pkt.Player != null)
		{
			Spawn(pkt.Player);
			uiMonsterInfo.SetMainCamera();

			// 본인 UI추가.
			var isMine = true;
			hatcheryUIManager.OnPlayerEnter(pkt.Player, isMine);
		}
		if (pkt.BossMaxHp > 0 && pkt.BossName != null && pkt.BossSpeed > 0)
		{
			uiMonsterInfo.SetFullHP(pkt.BossMaxHp, false);
			uiMonsterInfo.SetName(pkt.BossName);
			monster.speed = pkt.BossSpeed;
		}
		if (pkt.BossTransformInfo != null)
		{
			var bossTr = pkt.BossTransformInfo;
			Vector3 pos = new Vector3(bossTr.PosX, bossTr.PosY, bossTr.PosZ);
			Quaternion rot = Quaternion.Euler(0, bossTr.Rot, 0);

			monster.transform.position = pos;
			monster.transform.rotation = rot;
		}
	
	}

	public void SetBossCurHp(int hp)
	{
		//if (uiMonsterInfo.GetCurHP() > hp)
		//{
		//	monster.HitAnimation();
		//}
		uiMonsterInfo.SetCurHP(hp);
	}

	public void Spawn(PlayerInfo playerInfo)
	{
		myPlayer = CreatePlayer(playerInfo);
		myPlayer.SetIsMine(true);
	}

	public Character CreatePlayer(PlayerInfo playerInfo)
	{
		var tr = playerInfo.Transform;
		Vector3 eRot = new Vector3(0, tr.Rot, 0);
		var spawnRot = Quaternion.Euler(eRot);
        var spawnPos = new Vector3(tr.PosX, tr.PosY, tr.PosZ);

        var playerId = playerInfo.PlayerId;
		var playerResPath = playerDb.GetValueOrDefault(playerInfo.Class, basePlayerPath);
		var playerRes = Resources.Load<Character>(playerResPath);
		var player = Instantiate(playerRes, spawnPos, spawnRot);
		// player.Move(spawnPos, spawnRot);
		player.SetPlayerId(playerId);
		player.SetNickname(playerInfo.Nickname);

		if (playerList.ContainsKey(playerId))
		{
			var prevPlayer = playerList[playerId];
			playerList[playerId] = player;

			if (prevPlayer)
				Destroy(prevPlayer.gameObject);
		}
		else
		{
			playerList.Add(playerId, player);
		}

		return player;
	}

	public void ReleasePlayer(int playerId)
	{
		if (!playerList.ContainsKey(playerId))
			return;

		var player = playerList[playerId];
		playerList.Remove(playerId);

		Destroy(player.gameObject);
		hatcheryUIManager.OnOtherPlayerExit(playerId);
	}

	public Character GetPlayerAvatarById(int playerId)
	{
		if (playerList.ContainsKey(playerId))
			return playerList[playerId];

		return null;
	}

	public void LeaveHatchery()
	{
		C_LeaveHatchery leavePacket = new C_LeaveHatchery { };

		GameManager.Network.Send(leavePacket);
	}

	public void SetPlayerCurHpMp(S_SetPlayerHpMpHatchery pkt)
	{
		
		Character hittedPlayer = GetPlayerAvatarById(pkt.PlayerId);

		// 수신받은 playerId를 가진 캐릭터가 던전에 없는 경우 오류 출력
		if (!hittedPlayer)
        {
			Debug.LogError($"PlayerID: {pkt.PlayerId} is not exist!");
			return;
		}

		// 피격 대상이 본인 캐릭터인 경우와 아닌 경우를 구분해서 로직 실행
		if (pkt.PlayerId == myPlayer.PlayerId)
		{
			var isMine = true;
			bool isAttcked = hatcheryUIManager.SetPlayerCurHPMP(pkt.PlayerId, pkt.PlayerCurHp, pkt.PlayerCurMp, isMine);
			if (pkt.PlayerCurHp <= 0)
			{
				hittedPlayer.gameObject.layer = 11; // CharaterDead
				hittedPlayer.DeadMotion();
				GameObject.Find("UIResult").transform.Find("LosePopup").gameObject.SetActive(true);
			}
            else if(isAttcked)
            {
				hittedPlayer.HitMotion();
			}
				
			
		}
        else
        {
			var isMine = false;
			bool isAttcked = hatcheryUIManager.SetPlayerCurHPMP(pkt.PlayerId, pkt.PlayerCurHp, pkt.PlayerCurHp, isMine);
			if (pkt.PlayerCurHp <= 0)
				hittedPlayer.DeadMotion();
			else if (isAttcked)
			{
				hittedPlayer.HitMotion();
			}
		}

		return;
	}

	public void SetPotion(S_TryUsePotion pkt)
	{
		hatcheryUIManager.SetPotion(pkt.ItemId, pkt.Quantity);
		return;
	}

	public void EnterSecondPhase(float bindTime, float updatedBossSpeed)
	{
		phase = 2;
		hatcheryUIManager.StartCoroutine("DisplayNotification", $"2페이즈 돌입: 보스의 움직임이 빨라지고, 플레이어들이 {bindTime.ToString("F1")}초동안 움직임이 봉인됩니다!");
		monster.speed = updatedBossSpeed;
		myPlayer.StartCoroutine("BindMovement", bindTime);
	}

	public void EnterThridPhase(float countTime)
	{
		phase = 3;
		hatcheryUIManager.StartCoroutine("DisplayNotification", $"3페이즈 돌입: {countTime.ToString("F1")}초 뒤에 광역 즉사기를 시전합니다.");
		hatcheryUIManager.startDeathCounter(countTime);
	}

	public void EndThridPhase()
	{
		hatcheryUIManager.stopDeathCounter();
	}

	public void SetNotification(string msg)
	{
		hatcheryUIManager.StartCoroutine("DisplayNotification", msg);
	}

	public void SelectButton(int btnIdx)
	{
		monster.OffBtnsInteractable();

        C_HatcherySelectReward response = new C_HatcherySelectReward { SelectedBtn = btnIdx };

        GameManager.Network.Send(response);
    }
}
