using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIChat : MonoBehaviour
{
    public NetworkManager networkManager;
    public TMPro.TMP_Text ingamechat;
    public ScrollRect scrollRect;
    public int index = 0;
    List<string> chathistory = new List<string>();
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<TMPro.TMP_InputField>().Select();
        this.GetComponent<TMPro.TMP_InputField>().ActivateInputField();
        ingamechat.text = networkManager.ingamechat;     
    }

    // Update is called once per frame
    void Update()
    {
        this.GetComponent<TMPro.TMP_InputField>().Select();
        this.GetComponent<TMPro.TMP_InputField>().ActivateInputField();
        ingamechat.text = networkManager.ingamechat;
        

        if (Input.GetButtonUp("Submit"))
        {
            string text = this.GetComponent<TMPro.TMP_InputField>().text;
            if (text.Length > 0)
            {    
                chathistory.Insert(0, text);
                if (chathistory.Count >= 50)
                {
                    chathistory.RemoveAt(chathistory.Count-1);
                }
                //let's escape the pipe and colon symbols - everything else should be fine
                string escaped = this.GetComponent<TMPro.TMP_InputField>().text;
                escaped = escaped.Replace("|", ";;;pipesymbol;;;").Replace(":", ";;;colon;;;");
                networkManager.SendMsg("T:"+escaped+":\n");
                this.GetComponent<TMPro.TMP_InputField>().text = "";
                index = 0;
                scrollRect.verticalScrollbar.value = 0;
            }
            //Send chat
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            this.GetComponent<TMPro.TMP_InputField>().text = chathistory[index];
            index++;
            if (index > chathistory.Count-1)
                index = chathistory.Count-1;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            index--;
            if (index < 0)
            {
                this.GetComponent<TMPro.TMP_InputField>().text = "";
                index = 0;
            }
            else
            {
                this.GetComponent<TMPro.TMP_InputField>().text = chathistory[index];
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            this.GetComponent<TMPro.TMP_InputField>().text = "";
          //  World.Instance.chatactive = false;
          //  World.Instance.inUI = false;
            networkManager.chatactive = false;
            transform.parent.gameObject.SetActive(false);
            index = 0;
        }

    }
}
