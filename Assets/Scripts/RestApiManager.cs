using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class RestApiManager : MonoBehaviour
{
    [SerializeField] private List<RawImage> YourRawImage;
    [SerializeField] private int UserId = 1;
    [SerializeField] private string ServerApiPath = "https://my-json-server.typicode.com/jaydeeay/JSONSV";
    [SerializeField] private string RickAndoMortyApi = "https://rickandmortyapi.com/api";
    public int[] cards;

    public void GetCharacterClick() 
    {
        StartCoroutine(GetCharacter(1,1));
    }
    public void ChangeNumber(int number)
    {
        UserId = number;
    }
    public void GetPlayerInfoClick() 
    {
        StartCoroutine(GetPlayerInfo());
    }   


    IEnumerator GetPlayerInfo()
    {
        string url = ServerApiPath + "/users/" + UserId;
        Debug.Log(url);
        UnityWebRequest www = UnityWebRequest.Get(url);
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
                UserData user = JsonUtility.FromJson<UserData>(www.downloadHandler.text);

                Debug.Log("Name:" + user.name);

                for (int i = 0; i < user.deck.Length; i++)
                {
                    StartCoroutine(GetCharacter(user.deck[i],i));
                    yield return new WaitForSeconds(0.1f);
                }
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
    IEnumerator GetCharacter(int Id, int Place)
    {
        UnityWebRequest www = UnityWebRequest.Get(RickAndoMortyApi + "/character/" + Id);
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
                Character character = JsonUtility.FromJson<Character>(www.downloadHandler.text);
                Debug.Log(character.id);
                StartCoroutine(DownloadImage(character.image,Place));
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

    IEnumerator DownloadImage(string url, int Place)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();
        
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else YourRawImage[Place].texture = ((DownloadHandlerTexture)request.downloadHandler).texture;                
    }
}

[System.Serializable]
public class CharacterList
{
    public CharacterListInfo info;
    public List<Character> results;
}

[System.Serializable]
public class CharacterListInfo
{
    public int count;
    public int pages;
    public string prev;
    public string next;
}

[System.Serializable]
public class Character
{
    public int id;
    public string name;
    public string specie;
    public string image;
}
public class CharacterImage
{
    public string image;
}

public class UserData
{
    public int id;
    public string name;
    public int[] deck;
}
