using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILoginWindow : MonoBehaviour
{    
    private Button enterBtn;
    private InputField inputUsername;
    private Text infoMsg;
   
    private void Start()
    {
        enterBtn = transform.Find("enterBtn").GetComponent<Button>();
        inputUsername = transform.Find("InputUsername").GetComponent<InputField>();
        infoMsg = transform.Find("InfoMsg").GetComponent<Text>();
        enterBtn.onClick.AddListener(SubmitUsername);        
    }
    
    public void Show()
    {
        this.gameObject.SetActive(true);        
    }

    void SubmitUsername()
    {
        enterBtn.interactable = false;
        inputUsername.interactable = false;
        string username = inputUsername.textComponent.text;
        if (username == null || username == "")
        {
            infoMsg.text = "Please input username";
            enterBtn.interactable = true;
            inputUsername.interactable = true;
            inputUsername.textComponent.text = "";
        } else
        {
            ServerSocketManager.instance.RequestJoin(username);
            infoMsg.text = "Connecting...";
        }
    }

    public void Hide()
    {        
        this.gameObject.SetActive(false);
    }    
    public void SetInfoMsg(string msg)
    {        
        infoMsg.text = msg;
    }
}
