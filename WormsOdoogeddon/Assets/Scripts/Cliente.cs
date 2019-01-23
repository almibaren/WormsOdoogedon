using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;


public class Player
{
    public string playerName;
    public int posJugador;
    public GameObject avatar;
    public int connectId;
    public int idUsuario;
}


public class Cliente : MonoBehaviour
{

    private const int MAX_CONNECTION = 100;
    private int port = 5701;

    private int hostId;
    private int webHostId;

    private int reliableChannel;
    private int unReliableChannel;
    private Player jugadorLocal, jugadorRival;

    private float connectionTime;
    private int connectionId;
    private bool isConnected;
    private bool isStarted = false;

    private byte error;

    //el nombre del usuario
    //public string playerName;
    private int ourClientId;

    public List<Player> jugadores = new List<Player>();
    public GameObject nombre, password,popup;
    private string user,passwd;
    public GameObject canvas1, canvas2;
    public Text usuario;

    public void Connect()
    {
        
        popup = GameObject.Find("Login").transform.Find("Panel").transform.Find("errorTxt").gameObject;
        user = nombre.GetComponent<InputField>().text;
        passwd = password.GetComponent<InputField>().text;

        if (user.Trim().Equals("") || passwd.Trim().Equals("")) {
            popup.SetActive(true);
            popup.transform.GetComponent<Text>().text="DEBES RELLENAR LOS CAMPOS";
            return;
        }

        NetworkTransport.Init();
        ConnectionConfig cc = new ConnectionConfig();

        reliableChannel = cc.AddChannel(QosType.Reliable);
        unReliableChannel = cc.AddChannel(QosType.Unreliable);

        HostTopology topo = new HostTopology(cc, MAX_CONNECTION);

        hostId = NetworkTransport.AddHost(topo, 0);
        connectionId = NetworkTransport.Connect(hostId, "127.0.0.1", port, 0, out error);

        connectionTime = Time.time;
        isConnected = true;

    }

    private void Update()
    {
        if (!isConnected)
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
            /*case NetworkEventType.Nothing:
                break;

            case NetworkEventType.ConnectEvent:
                break;*/

            case NetworkEventType.DataEvent:
                string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                //Debug.Log("receiving: " + msg);
                string[] splitData = msg.Split('|');
                Debug.Log("dato del case " + splitData[0] + " segundo valor " + splitData[1]);
                switch (splitData[0])
                {
                    case "ASKNAME":
                        OnAskName(splitData);
                        break;

                    case "CNN":
                        Debug.Log("dato del cnn " + splitData[1] + " segundo valor " + splitData[2] + " tercer valor " + splitData[3]);
                        if (splitData[1].Equals(user)){
                            rellenarCamposJugadorLocal(splitData);
                            Debug.Log(jugadorLocal.idUsuario +"aaa");
                        } else {
                            rellenarCamposJugadorRival(splitData);
                        }
                        Loggeado(int.Parse(splitData[3]), splitData[1]);
                        break;

                    case "DC":
                        break;

                    case "EMPEZAR":
                       
                        break;

                    default:
                        Debug.Log("Mensaje Invalido" + msg);
                        break;

                }
                break;

                /* case NetworkEventType.DisconnectEvent:
                     break;*/
        }


    }

    private void OnAskName(string[] data)
    {
        //Id del player
        ourClientId = int.Parse(data[1]);

        //Enviar el nombre al servidor
   
        Send("NAMEIS|" + user +"|" + passwd, reliableChannel);

        //enviar datos al resto de jugadores
        for (int i = 2; i < data.Length - 1; i++)
        {
            string[] d = data[i].Split('%');
          /*  SpawnPlayer(d[0], int.Parse(d[1]));*/
        }

    }

    private void Send(string message, int channelId)
    {
        //Debug.Log("Sending: " + message);
        byte[] msg = Encoding.Unicode.GetBytes(message);
        NetworkTransport.Send(hostId, connectionId, channelId, msg, message.Length * sizeof(char), out error);

    }

    private void Loggeado(int id, string player) {
        if (id == -1)
        {
            popup.transform.GetComponent<Text>().text = "EL USUARIO O LA CONTRASEÑA NO SON CORRECTOS";
            popup.SetActive(true);
           // GameObject.Find("PopUp").Equals(EditorUtility.DisplayDialog("El usuario o la Contraseña no son correctos", "", "OK", ""));
            nombre.GetComponent<InputField>().text = "";
            password.GetComponent<InputField>().text = "";
        }
        else {
            canvas1.SetActive(false);
            canvas2.SetActive(true);           
            usuario.text = player.ToString();
        }


    }

   /* 
    private void SpawnPlayer(string playerName, int cnnId)
    {
        if (cnnId == ourClientId)
        {
            canvas1.SetActive(false);
            canvas2.SetActive(true);
        }


        Player p = new Player();
        if (cnnId % 2 != 0)
        {
            p.avatar = Instantiate(playerPrefab, jugador1.position, Quaternion.identity);//con esto creo un jugador
        }
        else
        {
            p.avatar = Instantiate(playerPrefab, jugador2.position, Quaternion.identity);//con esto creo un jugador
        }


        p.playerName = playerName;
        p.connectId = cnnId;
        jugadores.Add(p);
        
    }*/
    public void Registrar() {
        Application.OpenURL("http://192.168.6.7:8000/registrar");        
    }
    public int getClienteId()
    {

        return connectionId;
    }
    private void rellenarCamposJugadorLocal( string[] splitData) {
        jugadorLocal = new Player();
        jugadorLocal.idUsuario = int.Parse(splitData[3]);
        jugadorLocal.playerName = splitData[1];
        jugadorLocal.connectId = int.Parse(splitData[2]);
    }
    private void rellenarCamposJugadorRival(string[] splitData) {
        jugadorRival = new Player();
        jugadorRival.idUsuario = int.Parse(splitData[3]);
        jugadorRival.playerName = splitData[1];
        jugadorRival.connectId = int.Parse(splitData[2]);
    }
    public void inventario() {
        Send("INV|" + jugadorLocal.idUsuario + "|" + jugadorLocal.playerName,reliableChannel);
    }
}
