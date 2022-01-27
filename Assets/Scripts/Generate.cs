using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Generate : MonoBehaviour
{
    private void Start()
    {
        Time.fixedDeltaTime = 1;
    }

    public GameObject npc;
    public InputField numberInput; 
    public void CreateNPC()
    {
        numberInput = GameObject.Find("InputFieldGenerateNumber").GetComponent<InputField>();
        if (!String.IsNullOrEmpty(numberInput.text))
        {
            for (int i = 0; i < int.Parse(numberInput.text); i++)
            {
                Instantiate(npc,new Vector3((float)(Random.Range(-310,310)*0.1),(float)(Random.Range(-250,250)*0.1)), new Quaternion());
            }
        }
    }
}
