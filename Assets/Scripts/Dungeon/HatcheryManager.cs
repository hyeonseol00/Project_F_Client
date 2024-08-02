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

	[SerializeField] private UIMonsterInformation uiMonsterInfo;

	private Dictionary<int, Character> playerList = new Dictionary<int, Character>();
	private Dictionary<int, string> playerDb = new Dictionary<int, string>();

	private string basePlayerPath = "DungeonPlayer/Character1";

	private void Start()
	{
		// 테스트 코드
		/* TransformInfo transform = new TransformInfo { PosX = 0.0f, PosY = 0.0f, PosZ = 0.0f, Rot = 180.0f };
		StatInfo statInfo = new StatInfo {Level = 1, Atk = 10, Def = 10, Hp = 10, Magic = 10, MaxHp = 100, MaxMp = 100, Mp = 100, Speed = 10 };
		PlayerInfo playerInfo = new PlayerInfo { Class = 1001, Nickname = "jaeseok", PlayerId = 1, Transform = transform, StatInfo = statInfo };

		Spawn(playerInfo);
		uiMonsterInfo.SetMainCamera();

		PlayerInfo playerInfo2 = new PlayerInfo { Class = 1002, Nickname = "hyeonseol", PlayerId = 2, Transform = transform, StatInfo = statInfo };
		var spawnPos = spawnArea.position;
		var player2 = CreatePlayer(playerInfo2, spawnPos);
		player2.SetIsMine(false); */
		// ~테스트 코드
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
		var playerList = spawnPacket.Players;
		foreach (var playerInfo in playerList)
		{
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
		}
		if (pkt.BossMaxHp != null && pkt.BossName != null)
		{
			uiMonsterInfo.SetFullHP(pkt.BossMaxHp, false);
			uiMonsterInfo.SetName(pkt.BossName);
		}
	}

	public void SetBossCurHp(int hp)
	{
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
		Vector3 eRot = new Vector3(0, 0, 0);
		var spawnRot = Quaternion.Euler(eRot);

		var spawnPos = spawnArea.position;
		spawnPos.x += tr.PosX;
		spawnPos.z += tr.PosZ;

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
	}

	public Character GetPlayerAvatarById(int playerId)
	{
		if (playerList.ContainsKey(playerId))
			return playerList[playerId];

		return null;
	}
}
