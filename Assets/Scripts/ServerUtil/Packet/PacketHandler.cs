using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

class PacketHandler
{
    #region Town

    public static void S_EnterHandler(PacketSession session, IMessage packet)
    {
        S_Enter enterPacket = packet as S_Enter;

        if (enterPacket == null)
            return;

        TownManager.Instance.Spawn(enterPacket.Player);
        TownManager.Instance.Set(enterPacket);
    }

    public static void S_LeaveHandler(PacketSession session, IMessage packet) { }

    public static void S_SpawnHandler(PacketSession session, IMessage packet)
    {
        S_Spawn spawnPacket = packet as S_Spawn;
        if (spawnPacket == null)
            return;

        var playerList = spawnPacket.Players;
        foreach (var playerInfo in playerList)
        {
            var player = TownManager.Instance.CreatePlayer(playerInfo);
            player.SetIsMine(false);
        }
    }

    public static void S_DespawnHandler(PacketSession session, IMessage packet)
    {
        S_Despawn despawnPacket = packet as S_Despawn;
        if (despawnPacket == null)
            return;

        foreach (var playerId in despawnPacket.PlayerIds)
        {
            TownManager.Instance.ReleasePlayer(playerId);
        }
    }

    public static void S_MoveHandler(PacketSession session, IMessage packet)
    {
        S_Move movePacket = packet as S_Move;
        if (movePacket == null)
            return;

        var tr = movePacket.Transform;
        Vector3 move = new Vector3(tr.PosX, tr.PosY, tr.PosZ);
        Vector3 eRot = new Vector3(0, tr.Rot, 0);

        var player = TownManager.Instance.GetPlayerAvatarById(movePacket.PlayerId);
        if (player)
        {
            player.Move(move, Quaternion.Euler(eRot));
        }
    }


    public static void S_AnimationHandler(PacketSession session, IMessage packet)
    {
        S_Animation animationPacket = packet as S_Animation;
        if (animationPacket == null)
            return;

        var animCode = animationPacket.AnimCode;

        var player = TownManager.Instance.GetPlayerAvatarById(animationPacket.PlayerId);
        if (player)
        {
            player.Animation(animCode);
        }
    }

    public static void S_ChangeCostumeHandler(PacketSession session, IMessage packet) { }

    public static void S_ChatHandler(PacketSession session, IMessage packet)
    {
        S_Chat chatPacket = packet as S_Chat;
        if (chatPacket == null)
            return;

        var msg = chatPacket.ChatMsg;

        var player = TownManager.Instance.GetPlayerAvatarById(chatPacket.PlayerId);
        if (player)
        {
            player.RecvMessage(msg);
        }
    }

    // ================100 Packet Type Handler=================== 
    public static void S_BuyItemHandler(PacketSession session, IMessage packet)
    {
        S_BuyItem pkt = packet as S_BuyItem;
        if (pkt == null)
            return;

        TownManager.Instance.myPlayer.ProcessBuyItemEvent(pkt.Item, pkt.Gold);
    }

    public static void S_SellItemHandler(PacketSession session, IMessage packet)
    {
        S_SellItem pkt = packet as S_SellItem;
        if (pkt == null)
            return;

        TownManager.Instance.myPlayer.ProcessSellItemEvent(pkt.Item, pkt.Gold);
    }

    public static void S_UseItemHandler(PacketSession session, IMessage packet)
    {
        S_UseItem pkt = packet as S_UseItem;
        if (pkt == null)
            return;

        TownManager.Instance.myPlayer.ProcessUseItemEvent(pkt.Item);
    }

    public static void S_EquipWeaponHandler(PacketSession session, IMessage packet)
    {
        S_EquipWeapon pkt = packet as S_EquipWeapon;
        if (pkt == null)
            return;

        TownManager.Instance.myPlayer.ProcessEquipEvent(pkt.ItemId);
    }

    public static void S_UnequipWeaponHandler(PacketSession session, IMessage packet)
    {
        S_UnequipWeapon pkt = packet as S_UnequipWeapon;
        if (pkt == null)
            return;

        TownManager.Instance.myPlayer.ProcessUnequipEvent(pkt.ItemType);
    }

    // ============== 100 Packet Type Handler End==================== 

    // ============== 150 Packet Type Handler Begin==================== 

    public static void S_GetQuestsHandler(PacketSession session, IMessage packet)
    {
        S_GetQuests pkt = packet as S_GetQuests;
        if (pkt == null)
            return;

        Debug.Log($"S_GetQuestsHandler access! {pkt}");
    }

    public static void S_AcceptQuestsHandler(PacketSession session, IMessage packet)
    {
        S_AcceptQuest pkt = packet as S_AcceptQuest;
        if (pkt == null)
            return;

        Debug.Log($"S_AcceptQuestsHandler access! {pkt}");
    }

    public static void S_UpdateQuestsHandler(PacketSession session, IMessage packet)
    {
        S_UpdateQuest pkt = packet as S_UpdateQuest;
        if (pkt == null)
            return;

        Debug.Log($"S_UpdateQuestsHandler access! {pkt}");
    }

    public static void S_CompleteQuestsHandler(PacketSession session, IMessage packet)
    {
        S_CompleteQuest pkt = packet as S_CompleteQuest;
        if (pkt == null)
            return;

        Debug.Log($"S_CompleteQuestsHandler access! {pkt}");
    }

    // ============== 150 Packet Type Handler End==================== 

    #endregion


    #region Battle

    public static void S_EnterDungeonHandler(PacketSession session, IMessage packet)
    {
        S_EnterDungeon pkt = packet as S_EnterDungeon;
        if (pkt == null)
            return;

        Scene scene = SceneManager.GetActiveScene();

        if (scene.name == GameManager.BattleScene)
        {
            BattleManager.Instance.Set(pkt);
        }
        else
        {
            GameManager.Instance.Pkt = pkt;
            SceneManager.LoadScene(GameManager.BattleScene);
        }
    }

    public static void S_LeaveDungeonHandler(PacketSession session, IMessage packet)
    {
        S_LeaveDungeon pkt = packet as S_LeaveDungeon;
        if (pkt == null)
            return;

        SceneManager.LoadScene(GameManager.TownScene);
    }

    public static void S_ScreenTextHandler(PacketSession session, IMessage packet)
    {
        S_ScreenText pkt = packet as S_ScreenText;
        if (pkt == null)
            return;

        if (pkt.ScreenText != null)
        {
            var uiScreen = BattleManager.Instance.UiScreen;
            uiScreen.Set(pkt.ScreenText);
        }
    }

    public static void S_ScreenDoneHandler(PacketSession session, IMessage packet)
    {
        S_ScreenDone pkt = packet as S_ScreenDone;
        if (pkt == null)
            return;

        var uiScreen = BattleManager.Instance.UiScreen;
        uiScreen.gameObject.SetActive(false);
    }

    public static void S_BattleLogHandler(PacketSession session, IMessage packet)
    {
        S_BattleLog pkt = packet as S_BattleLog;
        if (pkt == null)
            return;

        if (pkt.BattleLog != null)
        {
            var uiBattleLog = BattleManager.Instance.UiBattleLog;
            uiBattleLog.Set(pkt.BattleLog);
        }
    }

    public static void S_SetPlayerHpHandler(PacketSession session, IMessage packet)
    {
        S_SetPlayerHp pkt = packet as S_SetPlayerHp;
        if (pkt == null)
            return;

        var uiPlayer = BattleManager.Instance.UiPlayerInformation;
        uiPlayer.SetCurHP(pkt.Hp);
    }

    public static void S_SetPlayerMpHandler(PacketSession session, IMessage packet)
    {
        S_SetPlayerMp pkt = packet as S_SetPlayerMp;
        if (pkt == null)
            return;

        var uiPlayer = BattleManager.Instance.UiPlayerInformation;
        uiPlayer.SetCurMP(pkt.Mp);
    }

    public static void S_SetMonsterHpHandler(PacketSession session, IMessage packet)
    {
        S_SetMonsterHp pkt = packet as S_SetMonsterHp;
        if (pkt == null)
            return;

        BattleManager.Instance.SetMonsterHp(pkt.MonsterIdx, pkt.Hp);
    }

    public static void S_PlayerActionHandler(PacketSession session, IMessage packet)
    {
        S_PlayerAction pkt = packet as S_PlayerAction;
        if (pkt == null)
            return;

        if (pkt.TargetMonsterIdx != -1)
        {
            Monster monster = BattleManager.Instance.GetMonster(pkt.TargetMonsterIdx);
            monster.Hit();
            EffectManager.Instance.SetEffectToMonster(pkt.TargetMonsterIdx, pkt.ActionSet.EffectCode);
        }

        BattleManager.Instance.PlayerAnim(pkt.ActionSet.AnimCode);
    }

    public static void S_MonsterActionHandler(PacketSession session, IMessage packet)
    {
        S_MonsterAction pkt = packet as S_MonsterAction;
        if (pkt == null)
            return;

        Monster monster = BattleManager.Instance.GetMonster(pkt.ActionMonsterIdx);
        monster.SetAnim(pkt.ActionSet.AnimCode);

        if (pkt.ActionSet.AnimCode == 0 || pkt.ActionSet.AnimCode == 1)
        {
            if (pkt.ActionSet.EffectCode != -1)
            {
                BattleManager.Instance.PlayerHit();
            }
        }
        EffectManager.Instance.SetEffectToPlayer(pkt.ActionSet.EffectCode);
    }

    public static void S_RegisterHandler(PacketSession session, IMessage packet)
    {
        S_Register pkt = packet as S_Register;
        if (pkt == null)
            return;

        if (pkt.Success)
            TownManager.Instance.UiStart.SuccessRegist(pkt.Message);
        else
            TownManager.Instance.UiStart.FailRegist(pkt.Message);
    }

    public static void S_LogInHandler(PacketSession session, IMessage packet)
    {
        S_LogIn pkt = packet as S_LogIn;
        if (pkt == null)
            return;

        if (pkt.Success)
            TownManager.Instance.UiStart.StartGame();
        else
            TownManager.Instance.UiStart.FailLogIn(pkt.Message);
    }

    #endregion

    public static void S_EnterHatcheryHandler(PacketSession session, IMessage packet)
    {
        S_EnterHatchery pkt = packet as S_EnterHatchery;
        if (pkt == null)
            return;

        Scene scene = SceneManager.GetActiveScene();
        if (scene.name == GameManager.HatcheryScene)
        {
            HatcheryManager.Instance.Set(pkt);
        }
        else
        {
            GameManager.Instance.HatcheryEnterPkt = pkt;
            SceneManager.LoadScene(GameManager.HatcheryScene);
        }
    }

    public static void S_SpawnPlayerHatcheryHandler(PacketSession session, IMessage packet)
    {
        S_SpawnPlayerHatchery pkt = packet as S_SpawnPlayerHatchery;
        if (pkt == null)
            return;

        Scene scene = SceneManager.GetActiveScene();
        if (scene.name == GameManager.HatcheryScene)
        {
            HatcheryManager.Instance.OtherPlayerSpawn(pkt);
        }
        else
        {
            GameManager.Instance.HatcherySpawnPkt = pkt;
        }
    }

    public static void S_MoveAtHatcheryHandler(PacketSession session, IMessage packet)
    {
        S_MoveAtHatchery pkt = packet as S_MoveAtHatchery;
        if (pkt == null)
            return;

        var tr = pkt.Transform;
        Vector3 move = new Vector3(tr.PosX, tr.PosY, tr.PosZ);
        Vector3 eRot = new Vector3(0, tr.Rot, 0);

        var player = HatcheryManager.Instance.GetPlayerAvatarById(pkt.PlayerId);
        if (player)
        {
            player.Move(move, Quaternion.Euler(eRot));
        }
    }

    public static void S_SetHatcheryBossHpHandler(PacketSession session, IMessage packet)
    {
        S_SetHatcheryBossHp pkt = packet as S_SetHatcheryBossHp;
        if (pkt == null)
            return;

        Scene scene = SceneManager.GetActiveScene();
        if (scene.name == GameManager.HatcheryScene)
        {
            HatcheryManager.Instance.SetBossCurHp(pkt.BossCurHp);
        }
        else
        {
            GameManager.Instance.SetBossHpPkt = pkt;
        }

        if (pkt.BossCurHp <= 0)
        {
            HatcheryManager.Instance.monster.Dead();
        }
    }

    public static void S_TryAttackHandler(PacketSession session, IMessage packet)
    {
        S_TryAttack pkt = packet as S_TryAttack;
        if (pkt == null)
            return;

        var player = HatcheryManager.Instance.GetPlayerAvatarById(pkt.PlayerId);
        if (player)
        {
            player.AttackMotion();
        }
    }

    public static void S_BossMoveHandler(PacketSession session, IMessage packet)
    {
        S_BossMove pkt = packet as S_BossMove;
        if (pkt == null)
            return;

        // 1. 패킷을 받는 순간에는 받은 위치데이터 강제 적용
        var monster = HatcheryManager.Instance.monster;

        var tr = pkt.BossTransform;
        Vector3 move = new Vector3(tr.PosX, tr.PosY, tr.PosZ);
        Vector3 eRot = new Vector3(0, tr.Rot, 0);

        monster.SetCoordinates(move, eRot);

        // 2. 단위 벡터 보스한테 박아두고 지속적으로 움직이게 Update()
        Vector3 unitVec = new Vector3(pkt.BossUnitVector.UnitX, 0, pkt.BossUnitVector.UnitZ);

        monster.SetUnitVector(unitVec);
	}

    public static void S_BossTryAttackHandler(PacketSession session, IMessage packet)
    {
		S_BossTryAttack pkt = packet as S_BossTryAttack;
        if (pkt == null)
            return;

        HatcheryManager.Instance.bossWeapon.TryBossAttack();
    }

	public static void S_SetPlayerHpHatcheryHandler(PacketSession session, IMessage packet)
	{
		S_SetPlayerHpHatchery pkt = packet as S_SetPlayerHpHatchery;
		if (pkt == null)
			return;

        HatcheryManager.Instance.SetPlayerCurHp(pkt);
    }

	public static void S_DespawnHatcheryHandler(PacketSession session, IMessage packet)
	{
		S_DespawnHatchery pkt = packet as S_DespawnHatchery;
		if (pkt == null)
			return;

        HatcheryManager.Instance.ReleasePlayer(pkt.PlayerId);
    }
}

