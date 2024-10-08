using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;
using UnityEngine;

class PacketManager
{
	#region Singleton
	static PacketManager _instance = new PacketManager();
	public static PacketManager Instance { get { return _instance; } }
	#endregion

	PacketManager()
	{
		Register();
	}

	Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>>();
	Dictionary<ushort, Action<PacketSession, IMessage>> _handler = new Dictionary<ushort, Action<PacketSession, IMessage>>();
		
	public Action<PacketSession, IMessage, ushort> CustomHandler { get; set; }

	public void Register()
	{		
		_onRecv.Add((ushort)MsgId.SEnter, MakePacket<S_Enter>);
		_handler.Add((ushort)MsgId.SEnter, PacketHandler.S_EnterHandler);		
		_onRecv.Add((ushort)MsgId.SSpawn, MakePacket<S_Spawn>);
		_handler.Add((ushort)MsgId.SSpawn, PacketHandler.S_SpawnHandler);		
		// _onRecv.Add((ushort)MsgId.SLeave, MakePacket<S_Leave>);
		// _handler.Add((ushort)MsgId.SLeave, PacketHandler.S_LeaveHandler);		
		_onRecv.Add((ushort)MsgId.SDespawn, MakePacket<S_Despawn>);
		_handler.Add((ushort)MsgId.SDespawn, PacketHandler.S_DespawnHandler);		
		_onRecv.Add((ushort)MsgId.SMove, MakePacket<S_Move>);
		_handler.Add((ushort)MsgId.SMove, PacketHandler.S_MoveHandler);		
		_onRecv.Add((ushort)MsgId.SAnimation, MakePacket<S_Animation>);
		_handler.Add((ushort)MsgId.SAnimation, PacketHandler.S_AnimationHandler);		
		// _onRecv.Add((ushort)MsgId.SChangeCostume, MakePacket<S_ChangeCostume>);
		// _handler.Add((ushort)MsgId.SChangeCostume, PacketHandler.S_ChangeCostumeHandler);		
		_onRecv.Add((ushort)MsgId.SChat, MakePacket<S_Chat>);
		_handler.Add((ushort)MsgId.SChat, PacketHandler.S_ChatHandler);		
		_onRecv.Add((ushort)MsgId.SEnterDungeon, MakePacket<S_EnterDungeon>);
		_handler.Add((ushort)MsgId.SEnterDungeon, PacketHandler.S_EnterDungeonHandler);		
		_onRecv.Add((ushort)MsgId.SLeaveDungeon, MakePacket<S_LeaveDungeon>);
		_handler.Add((ushort)MsgId.SLeaveDungeon, PacketHandler.S_LeaveDungeonHandler);		
		_onRecv.Add((ushort)MsgId.SScreenText, MakePacket<S_ScreenText>);
		_handler.Add((ushort)MsgId.SScreenText, PacketHandler.S_ScreenTextHandler);		
		_onRecv.Add((ushort)MsgId.SScreenDone, MakePacket<S_ScreenDone>);
		_handler.Add((ushort)MsgId.SScreenDone, PacketHandler.S_ScreenDoneHandler);		
		_onRecv.Add((ushort)MsgId.SBattleLog, MakePacket<S_BattleLog>);
		_handler.Add((ushort)MsgId.SBattleLog, PacketHandler.S_BattleLogHandler);		
		_onRecv.Add((ushort)MsgId.SSetPlayerHp, MakePacket<S_SetPlayerHp>);
		_handler.Add((ushort)MsgId.SSetPlayerHp, PacketHandler.S_SetPlayerHpHandler);		
		_onRecv.Add((ushort)MsgId.SSetPlayerMp, MakePacket<S_SetPlayerMp>);
		_handler.Add((ushort)MsgId.SSetPlayerMp, PacketHandler.S_SetPlayerMpHandler);		
		_onRecv.Add((ushort)MsgId.SSetMonsterHp, MakePacket<S_SetMonsterHp>);
		_handler.Add((ushort)MsgId.SSetMonsterHp, PacketHandler.S_SetMonsterHpHandler);		
		_onRecv.Add((ushort)MsgId.SPlayerAction, MakePacket<S_PlayerAction>);
		_handler.Add((ushort)MsgId.SPlayerAction, PacketHandler.S_PlayerActionHandler);		
		_onRecv.Add((ushort)MsgId.SMonsterAction, MakePacket<S_MonsterAction>);
		_handler.Add((ushort)MsgId.SMonsterAction, PacketHandler.S_MonsterActionHandler);

		_onRecv.Add((ushort)MsgId.SBuyItem, MakePacket<S_BuyItem>);
		_handler.Add((ushort)MsgId.SBuyItem, PacketHandler.S_BuyItemHandler);
		_onRecv.Add((ushort)MsgId.SSellItem, MakePacket<S_SellItem>);
		_handler.Add((ushort)MsgId.SSellItem, PacketHandler.S_SellItemHandler);

		_onRecv.Add((ushort)MsgId.SUseItem, MakePacket<S_UseItem>);
		_handler.Add((ushort)MsgId.SUseItem, PacketHandler.S_UseItemHandler);
		_onRecv.Add((ushort)MsgId.SEquipWeapon, MakePacket<S_EquipWeapon>);
		_handler.Add((ushort)MsgId.SEquipWeapon, PacketHandler.S_EquipWeaponHandler);
		_onRecv.Add((ushort)MsgId.SUnequipWeapon, MakePacket<S_UnequipWeapon>);
		_handler.Add((ushort)MsgId.SUnequipWeapon, PacketHandler.S_UnequipWeaponHandler);

		_onRecv.Add((ushort)MsgId.SRegister, MakePacket<S_Register>);
		_handler.Add((ushort)MsgId.SRegister, PacketHandler.S_RegisterHandler);
		_onRecv.Add((ushort)MsgId.SLogIn, MakePacket<S_LogIn>);
		_handler.Add((ushort)MsgId.SLogIn, PacketHandler.S_LogInHandler);
		_onRecv.Add((ushort)MsgId.SEnterHatchery, MakePacket<S_EnterHatchery>);
		_handler.Add((ushort)MsgId.SEnterHatchery, PacketHandler.S_EnterHatcheryHandler); 
		_onRecv.Add((ushort)MsgId.SSpawnPlayerHatchery, MakePacket<S_SpawnPlayerHatchery>);
		_handler.Add((ushort)MsgId.SSpawnPlayerHatchery, PacketHandler.S_SpawnPlayerHatcheryHandler);
		_onRecv.Add((ushort)MsgId.SMoveAtHatchery, MakePacket<S_MoveAtHatchery>);
		_handler.Add((ushort)MsgId.SMoveAtHatchery, PacketHandler.S_MoveAtHatcheryHandler);
		_onRecv.Add((ushort)MsgId.SSetHatcheryBossHp, MakePacket<S_SetHatcheryBossHp>);
		_handler.Add((ushort)MsgId.SSetHatcheryBossHp, PacketHandler.S_SetHatcheryBossHpHandler);
		_onRecv.Add((ushort)MsgId.STryAttack, MakePacket<S_TryAttack>);
		_handler.Add((ushort)MsgId.STryAttack, PacketHandler.S_TryAttackHandler);
		_onRecv.Add((ushort)MsgId.SBossMove, MakePacket<S_BossMove>);
		_handler.Add((ushort)MsgId.SBossMove, PacketHandler.S_BossMoveHandler);
		_onRecv.Add((ushort)MsgId.SBossTryAttack, MakePacket<S_BossTryAttack>);
		_handler.Add((ushort)MsgId.SBossTryAttack, PacketHandler.S_BossTryAttackHandler);
		_onRecv.Add((ushort)MsgId.SSetPlayerHpmpHatchery, MakePacket<S_SetPlayerHpMpHatchery>);
		_handler.Add((ushort)MsgId.SSetPlayerHpmpHatchery, PacketHandler.S_SetPlayerHpMpHatcheryHandler);
		_onRecv.Add((ushort)MsgId.SDespawnHatchery, MakePacket<S_DespawnHatchery>);
		_handler.Add((ushort)MsgId.SDespawnHatchery, PacketHandler.S_DespawnHatcheryHandler);
		_onRecv.Add((ushort)MsgId.STryUsePotion, MakePacket<S_TryUsePotion>);
		_handler.Add((ushort)MsgId.STryUsePotion, PacketHandler.S_TryUsePotionHandler);
		_onRecv.Add((ushort)MsgId.SEnterSecondPhase, MakePacket<S_EnterSecondPhase>);
		_handler.Add((ushort)MsgId.SEnterSecondPhase, PacketHandler.S_EnterSecondPhaseHandler);
		_onRecv.Add((ushort)MsgId.SEnterThirdPhase, MakePacket<S_EnterThirdPhase>);
		_handler.Add((ushort)MsgId.SEnterThirdPhase, PacketHandler.S_EnterThirdPhaseHandler);
		_onRecv.Add((ushort)MsgId.SDisplayNotificationHatchery, MakePacket<S_DisplayNotificationHatchery>);
		_handler.Add((ushort)MsgId.SDisplayNotificationHatchery, PacketHandler.S_DisplayNotificationInHatcheryHandler);
        _onRecv.Add((ushort)MsgId.SKillBoss, MakePacket<S_KillBoss>);
        _handler.Add((ushort)MsgId.SKillBoss, PacketHandler.S_KillBossHandler);
        _onRecv.Add((ushort)MsgId.SHatcheryConfirmReward, MakePacket<S_HatcheryConfirmReward>);
        _handler.Add((ushort)MsgId.SHatcheryConfirmReward, PacketHandler.S_HatcheryConfirmRewardHandler);
		_onRecv.Add((ushort)MsgId.STrySkill, MakePacket<S_TrySkill>);
		_handler.Add((ushort)MsgId.STrySkill, PacketHandler.S_TrySkillHandler);
	}

	public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
	{
		ushort count = 0;

		// 크기를 4바이트로 읽음
		int size = BitConverter.ToInt32(buffer.Array, buffer.Offset);
		count += 4;

		// 아이디를 1바이트로 읽음
		byte id = buffer.Array[buffer.Offset + count];
		count += 1;

		Action<PacketSession, ArraySegment<byte>, ushort> action = null;
		if (_onRecv.TryGetValue(id, out action))
			action.Invoke(session, buffer, id);
	}

	void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer, ushort id) where T : IMessage, new()
	{
		T pkt = new T();
		pkt.MergeFrom(buffer.Array, buffer.Offset + 5, buffer.Count - 5);
		
		if (CustomHandler != null)
		{
			CustomHandler.Invoke(session, pkt, id);
		}
		else
		{
			Action<PacketSession, IMessage> action = null;
			if (_handler.TryGetValue(id, out action))
				action.Invoke(session, pkt);	
		}
	}

	public Action<PacketSession, IMessage> GetPacketHandler(ushort id)
	{
		Action<PacketSession, IMessage> action = null;
		if (_handler.TryGetValue(id, out action))
			return action;
		return null;
	}
}