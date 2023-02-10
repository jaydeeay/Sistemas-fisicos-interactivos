using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HttpAuthHandler : MonoBehaviour
{
    [SerializeField]
    private string ServerApiUrl;

    public string Token    { get; set; }
    public string Username    { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        Token = PlayerPrefs.GetString("token");
        Username = PlayerPrefs.GetString("username");

        if (string.IsNullOrEmpty(Token))
        {
            Debug.Log("No hay token");
            //Ir a login
            return;
        }
        else
        {
            Debug.Log(Token);
            Debug.Log(Username);

            //Verificar token

            StartCoroutine(GetPerfil());
        }
    }

    public void Registrar()
    {
        User user = new User();

        user.username = GameObject.Find("InputUsername").GetComponent<InputField>().text;
        user.password = GameObject.Find("InputPassword").GetComponent<InputField>().text;

        string PostData = JsonUtility.ToJson(user);
        StartCoroutine(Registro(PostData));
    }    
    public void Ingresar()
    {
        User user = new User();

        user.username = GameObject.Find("InputUsername").GetComponent<InputField>().text;
        user.password = GameObject.Find("InputPassword").GetComponent<InputField>().text;

        string postData = JsonUtility.ToJson(user);
        StartCoroutine(Login(postData));
    }

    IEnumerator Registro(string postData)
    {
        UnityWebRequest www = UnityWebRequest.Put(ServerApiUrl + "/api/usuarios", postData);

        www.method = "POST";
        www.SetRequestHeader("Content Type", "application/json");

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR:" + www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);

            if (www.responseCode == 200)
            {
                AuthJsonData jsonData = JsonUtility.FromJson<AuthJsonData>(www.downloadHandler.text);

                Debug.Log(jsonData.usuario.username + " se registro con id " + jsonData.usuario._id);
                //Proceso de autenticacion para obtener un toquen

            }
            else
            {
                string mensaje = "Status:" + www.responseCode;
                mensaje += "\ncontent-type:" + www.GetResponseHeader("content-type");
                mensaje += "\nError:" + www.error;
                Debug.Log(mensaje);
            }

        }
    }    

    IEnumerator Login(string postData)
    {
        UnityWebRequest www = UnityWebRequest.Put(ServerApiUrl + "/api/auth/login/", postData);

        www.method = "POST";
        www.SetRequestHeader("Content Type", "application/json");

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR:" + www.error);
        }
        else
        {
            if (www.responseCode == 200)
            {
                AuthJsonData jsonData = JsonUtility.FromJson<AuthJsonData>(www.downloadHandler.text);

                Debug.Log(jsonData.usuario.username + " inició sesión");
                Debug.Log(Token);

                Token = jsonData.token;
                Username = jsonData.usuario.username;

                PlayerPrefs.SetString("token", Token); //Guardar datos si se cierra el juego o app
                PlayerPrefs.SetString("username", Username); //Guardar datos si se cierra el juego o app
            }
            else
            {
                string mensaje = "Status:" + www.responseCode;
                mensaje += "\ncontent-type:" + www.GetResponseHeader("content-type");
                mensaje += "\nError:" + www.error;
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

                Debug.Log(jsonData.usuario.username + " Sigue con la sesion de iniciado ");
                Debug.Log(Token);

                Token = jsonData.token;
                Username = jsonData.usuario.username;

                PlayerPrefs.SetString("token", Token); //Guardar datos si se cierra el juego o app
                PlayerPrefs.SetString("username", Username); //Guardar datos si se cierra el juego o app
            }
            else
            {
                string mensaje = "Status:" + www.responseCode;
                mensaje += "\ncontent-type:" + www.GetResponseHeader("content-type");
                mensaje += "\nError:" + www.error;
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
}

public class AuthJsonData
{
    public User usuario;
    public string token;
}