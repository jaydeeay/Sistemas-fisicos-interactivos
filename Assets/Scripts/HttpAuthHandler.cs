using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class HttpAuthHandler : MonoBehaviour
{
    [SerializeField]
    private string ServerApiUrl;
    [SerializeField] private GameObject login, score;
    public string Token    { get; set; }
    public string Username    { get; set; }

    private string token;
    public TMP_Text[] ListaScore;

    // Start is called before the first frame update
    void Start()
    {
        Token = PlayerPrefs.GetString("token");
        Username = PlayerPrefs.GetString("username");

        if (string.IsNullOrEmpty(Token))
        {
            Debug.Log("No hay token");
            //Ir a login
            score.SetActive(false);
            login.SetActive(true);
            return;
        }
        else
        {
            Debug.Log(Token);
            Debug.Log(Username);

            //Verificar token
            login.SetActive(false);
            score.SetActive(true);
            token = Token;
            Debug.Log(Token);
            Debug.Log(Username);
            StartCoroutine(GetPerfil());

            StartCoroutine(GetPerfil());
        }
    }

    public void Registrar()
    {
        User user = new User();

        user.username = GameObject.Find("InputUsername").GetComponent<InputField>().text;
        user.password = GameObject.Find("InputPassword").GetComponent<InputField>().text;

        string postData = JsonUtility.ToJson(user);
        StartCoroutine(Registro(postData));
    }    
    public void Ingresar()
    {
        User user = new User();

        user.username = GameObject.Find("InputUsername").GetComponent<InputField>().text;
        user.password = GameObject.Find("InputPassword").GetComponent<InputField>().text;

        string postData = JsonUtility.ToJson(user);
        StartCoroutine(Login(postData));
    }

    public void Actualizar()
    {
        User user = new User();
        user.username = Username;
        if (int.TryParse(GameObject.Find("InputScore").GetComponent<InputField>().text, out _))
        {
            user.data.score = int.Parse(GameObject.Find("InputScore").GetComponent<InputField>().text);
        }
        string postData = JsonUtility.ToJson(user);
        StartCoroutine(updateDate(postData));
    }

    IEnumerator Registro(string postData)
    {

        UnityWebRequest www = UnityWebRequest.Put(ServerApiUrl + "/api/usuarios", postData);
        www.method = "POST";
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR :" + www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);

            if (www.responseCode == 200)
            {

                AuthJsonData jsonData = JsonUtility.FromJson<AuthJsonData>(www.downloadHandler.text);

                Debug.Log(jsonData.usuario.username + " se regitro con id " + jsonData.usuario._id);


                //Proceso de autenticacion
            }
            else
            {
                string mensaje = "Status :" + www.responseCode;
                mensaje += "\ncontent-type:" + www.GetResponseHeader("content-type");
                mensaje += "\nError :" + www.error;
                Debug.Log(mensaje);
            }

        }
    }

    IEnumerator Login(string postData)
    {

        UnityWebRequest www = UnityWebRequest.Put(ServerApiUrl + "/api/auth/login", postData);
        www.method = "POST";
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR :" + www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);

            if (www.responseCode == 200)
            {

                AuthJsonData jsonData = JsonUtility.FromJson<AuthJsonData>(www.downloadHandler.text);

                Debug.Log(jsonData.usuario.username + " inicio sesion");

                Token = jsonData.token;
                Username = jsonData.usuario.username;

                PlayerPrefs.SetString("token", Token);
                PlayerPrefs.SetString("username", Username);
                login.SetActive(false);
                score.SetActive(true);
                StartCoroutine(RequestInfo());

            }
            else
            {
                string mensaje = "Status :" + www.responseCode;
                mensaje += "\ncontent-type:" + www.GetResponseHeader("content-type");
                mensaje += "\nError :" + www.error;
                Debug.Log(mensaje);
            }

        }
    }

    IEnumerator GetPerfil()
    {
        UnityWebRequest www = UnityWebRequest.Get(ServerApiUrl + "/api/usuarios" + Username);

        www.SetRequestHeader("x-token", "application/json");

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR:" + www.error);
        }
        else
        {
            //Debug.log(www.GetResponseHeader("content-type"));

            // Show results as text
            //Debug.Log(www.downloadHandler.text);



            if (www.responseCode == 200)
            {

                AuthJsonData jsonData = JsonUtility.FromJson<AuthJsonData>(www.downloadHandler.text);

                Debug.Log(jsonData.usuario.username + " Sigue con la sesion inciada");

                //hola 
                StartCoroutine(RequestInfo());

            }
            else
            {
                score.SetActive(false);
                login.SetActive(true);
                string mensaje = "Status :" + www.responseCode;
                mensaje += "\ncontent-type:" + www.GetResponseHeader("content-type");
                mensaje += "\nError :" + www.error;
                Debug.Log(mensaje);
            }
        }
    }

    IEnumerator updateDate(string postData)
    {

        UnityWebRequest www = UnityWebRequest.Put(ServerApiUrl + "/api/usuarios/", postData);
        www.method = "PATCH";
        www.SetRequestHeader("x-token", Token);
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();
        if (www.isNetworkError)
        {
            score.SetActive(false);
            login.SetActive(true);
            Debug.Log("NETWORK ERROR :" + www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);

            if (www.responseCode == 200)
            {

                AuthJsonData jsonData = JsonUtility.FromJson<AuthJsonData>(www.downloadHandler.text);
                StartCoroutine(RequestInfo());
            }
            else
            {
                string mensaje = "Status :" + www.responseCode;
                mensaje += "\ncontent-type:" + www.GetResponseHeader("content-type");
                mensaje += "\nError :" + www.error;
            }

        }
    }

    IEnumerator RequestInfo()
    {
        UnityWebRequest www = UnityWebRequest.Get(ServerApiUrl + "/api/usuarios");
        www.SetRequestHeader("x-token", Token);
        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR :" + www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);

            if (www.responseCode == 200)
            {

                //hola 
                userlist jsonList = JsonUtility.FromJson<userlist>(www.downloadHandler.text);
                Debug.Log(jsonList.usuarios.Count);
                foreach (User a in jsonList.usuarios)
                {
                    Debug.Log(a.username);
                }
                List<User> lista = jsonList.usuarios;
                List<User> listaOrdenada = lista.OrderByDescending(u => u.data.score).ToList<User>();
                int Lugar = 0;
                foreach (User a in listaOrdenada)
                {
                    if (Lugar > 4)
                    {

                    }
                    else
                    {
                        string nombre = Lugar + 1 + "." + "Usuario:" + a.username + ",Puntaje:" + a.data.score;
                        ListaScore[Lugar].text = nombre;
                        Lugar++;
                    }

                }
            }
            else
            {
                score.SetActive(false);
                login.SetActive(true);
                string mensaje = "Status :" + www.responseCode;
                mensaje += "\ncontent-type:" + www.GetResponseHeader("content-type");
                mensaje += "\nError :" + www.error;
                Debug.Log(mensaje);
            }

        }
    }

}

[System.Serializable]
public class User
{
    public string _id;
    public string username;
    public string password;

    public userData data;

    public User()
    {
        data = new userData();
    }
    public User(string username, string password)
    {
        this.username = username;
        this.password = password;
        data = new userData();
    }
}
[System.Serializable]
public class userData
{
    public int score;
    public userData()
    {

    }

}

public class AuthJsonData
{
    public User usuario;
    public userData data;
    public string token;
}

[System.Serializable]
public class userlist
{
    public List<User> usuarios;
}
