﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;


//Clase para los clientes
public class ServerClient
{
    public int connectionId;
    public string playerName;
    public int id;
}

public class Servidor : MonoBehaviour
{

    private const int MAX_CONNECTION = 100;
    private int port = 5701;

    private int hostId;
    private int webHostId;

    private int reliableChannel;
    private int unReliableChannel;

    private bool isStarted = false;
    private byte error;
    private ServerClient jugador1, jugador2;

    private List<ServerClient> clients = new List<ServerClient>();

    bool primerJugadoCreado = false;
    int cantidadJugadores = 0;
    GameObject posJ1;

    private void Start()
    {

     
        NetworkTransport.Init();
        ConnectionConfig cc = new ConnectionConfig();

        reliableChannel = cc.AddChannel(QosType.Reliable);
        unReliableChannel = cc.AddChannel(QosType.Unreliable);

        HostTopology topo = new HostTopology(cc, MAX_CONNECTION);
        hostId = NetworkTransport.AddHost(topo, port, null);
        webHostId = NetworkTransport.AddWebsocketHost(topo, port, null);

        isStarted = true;

        posJ1 = new GameObject();
        posJ1.transform.position = new Vector3(0, 0, -300);

        //Debug.Log("Arranacado");

    }

    private void Update()
    {

        if (!isStarted)
        {
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
        switch (recData)
        {
            case NetworkEventType.Nothing:
                break;

            case NetworkEventType.ConnectEvent:
                //Debug.Log("Player" + connectionId + "Se ha conectado");
                OnConnection(connectionId);
                break;

            case NetworkEventType.DataEvent:
                string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                Debug.Log("QUE RECIBO DE CADA CONEXION" + connectionId + ": " + msg);
                string[] splitData = msg.Split('|');
                switch (splitData[0])
                {
                    case "NAMEIS":
                        SymfonyConnect(connectionId, splitData[1],splitData[2]);
                        
                        break;

                    case "CNN":
                        break;

                    case "DC":
                        break;

                    case "EMPEZAR":
                        //Debug.Log("EMPEZAR" + msg);
                        Send("EMPEZAR|", reliableChannel, clients);
                        break;

                    case "INV":
                        inventario(int.Parse(splitData[1]),connectionId,splitData[2]);
                        
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

                    default:
                        Debug.Log("Mensaje Invalido" + msg);
                        break;

                }
                break;

            case NetworkEventType.DisconnectEvent:
                //Debug.Log("Player" + connectionId + "Se ha desconectado");
                break;
        }

    }



    private void OnConnection(int cnnId)
    {
        //Añadir a la lista
        ServerClient c = new ServerClient();
        c.connectionId = cnnId;
        c.playerName = "TEMP";
        clients.Add(c);

        //Despues de añadir el cliente al servidor
        //mandamos un cliente a los clientes
        string msg = "ASKNAME|" + cnnId + "|";

        foreach (ServerClient sc in clients)
            msg += sc.playerName + "%" + sc.connectionId + '|';

        msg = msg.Trim('|');
        //ejemplo de linea de envio --> ASKNAME|1|ANDER%1|
        //Debug.Log("enviado a clientes"+msg);
        Send(msg, reliableChannel, cnnId);

    }

    private void Send(string message, int channelId, int cnnId)
    {
        List<ServerClient> c = new List<ServerClient>();
        c.Add(clients.Find(x => x.connectionId == cnnId));
        //Debug.Log("-----------------------enviado---------------------------");
        Send(message, channelId, c);
    }

    private void Send(string message, int channelId, List<ServerClient> c)
    {
        //Debug.Log("Sending: " + message);
        byte[] msg = Encoding.Unicode.GetBytes(message);
        foreach (ServerClient sc in c)
        {
            NetworkTransport.Send(hostId, sc.connectionId, channelId, msg, message.Length * sizeof(char), out error);
            //Debug.Log("-enviado2-"+message);
        }
    }

    private void SymfonyConnect(int cnnId, string playerName, string playerPasswd)
    {
        //Conectar con symphony
        string LoginUrl = "http://192.168.6.7:8000/ws/login";
        WWWForm loginArray = new WWWForm();
        loginArray.AddField("user", playerName);
        loginArray.AddField("passwd", playerPasswd);
        WWW www = new WWW(LoginUrl,loginArray);
        StartCoroutine(WaitForWWW(www, cnnId, playerName));
        
        //Enviar a los demas clientes el jugador conectado
        //Debug.Log("Nuevo jugador" + playerName + "Se ha unido a la partida");
    }

    private IEnumerator WaitForWWW(WWW www, int cnnId, string playerName)
    {
        yield return www;
       
        if (string.IsNullOrEmpty(www.error)) {
            
            JSONObject f = new JSONObject(www.text);
            Debug.Log(f.ToString());
            if (f.ToString().Equals("[]")) {
                Debug.Log("no se que haces aqui");
                
                //Send("CNN|" + playerName + '|' + cnnId + '|' + -1, reliableChannel, clients);
            }else{
                Debug.Log("se que haces aqui");
                if (!primerJugadoCreado) {
                    jugador1 = new ServerClient();
                    jugador1.id = int.Parse(f[0]["id"].ToString());
                    jugador1.playerName = playerName;
                    jugador1.connectionId = cnnId;
                    Send("CNN|" + playerName + '|' + cnnId + '|' + f[0]["id"].ToString(), reliableChannel, clients);
                } else {
                    Send("CNN|" + jugador1.playerName + '|' + jugador1.connectionId + '|' + f[0]["id"].ToString(), reliableChannel, clients);
                    Send("CNN|" + playerName + '|' + cnnId + '|' + f[0]["id"].ToString(), reliableChannel, clients);
                }
            }
            //Debug.Log(f[0]["id"].ToString());
            primerJugadoCreado = true;
        }
        else
        {          
            Debug.Log(www.error);
        }
    }
    private void inventario(int idUsuario, int cnnId, string playerName) {
        //Conectar con symphony
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
            Debug.Log(f.ToString());
            String nombres="", rutas="";
            bool datos = true;
            int contador=0;
            while (datos) {
               
                try {
                    nombres = nombres + f[contador]["nombre"] + ".";
                    rutas = rutas + f[contador]["imagen"].ToString().Split('/')[1] + ".";
                    contador++;
                    if (f[contador].Equals("")) {
                        datos = false;
                    }
                } catch(Exception e) {
                    datos = false;
                }
            }
            Debug.Log(nombres + "|" + contador + "|" + rutas);
            if (f.ToString().Equals("[]")) {
                Send("INV|" + playerName + '|' + cnnId + '|' + -1, reliableChannel, clients);
            } else {
                Send("INV|" + playerName + '|' + cnnId + '|' + nombres + "|" + rutas + "|" + contador, reliableChannel, clients);
            }
            //Debug.Log(f[0]["id"].ToString());
        } else {
            Debug.Log(www.error);
        }
    }
}
