using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using System;

public class Player {
    public string playerName;
    public int posJugador;
    public GameObject avatar;
    public int connectId;
    public int idUsuario;
}


public class Cliente : MonoBehaviour {
    private SimpleAES simpleAES;

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

    private int ourClientId;

    public List<Player> jugadores = new List<Player>();
    public GameObject nombre, password, errortxt, noObjeto, gorroPrefab, passwordAju, rePasswordAju;
    private string user, passwd;
    public GameObject canvas1, canvas2, canvas3, canvas4, panelChangePassword, errorPassAju;
    public Text usuario, ajustesName, ajustesLastName, ajustesEmail;
    public GameObject prefabGusano, posJ1, posJ2, prefabBala;
    private GameObject bala;
    private bool juego = false, jugadoresCreados = false, balaCreada = false, miTurno = false;
    public Toggle toogleAjustes;
    public InputField passwordAjuInput;

    public void Awake() {
        DontDestroyOnLoad(this.gameObject);
    }

    public void Connect() {

        errortxt = GameObject.Find("Login").transform.Find("Panel").transform.Find("errorTxt").gameObject;
        user = nombre.GetComponent<InputField>().text;
        passwd = password.GetComponent<InputField>().text;

        if (user.Trim().Equals("") || passwd.Trim().Equals("")) {
            errortxt.SetActive(true);
            errortxt.transform.GetComponent<Text>().text = "DEBES RELLENAR LOS CAMPOS";
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
        simpleAES = new SimpleAES();


    }

    private void Update() {
        if (!isConnected) {
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
            /*case NetworkEventType.Nothing:
                break;

            case NetworkEventType.ConnectEvent:
                break;*/

            case NetworkEventType.DataEvent:
                //byte[] msgToDecrypt = new byte[dataSize];
                //Array.Copy(recBuffer, 0, msgToDecrypt, 0, dataSize);
                //string msgd = simpleAES.Decrypt(msgToDecrypt);

                string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                Debug.Log(msg + " Recibido del Server");
                //Debug.Log("receiving: " + msg + "\n from encryption " + msgd);
                string[] splitData = msg.Split('|');
                switch (splitData[0]) {
                    case "ASKNAME":
                        OnAskName(splitData);
                        break;

                    case "CNN":
                        int id=-1;
                        string nameToLog="";
                        //string[] splitSplitData = splitData[1].Split('%');
                        for (int i = 1; i < splitData.Length; i++) {
                            string[] splitSplitData = splitData[i].Split('%');
                            if (splitSplitData[0].Equals(user)) {
                                rellenarCamposJugadorLocal(splitData[i].Split('%'));
                                id = int.Parse(splitSplitData[2]);
                                nameToLog = splitSplitData[0];
                            }
                            else {
                                rellenarCamposJugadorRival(splitData[i].Split('%'));
                            }
                        }
                        Loggeado(id, nameToLog);
                        break;

                    case "DC":
                        break;
                    case "INV":
                        if (splitData[3] != "-1") {
                            inventarioCargar(splitData[1], int.Parse(splitData[2]), splitData[3], splitData[4], int.Parse(splitData[5]));
                        }
                        else {
                            noObjeto.SetActive(true);
                            noObjeto.transform.GetComponent<Text>().text = "No tiene ningun objeto comprado acceda a la tienda para comprar.";
                        }
                        break;
                    case "AJU":
                        Debug.Log(splitData[3].Replace('"', ' ').Trim());
                        string name, lastName, mail;

                        name = splitData[3].Replace('"', ' ').Trim();
                        lastName = splitData[4].Replace('"', ' ').Trim();
                        mail = splitData[5].Replace('"', ' ').Trim();

                        ajustesName.text = name;
                        ajustesLastName.text = lastName;
                        ajustesEmail.text = mail;
                        break;

                    case "AJUPASS":
                        Debug.Log("ESTOY EN CASE AJUPASS" + splitData[2]);
                        if (splitData[2].Equals("1")) {
                            canvas4.SetActive(false);
                            canvas2.SetActive(true);
                        }
                        else {
                            errorPassAju.GetComponent<Text>().text = "ERROR";
                            errorPassAju.SetActive(true);
                        }
                        break;
                    case "EMPEZAR":

                        break;
                    case "POS1":
                        posJ1.transform.position = new Vector3(float.Parse(splitData[1]), float.Parse(splitData[2]), 0);
                        break;
                    case "POS2":
                        posJ2.transform.position = new Vector3(float.Parse(splitData[1]), float.Parse(splitData[2]), 0);
                        break;
                    case "SPAWN":
                        juego = true;
                        Debug.Log(jugadorLocal.connectId + "|" + jugadorRival.connectId);
                        if (jugadorLocal.connectId < jugadorRival.connectId) {
                            miTurno = true;
                            GameObject.Find("Juego").GetComponent<Juego>().miTurno = true;
                        }
                        if (!miTurno) {
                            bloquearFunciones();
                        }
                        break;
                    case "DER":
                        if (jugadorLocal.playerName.Equals(splitData[1])) {
                            jugadorLocal.avatar.GetComponent<MovimientoGusano>().moverDerecha();
                        }
                        else {
                            jugadorRival.avatar.GetComponent<MovimientoGusano>().moverDerecha();
                        }
                        break;
                    case "IZQ":
                        if (jugadorLocal.playerName.Equals(splitData[1])) {
                            jugadorLocal.avatar.GetComponent<MovimientoGusano>().moverIzquierda();
                        }
                        else {
                            jugadorRival.avatar.GetComponent<MovimientoGusano>().moverIzquierda();
                        }
                        break;
                    case "PAR":
                        if (jugadorLocal.playerName.Equals(splitData[1])) {
                            jugadorLocal.avatar.GetComponent<MovimientoGusano>().pararDeMover();
                        }
                        else {
                            jugadorRival.avatar.GetComponent<MovimientoGusano>().pararDeMover();
                        }
                        break;
                    case "SAL":
                        if (jugadorLocal.playerName.Equals(splitData[1])) {
                            jugadorLocal.avatar.GetComponent<MovimientoGusano>().saltar();
                        }
                        else {
                            jugadorRival.avatar.GetComponent<MovimientoGusano>().saltar();
                        }
                        Send("PAR|" + splitData[1], reliableChannel);
                        break;
                    case "ARR":
                        if (jugadorLocal.playerName.Equals(splitData[1])) {
                            jugadorLocal.avatar.GetComponent<MovimientoGusano>().apuntarArriba();
                        }
                        else {
                            jugadorRival.avatar.GetComponent<MovimientoGusano>().apuntarArriba();
                        }
                        break;
                    case "ABA":
                        if (jugadorLocal.playerName.Equals(splitData[1])) {
                            jugadorLocal.avatar.GetComponent<MovimientoGusano>().apuntarAbajo();
                        }
                        else {
                            jugadorRival.avatar.GetComponent<MovimientoGusano>().apuntarAbajo();
                        }
                        break;
                    case "DIS":
                        Vector3 direccion;
                        if (jugadorLocal.playerName.Equals(splitData[1])) {
                            if (!balaCreada) {
                                bala = Instantiate(prefabBala, jugadorLocal.avatar.transform.position, Quaternion.identity);
                            }
                            else {
                                bala.transform.position = jugadorLocal.avatar.transform.position;
                            }
                            direccion = jugadorLocal.avatar.GetComponent<MovimientoGusano>().getDireccionDisparo();
                            if (direccion.x > 0) {
                                bala.transform.position = new Vector3(bala.transform.position.x + 1, bala.transform.position.y, 0);
                            }
                            else {
                                bala.transform.position = new Vector3(bala.transform.position.x - 1, bala.transform.position.y, 0);
                            }
                            bala.GetComponent<Rigidbody2D>().AddForce(direccion * 100);
                        }
                        else {
                            if (!balaCreada) {
                                bala = Instantiate(prefabBala, jugadorRival.avatar.transform.position, Quaternion.identity);
                            }
                            else {
                                bala.transform.position = jugadorRival.avatar.transform.position;
                            }
                            direccion = jugadorRival.avatar.GetComponent<MovimientoGusano>().getDireccionDisparo();
                            if (direccion.x > 0) {
                                bala.transform.position = new Vector3(bala.transform.position.x + 1, bala.transform.position.y, 0);
                            }
                            else {
                                bala.transform.position = new Vector3(bala.transform.position.x - 1, bala.transform.position.y, 0);
                            }
                            bala.GetComponent<Rigidbody2D>().AddForce(direccion * 100);
                        }
                        cambiarTurno();
                        break;
                    case "GOL":
                        if (jugadorLocal.playerName.Equals(splitData[1])) {
                            jugadorRival.avatar.transform.position = new Vector3(0, 3, 0);
                            jugadorRival.avatar.GetComponent<MovimientoGusano>().vida = jugadorRival.avatar.GetComponent<MovimientoGusano>().vida - 1;
                            if (jugadorRival.avatar.GetComponent<MovimientoGusano>().vida == 0) {
                                GameObject.Find("Juego").GetComponent<Juego>().jugadorPierde(jugadorRival.playerName);
                            }
                            else {
                                GameObject.Find("Juego").GetComponent<Juego>().cambiarVidas(jugadorRival.avatar.GetComponent<MovimientoGusano>().vida);
                            }
                        }
                        else {
                            jugadorLocal.avatar.transform.position = new Vector3(0, 3, 0);
                        }
                        break;

                    default:
                        Debug.Log("Mensaje Invalido" + msg);
                        break;

                }
                break;

                /* case NetworkEventType.DisconnectEvent:
                     break;*/
        }
        if (juego) {
            if (!jugadoresCreados) {
                SpawnPlayer();
            }
            if (jugadorRival.avatar.GetComponent<MovimientoGusano>().golpeado == true) {
                Send("GOL|" + jugadorLocal.playerName, reliableChannel);
                jugadorRival.avatar.GetComponent<MovimientoGusano>().golpeado = false;
            }
        }

    }
    private void OnAskName(string[] data) {
        //Id del player
        ourClientId = int.Parse(data[1]);
        //Enviar el nombre al servidor
        Send("NAMEIS|" + user + "|" + passwd, reliableChannel);

    }

    private void Send(string message, int channelId) {
        string msgBar = message + "|º";
        //string sMsg = simpleAES.Encrypt(message);
        byte[] msg = simpleAES.Encrypt(msgBar);
        simpleAES = new SimpleAES();
        Debug.Log("El byte Array que envia el cliente es de: " + msg.Length);

        NetworkTransport.Send(hostId, connectionId, channelId, msg, message.Length * sizeof(char), out error);
        

    }

    private void Loggeado(int id, string player) {
        if (id.Equals(-1)) {
            errortxt.transform.GetComponent<Text>().text = "EL USUARIO O LA CONTRASEÑA NO SON CORRECTOS";
            errortxt.SetActive(true);
            // GameObject.Find("PopUp").Equals(EditorUtility.DisplayDialog("El usuario o la Contraseña no son correctos", "", "OK", ""));
            nombre.GetComponent<InputField>().text = "";
            password.GetComponent<InputField>().text = "";
        }
        else {
            canvas1.SetActive(false);
            canvas2.SetActive(true);
            usuario.text = jugadorLocal.playerName;
        }


    }

    public void setpos(GameObject posiJ1, GameObject posiJ2) {
        Send("POS1|" + posiJ1.transform.position.x + "|" + posiJ1.transform.position.y, reliableChannel);
        Send("POS2|" + posiJ2.transform.position.x + "|" + posiJ2.transform.position.y, reliableChannel);
        this.posJ1 = posiJ1;
        this.posJ2 = posiJ2;
    }
    public void setposJ2(GameObject posiJ2) {
        Send("POS2|" + posiJ2.transform.position.x + "|" + posiJ2.transform.position.y, reliableChannel);
    }
    public GameObject getposJ1() {
        return posJ1;
    }
    public GameObject getposJ2() {
        return posJ2;
    }

    public void SpawnPlayer() {
        if (jugadorLocal.connectId % 2 != 0) {
            jugadorLocal.avatar = Instantiate(prefabGusano, posJ1.transform.position, Quaternion.identity);
            jugadorRival.avatar = Instantiate(prefabGusano, posJ2.transform.position, Quaternion.identity);
        }
        else {
            jugadorLocal.avatar = Instantiate(prefabGusano, posJ2.transform.position, Quaternion.identity);
            jugadorRival.avatar = Instantiate(prefabGusano, posJ1.transform.position, Quaternion.identity);
        }
        jugadorRival.avatar.transform.position = new Vector3(jugadorRival.avatar.transform.position.x, jugadorRival.avatar.transform.position.y, 0);
        jugadorLocal.avatar.transform.position = new Vector3(jugadorLocal.avatar.transform.position.x, jugadorLocal.avatar.transform.position.y, 0);
        jugadoresCreados = true;
        reposicionarJugadores();
        Debug.Log("CREAR JUGADORES");
    }

    private void reposicionarJugadores() {
        if (jugadorLocal.connectId % 2 != 0) {
            jugadorLocal.avatar.transform.position = posJ1.transform.position;
            jugadorRival.avatar.transform.position = posJ2.transform.position;
        }
        else {
            jugadorLocal.avatar.transform.position = posJ2.transform.position;
            jugadorRival.avatar.transform.position = posJ1.transform.position;
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
    public int getClienteId() {

        return connectionId;
    }
    private void rellenarCamposJugadorLocal(string[] splitData) {
        jugadorLocal = new Player();
        jugadorLocal.idUsuario = int.Parse(splitData[2]);
        jugadorLocal.playerName = splitData[0];
        jugadorLocal.connectId = int.Parse(splitData[1]);
    }
    private void rellenarCamposJugadorRival(string[] splitData) {
        jugadorRival = new Player();
        jugadorRival.idUsuario = int.Parse(splitData[2]);
        jugadorRival.playerName = splitData[0];
        jugadorRival.connectId = int.Parse(splitData[1]);
    }
    public void inventarioMenu() {
        Send("INV|" + jugadorLocal.idUsuario + "|" + jugadorLocal.playerName, reliableChannel);
        canvas2.SetActive(false);
        canvas3.SetActive(true);
    }
    public void AjustesMenu() {
        Send("AJU|" + jugadorLocal.idUsuario + "|" + jugadorLocal.playerName, reliableChannel);
        canvas2.SetActive(false);
        canvas4.SetActive(true);
    }
    public void jugar() {
        SceneManager.LoadScene("Juego");
    }
    public void inventarioTienda() {
        //He intentado que abra la tienda ya loggeado pero resulta imposible hacer openUrl + post. O uno u otro pero los 2 juntos no se pueden.
        Application.OpenURL("http://192.168.6.7:8000/login");
    }
    public void juegoEmpezado() {
    }
    public void mover(string direccion) {
        if (direccion.Equals("derecha")) {
            Send("DER|" + jugadorLocal.playerName, reliableChannel);
        }
        else {
            Send("IZQ|" + jugadorLocal.playerName, reliableChannel);
        }
    }
    public void saltar() {
        Send("SAL|" + jugadorLocal.playerName, reliableChannel);
    }
    public void parar() {
        Send("PAR|" + jugadorLocal.playerName, reliableChannel);
    }
    public void volver() {
        canvas2.SetActive(true);
        canvas3.SetActive(false);
        canvas4.SetActive(false);
    }


    public void inventarioCargar(string playername, int cnnid, string nombres, string rutas, int contador) {
        string[] nombreGorro = nombres.Split('_');
        Debug.Log(nombreGorro);
        string[] rutaGorro = rutas.Split('-');
        Debug.Log(rutaGorro);
        Transform inventario = GameObject.Find("Inventario").transform.Find("Panel");
        if (inventario == null) {
            Debug.Log("INVENTARIO ES NULL");
        }
        for (int i = 0; i < contador; i++) {
            Instantiate(gorroPrefab, new Vector2(0, 2 - (i * 2)), Quaternion.identity, inventario);
            gorroPrefab.SetActive(true);
            gorroPrefab.transform.GetComponentInChildren<Text>().text = nombreGorro[i].ToString();
            Transform imagenGorro = gorroPrefab.transform.Find("Image");
            Debug.Log(imagenGorro.ToString());
            if (imagenGorro == null) {
                Debug.Log("GORRO ES NULL");
            }
            Sprite spriteR = Resources.Load<Sprite>("Sprites/gorro2"/*+rutas[i].ToString()*/);
            imagenGorro.GetComponent<SpriteRenderer>().sprite = spriteR;
        }

    }

    public void salir() {
        Application.Quit();
    }

    public void disparar() {
        Send("DIS|" + jugadorLocal.playerName, reliableChannel);
    }

    public void apuntar(string direccion) {

        if (direccion.Equals("arriba")) {
            Send("ARR|" + jugadorLocal.playerName, reliableChannel);
        }
        else if (direccion.Equals("abajo")) {
            Send("ABA|" + jugadorLocal.playerName, reliableChannel);
        }
    }
    private void bloquearFunciones() {
        GameObject.Find("Juego").GetComponent<Juego>().miTurno = false;
    }
    private void cambiarTurno() {
        if (miTurno) {
            miTurno = false;
            bloquearFunciones();
        }
        else {
            miTurno = true;
            GameObject.Find("Juego").GetComponent<Juego>().miTurno = true;
        }
    }
    public void toggleAjustes(bool flag) {

        panelChangePassword.SetActive(toogleAjustes.isOn);

    }
    public void cambiarContra() {

        if (!passwordAju.GetComponent<Text>().text.Equals(rePasswordAju.GetComponent<Text>().text)) {
            errorPassAju.SetActive(true);
            return;
        }

        Send("AJUPASS|" + jugadorLocal.idUsuario + "|" + jugadorLocal.playerName + "|" + passwordAjuInput.text, reliableChannel);
    }
}
