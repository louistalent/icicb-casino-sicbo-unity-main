using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Runtime.InteropServices;
using SimpleJSON;

public class UIManager : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void GameReady(string msg);
    public TMP_InputField BetAmount;
    public TMP_InputField TotalAmount;
    public TMP_Text Alert;
    public TMP_Text[] mark_text = new TMP_Text[50];
    public Button disable_BET;
    public Button disable_clear;
    private float betAmount;
    private float totalAmount;
    private float[] dice_mark = new float[50];
    private int chipmark = 5;
    private string mark_array;
    public GameObject[] chip_button = new GameObject[5];
    public GameObject[] dice_button = new GameObject[50];
    public GameObject[] dice_display = new GameObject[3];
    public Texture[] dice = new Texture[6];
    public Texture chip;
    private Color color = Color.white;
    public static ReceiveJsonObject apiform;
    private string BaseUrl = "http://83.136.219.243:443";
    BetPlayer _player;

    void Start()
    {
        betAmount = 0f;
        BetAmount.text = betAmount.ToString("F2");
        _player = new BetPlayer();
        color.a = 0.4f;
        chip_button[0].GetComponent<Image>().color = color;
        clean();

#if UNITY_WEBGL == true && UNITY_EDITOR == false
        GameReady("Ready");
#endif

    }
    void Update()
    {

    }
    public void RequestToken(string data)
    {
        JSONNode usersInfo = JSON.Parse(data);
        _player.token = usersInfo["token"];
        totalAmount = float.Parse(usersInfo["amount"]);
        TotalAmount.text = totalAmount.ToString("F2");
    }
    private void clean()
    {
        color.a = 0f;
        betAmount = 0f;
        BetAmount.text = betAmount.ToString("F2");
        for (int i = 0; i < 50; i++)
        {
            dice_button[i].GetComponent<RawImage>().texture = null;
            dice_button[i].GetComponent<RawImage>().color = color;
            dice_mark[i] = 0f;
            mark_text[i].text = "";
        }
    }
    public void chip1()
    {
        chipmark = 5;
        chip_color(0);
    }
    public void chip2()
    {
        chipmark = 10;
        chip_color(1);
    }
    public void chip3()
    {
        chipmark = 20;
        chip_color(2);
    }
    public void chip4()
    {
        chipmark = 50;
        chip_color(3);
    }
    public void chip5()
    {
        chipmark = 100;
        chip_color(4);
    }
    private void chip_color(int index)
    {
        chip_button[0].GetComponent<Image>().color = Color.white;
        chip_button[1].GetComponent<Image>().color = Color.white;
        chip_button[2].GetComponent<Image>().color = Color.white;
        chip_button[3].GetComponent<Image>().color = Color.white;
        chip_button[4].GetComponent<Image>().color = Color.white;
        color.a = 0.4f;
        chip_button[index].GetComponent<Image>().color = color;
    }
    public void handleClickNumber(int index)
    {
        if (disable_BET.interactable)
        {
            if (betAmount + chipmark > totalAmount && totalAmount > 5f)
            {
                Alert.text = "";
                Alert.text = "MAXIMUM BET LIMIT " + totalAmount.ToString("F2") + "!";
            }
            else if (totalAmount < 5f)
            {
                Alert.text = "";
                Alert.text = "NOT ENOUGH BALANCE!";
            }
            else if (dice_mark[index - 1] + chipmark > 100)
            {
                Alert.text = "";
                Alert.text = "BET LIMIT 100.00!";
            }
            else
            {
                dice_button[index - 1].GetComponent<RawImage>().texture = chip;
                dice_button[index - 1].GetComponent<RawImage>().color = Color.white;
                switch (chipmark)
                {
                    case 5:
                        dice_mark[index - 1] += 5f;
                        mark_text[index - 1].text = dice_mark[index - 1].ToString();
                        betAmount += 5f;
                        BetAmount.text = betAmount.ToString("F2");
                        break;
                    case 10:
                        dice_mark[index - 1] += 10f;
                        mark_text[index - 1].text = dice_mark[index - 1].ToString();
                        betAmount += 10f;
                        BetAmount.text = betAmount.ToString("F2");
                        break;
                    case 20:
                        dice_mark[index - 1] += 20f;
                        mark_text[index - 1].text = dice_mark[index - 1].ToString();
                        betAmount += 20f;
                        BetAmount.text = betAmount.ToString("F2");
                        break;
                    case 50:
                        dice_mark[index - 1] += 50f;
                        mark_text[index - 1].text = dice_mark[index - 1].ToString();
                        betAmount += 50f;
                        BetAmount.text = betAmount.ToString("F2");
                        break;
                    case 100:
                        dice_mark[index - 1] += 100f;
                        mark_text[index - 1].text = dice_mark[index - 1].ToString();
                        betAmount += 100f;
                        BetAmount.text = betAmount.ToString("F2");
                        break;
                }
            }
        }
    }
    public void clear()
    {
        disable_BET.interactable = true;
        clean();
    }
    public void BET()
    {
        disable_BET.interactable = false;
        Alert.text = "";
        if (betAmount <= 0f)
        {
            Alert.text = "";
            Alert.text = "MINIMUM BET LIMIT 5.00!";
            disable_BET.interactable = true;
        }
        else if (totalAmount < 5f)
        {
            Alert.text = "";
            Alert.text = "NOT ENOUGH BALANCE!";
            disable_BET.interactable = true;
        }
        else if (betAmount > totalAmount)
        {
            Alert.text = "";
            Alert.text = "NOT ENOUGH BALANCE!";
            disable_BET.interactable = true;
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                color.a = 0f;
                dice_display[i].GetComponent<RawImage>().texture = null;
                dice_display[i].GetComponent<RawImage>().color = color;
            }
            StartCoroutine(Server());
        }
    }
    IEnumerator Server()
    {
        mark_array = "";
        for (int i = 0; i < 50; i++)
        {
            if (i == 0)
            {
                mark_array += dice_mark[i];
            }
            else
            {
                mark_array += "," + dice_mark[i];
            }
        }

        WWWForm form = new WWWForm();
        form.AddField("token", _player.token);
        form.AddField("betAmount", betAmount.ToString("F2"));
        form.AddField("dice_mark", mark_array);

        UnityWebRequest www = UnityWebRequest.Post(BaseUrl + "/api/BET", form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Alert.text = "";
            Alert.text = "CANNOT FIND SERVER!";
            disable_BET.interactable = true;
        }
        else
        {
            string strdata = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
            apiform = JsonUtility.FromJson<ReceiveJsonObject>(strdata);
            if (apiform.Message == "SUCCESS!")
            {
                totalAmount -= betAmount;
                TotalAmount.text = totalAmount.ToString("F2");
                for (int i = 0; i < 3; i++)
                {
                    yield return new WaitForSeconds(1f);
                    dice_display[i].GetComponent<RawImage>().texture = dice[apiform.diceArray[i] - 1];
                    dice_display[i].GetComponent<RawImage>().color = Color.white;
                }
                if (apiform.earnAmount > 0)
                {
                    totalAmount += apiform.earnAmount;
                    TotalAmount.text = totalAmount.ToString("F2");
                    Alert.text = "REWARD : " + apiform.earnAmount.ToString();
                    for (int i = 0; i < apiform.indexArray.Length; i++)
                    {
                        dice_button[apiform.indexArray[i]].GetComponent<RawImage>().color = Color.blue;
                    }
                }
                else
                {
                    Alert.text = "REWARD : 0.00";
                }
            }
            else if (apiform.Message == "BET ERROR!")
            {
                Alert.text = "";
                Alert.text = "BET ERROR!";
                disable_BET.interactable = true;
            }
            else if (apiform.Message == "SERVER ERROR!")
            {
                Alert.text = "";
                Alert.text = "SERVER ERROR!";
                disable_BET.interactable = true;
            }
        }
    }
}
public class BetPlayer
{
    public string token;
}
