﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


//Clase para los clientes
public class ServerClient {
    public int connectionId;
    public string playerName;
    public int id;
}

public class Servidor : MonoBehaviour {

    private const int MAX_CONNECTION = 100;
    private int port = 5701;

    private int hostId;
    private int webHostId;

    private int reliableChannel;
    private int unReliableChannel;

    public GameObject LoggingText;
    private int logCounter = 0;

    private bool isStarted = false;
    private byte error;
    private ServerClient jugador1, jugador2;

    private List<ServerClient> clients = new List<ServerClient>();

    bool primerJugadoCreado = false;
    int cantidadJugadores = 0;
    GameObject posJ1;
    private SimpleAES simpleAES;

    private void Start() {


        NetworkTransport.Init();
        ConnectionConfig cc = new ConnectionConfig();

        reliableChannel = cc.AddChannel(QosType.Reliable);
        unReliableChannel = cc.AddChannel(QosType.Unreliable);

        HostTopology topo = new HostTopology(cc, MAX_CONNECTION);
        hostId = NetworkTransport.AddHost(topo, port, null);
        webHostId = NetworkTransport.AddWebsocketHost(topo, port, null);
        simpleAES = new SimpleAES();
        isStarted = true;

        posJ1 = new GameObject();
        posJ1.transform.position = new Vector3(0, 0, -300);


    }

    private void Update() {

        if (!isStarted) {
            return;
        }

        int recHostId;
        int connectionId;
        int channelId;
        byte[] recBuffer = new byte[1024];
        int bufferSize = 1024;
        int dataSize;
        byte error;


        NetworkEventType recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out error);
        switch (recData) {
            case NetworkEventType.Nothing:
                break;

            case NetworkEventType.ConnectEvent:
                OnConnection(connectionId);
                break;

            case NetworkEventType.DataEvent:
               // string msgDebug = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
               // ToLog("QUE RECIBO DE CADA CONEXION" + connectionId + ": " + msgDebug);
                byte[] msgToDecrypt = new byte[dataSize];
                Array.Copy(recBuffer, 0, msgToDecrypt, 0, dataSize);
                string msg = simpleAES.Decrypt(msgToDecrypt);
                ToLog(msg);
                string[] splitData = msg.Split('|');

                switch (splitData[0]) {
                    case "NAMEIS":
                        if (ServerMock.CheckLoginDetailsMock(splitData[1])) {
                            sendUserCreationMessages(connectionId, splitData[1], "99");
                            return;
                        }
                        SymfonyConnect(connectionId, splitData[1], splitData[2]);

                        break;

                    case "CNN":
                        break;

                    case "DC":
                        break;

                    case "EMPEZAR":
                        Send("EMPEZAR|", reliableChannel, clients);
                        break;

                    case "INV":
                        if (ServerMock.CheckHatsMock(splitData[1])) {
                            Send("INV|" + splitData[1] + '|' + connectionId + '|' + -1, reliableChannel, clients);
                            return;
                        }
                        inventario(int.Parse(splitData[1]), connectionId, splitData[2]);
                        break;

                    case "POS1":
                        Send("POS1|" + splitData[1] + "|" + splitData[2], reliableChannel, clients);
                        cantidadJugadores++;
                        if (cantidadJugadores == 2) {
                            Send("SPAWN|a", reliableChannel, clients);
                        }
                        break;

                    case "POS2":
                        Send("POS2|" + splitData[1] + "|" + splitData[2], reliableChannel, clients);
                        break;

                    #region MOVEMENT MESSAGES
                    case "DER":
                        Send("DER|" + splitData[1], reliableChannel, clients);
                        break;

                    case "IZQ":
                        Send("IZQ|" + splitData[1], reliableChannel, clients);
                        break;

                    case "PAR":
                        Send("PAR|" + splitData[1], reliableChannel, clients);
                        break;

                    case "SAL":
                        Send("SAL|" + splitData[1], reliableChannel, clients);
                        break;

                    case "ARR":
                        Send("ARR|" + splitData[1], reliableChannel, clients);
                        break;

                    case "ABA":
                        Send("ABA|" + splitData[1], reliableChannel, clients);
                        break;

                    case "DIS":
                        Send("DIS|" + splitData[1], reliableChannel, clients);
                        break;

                    case "GOL":
                        Send("GOL|" + splitData[1], reliableChannel, clients);
                        break;
                    #endregion

                    default:
                        ToLog("Mensaje Invalido" + msg);
                        break;

                }
                break;

            case NetworkEventType.DisconnectEvent:
                break;
        }

    }

    private void sendUserCreationMessages(int cnnId, string playerName, string userId) {
        if (!primerJugadoCreado) {
            jugador1 = new ServerClient();
            jugador1.id = int.Parse(userId);
            jugador1.playerName = playerName;
            jugador1.connectionId = cnnId;
            Send("CNN|" + playerName + '|' + cnnId + '|' + userId, reliableChannel, clients);
        } else {
            Send("CNN|" + jugador1.playerName + '|' + jugador1.connectionId + '|' + userId, reliableChannel, clients);
            Send("CNN|" + playerName + '|' + cnnId + '|' + userId, reliableChannel, clients);
        }
    }

    private void OnConnection(int cnnId) {
 
        ServerClient c = new ServerClient();
        c.connectionId = cnnId;
        c.playerName = "TEMP";
        clients.Add(c);

        string msg = "ASKNAME|" + cnnId + "|";

        foreach (ServerClient sc in clients)
            msg += sc.playerName + "%" + sc.connectionId + '|';

        msg = msg.Trim('|');

        Send(msg, reliableChannel, cnnId);

    }

    private void Send(string message, int channelId, int cnnId) {
        List<ServerClient> c = new List<ServerClient>();
        c.Add(clients.Find(x => x.connectionId == cnnId));
        Send(message, channelId, c);
    }

    private void Send(string message, int channelId, List<ServerClient> c) {
        byte[] msg = simpleAES.Encrypt(message);
        ToLog("Sending: " + message);
       // byte[] msg = Encoding.Unicode.GetBytes(message);
        foreach (ServerClient sc in c) {
            NetworkTransport.Send(hostId, sc.connectionId, channelId, msg, message.Length * sizeof(char), out error);
        }
    }

    private void SymfonyConnect(int cnnId, string playerName, string playerPasswd) {
        //Conectar con symphony
        string LoginUrl = "http://192.168.6.7:8000/ws/login";
        WWWForm loginArray = new WWWForm();
        loginArray.AddField("user", playerName);
        loginArray.AddField("passwd", playerPasswd);
        WWW www = new WWW(LoginUrl, loginArray);
        StartCoroutine(WaitForWWW(www, cnnId, playerName));

        //Enviar a los demas clientes el jugador conectado
        //ToLog("Nuevo jugador" + playerName + "Se ha unido a la partida");
    }

    private IEnumerator WaitForWWW(WWW www, int cnnId, string playerName) {
        yield return www;

        if (string.IsNullOrEmpty(www.error)) {

            JSONObject f = new JSONObject(www.text);
            if (f.ToString().Equals("0")) {
                Send("CNN|" + playerName + '|' + cnnId + '|' + -1, reliableChannel, clients);
            } else {
                ToLog(f.ToString());
                sendUserCreationMessages(cnnId, playerName, f["id"].ToString());
            }
            primerJugadoCreado = true;
        } else {
            ToLog(www.error);
        }
    }
    private void inventario(int idUsuario, int cnnId, string playerName) {

        string url = "http://192.168.6.7:8000/ws/inventario";
        WWWForm usuario = new WWWForm();
        usuario.AddField("idUsuario", idUsuario);
        WWW www = new WWW(url, usuario);

        StartCoroutine(ConexionInventario(www, cnnId, playerName));
    }
    private IEnumerator ConexionInventario(WWW www, int cnnId, string playerName) {

        yield return www;

        if (string.IsNullOrEmpty(www.error)) {
            JSONObject f = new JSONObject(www.text);
            ToLog(f.ToString());
            String nombres = "", rutas = "";
            bool datos = true;
            int contador = 0;
            while (datos) {

                try {
                    nombres = nombres + f[contador]["nombre"] + "_";
                    ToLog(nombres);
                    rutas = rutas + f[contador]["imagen"].ToString().Split('/')[1] + "-";
                    ToLog(rutas);
                    contador++;
                    if (f[contador].Equals("")) {
                        datos = false;
                    }
                } catch (Exception e) {
                    datos = false;
                }
            }

            if (f.ToString().Equals("[]")) {
                Send("INV|" + playerName + '|' + cnnId + '|' + -1, reliableChannel, clients);
            } else {
                Send("INV|" + playerName + '|' + cnnId + '|' + nombres + "|" + rutas + "|" + contador, reliableChannel, clients);
            }
        } else {
            ToLog(www.error);
        }
    }
    private void ToLog(string msg) {
        if (logCounter >= 18) {
            LoggingText.GetComponent<Text>().text = msg;
            logCounter = 1;
        } else {
            LoggingText.GetComponent<Text>().text = LoggingText.GetComponent<Text>().text + "\n" + msg;
            logCounter++;
        }
    }
}
public static class ServerMock {

    public static bool CheckLoginDetailsMock(string user) {
        if (user == "offline") {
            return true;
        }
        return false;
    }
    public static bool CheckHatsMock(string user) {
        if (user == "offline") {
            return true;
        }
        return false;
    }

}
