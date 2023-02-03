using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HttpAuthHandler : MonoBehaviour
{
    [SerializeField]
    private string ServerApiUrl { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Registrar()
    {
        User user = new User();

        user.username = GameObject.Find("InputFieldUsername").GetComponent<InputField>().text;
        user.password = GameObject.Find("InputFieldPassword").GetComponent<InputField>().text;

        string PostData = JsonUtility.ToJson(user);
        StartCoroutine(Registro(PostData));
    }

    IEnumerator Registro(string postData)
    {
        UnityWebRequest www = UnityWebRequest.Put(ServerApiUrl + "/api/usuarios", postData);

        www.method = "POST";
        www.SetRequestHeader("Content Type", "application/json");

        yield return www.Send();

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

            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
        }
    }
}

[System.Serializable]
public class User
{
    public string _id;
    public string username;
    public string password;
    public int score;
}

public class AuthJsonData
{
    public User usuario;
    public string token;
}