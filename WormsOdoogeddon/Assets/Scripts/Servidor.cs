using System.Collections;
using System.Collections.Generic;
using System.Text;
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

    private List<ServerClient> clients = new List<ServerClient>();


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
        //Conectar con symphon
      
    

        //Enviar a los demas clientes el jugador conectado
        //Debug.Log("Nuevo jugador" + playerName + "Se ha unido a la partida");
        //Send("CNN|" + playerName + '|' + cnnId,reliableChannel,clients);

    }

}
