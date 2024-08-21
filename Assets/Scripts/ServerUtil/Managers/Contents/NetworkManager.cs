﻿using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using UnityEditor.Sprites;

public class NetworkManager
{
	public bool IsConnected => _session.IsConnected;
	ServerSession _session = new ServerSession();

	public void Send(IMessage packet)
	{
		if (GameManager.Instance.isSendPacketReady)
			_session.Send(packet);
	}

	public void Init()
	{
		// DNS (Domain Name System)
		string host = Dns.GetHostName();
		IPHostEntry ipHost = Dns.GetHostEntry(host);
		IPAddress ipAddr = ipHost.AddressList[0];
		IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

		Connector connector = new Connector();

		connector.Connect(endPoint,
			() => { return _session; },
			1);
	}
	
	public void Init(string ipString, string portString)
	{
		IPAddress ipAddr = null;

		IPHostEntry ipHost = Dns.GetHostEntry(ipString);
		ipAddr = ipHost.AddressList[0];

		//if (IPAddress.TryParse(ipString, out ipAddr) == false)
		//{
		//	// DNS (Domain Name System)
		//	//string host = Dns.GetHostName();
		//	//IPHostEntry ipHost = Dns.GetHostEntry(host);
		//	//ipAddr = ipHost.AddressList[0];

		//	ipAddr = IPAddress.Parse("127.0.0.1");
		//}

		if (ipString == "")
			ipAddr = IPAddress.Parse("127.0.0.1");

		int port; 
		if(int.TryParse(portString, out port) == false)
			port = 3000;
		
		IPEndPoint endPoint = new IPEndPoint(ipAddr, port);

		Connector connector = new Connector();

		connector.Connect(endPoint,
			() => { return _session; },
			1);
	}

	public void Update()
	{
		List<PacketMessage> list = PacketQueue.Instance.PopAll();
		foreach (PacketMessage packet in list)
        {
            if (!(packet.Id == (byte)MsgId.SMove ||
                packet.Id == (byte)MsgId.SSpawn ||
                packet.Id == (byte)MsgId.SMoveAtHatchery ||
                packet.Id == (byte)MsgId.STryAttack))
                GameManager.Instance.isSendPacketReady = true;

            Action<PacketSession, IMessage> handler = PacketManager.Instance.GetPacketHandler(packet.Id);
			if (handler != null)
				handler.Invoke(_session, packet.Message);
		}	
	}
}
