using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;
    public static GameManager Instance => _instance;
 
    
    private NetworkManager network;
    public static NetworkManager Network => _instance.network;


    public const string BattleScene = "Battle";
    public const string TownScene = "Town";
	public const string HatcheryScene = "Dungeon.BugHatchery";

	public S_EnterDungeon Pkt;
    public S_EnterHatchery HatcheryEnterPkt;
    public S_SetHatcheryBossHp SetBossHpPkt;
    public S_SpawnPlayerHatchery HatcherySpawnPkt;
    
    public string UserName;
    public int ClassIdx;

    public bool isSendPacketReady;
    
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        
            network = new NetworkManager();
        
            DontDestroyOnLoad(gameObject);

            isSendPacketReady = true;
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    private void Update()
    {
        if(network != null)
            network.Update();
    }

}
