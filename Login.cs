using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Login : MonoBehaviour
{
    public GameObject username;
    public GameObject password;
    private string Username;
    private string Password;
    public GameObject LoginPanel;
    public GameObject ARCamera;
    public GameObject IntroMachineCamera;
    public GameObject MCBCamera1;
    public GameObject MCBCamera2;
    public Text Status;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Username = username.GetComponent<InputField>().text;
        Password = password.GetComponent<InputField>().text;
        if (Username != "admin" || Password != "admin")
        {
            ARCamera.SetActive(false);
            IntroMachineCamera.SetActive(false);
            MCBCamera1.SetActive(false);
            MCBCamera2.SetActive(false);
        }
        else
        {
            ARCamera.SetActive(true);
            IntroMachineCamera.SetActive(true);
            MCBCamera1.SetActive(true);
            MCBCamera2.SetActive(true);
        }
    }
    public void loginButton()
    {
        if (Username == "admin" && Password == "admin")
        {
            LoginPanel.SetActive(false);
            ARCamera.SetActive(true);
            IntroMachineCamera.SetActive(true);
            MCBCamera1.SetActive(true);
            MCBCamera2.SetActive(true);
            Status.text = "Successfull";
        }
        else
        {
            LoginPanel.SetActive(true);
            ARCamera.SetActive(false);
            IntroMachineCamera.SetActive(false);
            MCBCamera1.SetActive(false);
            MCBCamera2.SetActive(false);
            Status.text = "Failed, Please try again";
        }
    }
}
