using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Net;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;
using M2MqttUnity;
//using Newtonsoft.Json;

using System;

//public class Test : MonoBehaviour
public class Test : MonoBehaviour
{
	//public button ButtonPub;
	private MqttClient client;
	//public GameObject Light1;
	//public GameObject Light2;
	public Button Auto_bt;
	public Button Manu_bt;
	
	public GameObject lightVal1;
	public GameObject lightVal2;
	//public GameObject lightPFlow;
	public GameObject lightPFilter;
	public GameObject lightTank1;
	public GameObject lightTank2;
	public GameObject lightTank3;
	public GameObject lightMotor;
	public GameObject lightValve3Way;
	public GameObject lightPurgeFilter;

	[SerializeField]
	private Text TimeVal_to_ON;
	[SerializeField]
	private Text TimeVal1_to_OFF;
	[SerializeField]
	private Text TimeVal2_to_OFF;
	[SerializeField]
	private Text TimeBreak;
	[SerializeField]
	private Text TimePFilter_to_ON;
	[SerializeField]
	private Text TimePFilter_to_OFF;
	[SerializeField]
	private Text TimePFlow_to_ON;
	[SerializeField]
	private Text TimePFlow_to_OFF;


	public PlayerData Spare1;
	public PlayerData Spare2;
	public bool throughStarter_Topic1 = false;
	public bool throughStarter_Topic2 = false;
	public bool lightValidation1;
	public bool lightValidation2;
	public bool lightPurgefilter;
	public bool lightPurgeFlow;
	public bool lightTank1_temp;
	public bool lightTank2_temp;
	public bool lightTank3_temp;
	public bool lightVal3Way_temp;
	public bool light_Motor;
	public bool light_PurgeFilter_Temp;

	private int countTimer = 0;

	public InputField consoleInputField;


	// Use this for initialization
	void Start()
	{
		PlayerData playerData = new PlayerData();
		// create client instance 
		client = new MqttClient("broker.hivemq.com");

		// register to message received 
		client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
		client.MqttMsgPublishReceived += client_MqttMsg;

		string clientId = Guid.NewGuid().ToString();
		client.Connect(clientId);
		// subscribe to the topic "/home/temperature" with QoS 2 
		//client.Subscribe(new string[] { "BerData/VPSandSite" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
		client.Subscribe(new string[] { "BerData/Status" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
		client.Subscribe(new string[] { "BerData/Time" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
        if (client.IsConnected)
        {
            consoleInputField.text = "Connection successfully";
        }
        else
        {
            consoleInputField.text = "Connection failed";
        }
    }
	[System.Serializable]
	public class Pars
	{
		public string tag;
		public int value;
	}
	[System.Serializable]
	public class PlayerData
	{	
		//public int value;
		public Pars[] d;
	}

	public void SetUiMessage(string msg)
	{
		if (consoleInputField != null)
		{
			consoleInputField.text = msg;
			//updateUI = true;
			//M2MqttUnity.M2MqttUnityClient
		}
	}

	public void AddUiMessage(string msg)
	{
		if (consoleInputField != null)
		{
			consoleInputField.text += msg + "\n";
			//updateUI = true;
		}
	}				
    void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
	
	{
		if(e.Topic == "BerData/Status")
        {
            string msg = System.Text.Encoding.UTF8.GetString(e.Message);
			msg = HandleString(msg, 31);
			//msg = HandleString(msg, 9);
			PlayerData data = JsonUtility.FromJson<PlayerData>(msg);
            throughStarter_Topic2 = true;
            GetData2(data);
            if (data.d[0].value == 1)
            {
                lightTank1_temp = true;
            }
            else lightTank1_temp = false;
            if (data.d[1].value == 1)
            {
                lightTank2_temp = true;
            }
            else lightTank2_temp = false;
            if (data.d[2].value == 1)
            {
                lightTank3_temp = true;
            }
            else lightTank3_temp = false;
            if (data.d[3].value == 1)
            {
                lightVal3Way_temp = true;
            }
            else lightVal3Way_temp = false;
            if (data.d[4].value == 1)
            {
                lightValidation1 = true;
            }
            else lightValidation1 = false;
            if (data.d[5].value == 1)
            {
                lightValidation2 = true;
            }
            else lightValidation2 = false;
			if (data.d[7].value == 1)
			{
				light_PurgeFilter_Temp = true;
			}
			else light_PurgeFilter_Temp = false;
			if (data.d[8].value == 1)
			{
				light_Motor = true;
			}
			else light_Motor = false;
		}			
	}
	void client_MqttMsg(object sender, MqttMsgPublishEventArgs e)
    {
		if (e.Topic == "BerData/Time")
		{
			string msg = System.Text.Encoding.UTF8.GetString(e.Message);
			msg = HandleString(msg, 29);
			PlayerData data = JsonUtility.FromJson<PlayerData>(msg);
			throughStarter_Topic1 = true;
			GetData1(data);
			//PlayerData[] player = JsonHelper.FromJson<PlayerData>(msg);
			//Debug.Log("Received: " + msg);
			//Debug.Log("Received: " + data.d[4].value.ToString());
		}
	}		
	public void GetData1(PlayerData data)
	{
		Spare1 = data;			
	}
	public void GetData2(PlayerData data)
	{
		Spare2 = data;
	}
	public string HandleString(string data, int numBer)
    {
		string mergeString = "";
		string[] splitString = data.Split(',');
		for(int i=0;i<numBer;i++)
        {
			mergeString += splitString[i] + ",";
        }
		mergeString += splitString[numBer] + "}";
		//mergeString = splitString[0] +","+ splitString[1] + "," + splitString[2] + "," + splitString[3] + "," + splitString[4] + "," + splitString[5] + "," + splitString[6] + "," + splitString[7] + "," + splitString[8] + "," + splitString[9] + "}";
		return mergeString;
    }		
	public static class JsonHelper
	{
		public static T[] FromJson<T>(string json)
		{
			Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
			return wrapper.d;
		}

		public static string ToJson<T>(T[] array)
		{
			Wrapper<T> wrapper = new Wrapper<T>();
			wrapper.d = array;
			return JsonUtility.ToJson(wrapper);
		}

		public static string ToJson<T>(T[] array, bool prettyPrint)
		{
			Wrapper<T> wrapper = new Wrapper<T>();
			wrapper.d = array;
			return JsonUtility.ToJson(wrapper, prettyPrint);
		}

		[Serializable]
		private class Wrapper<T>
		{
			public T[] d;
		}
	}
	void Update()
	{
		//countTimer++;
		//if (countTimer == 1)
  //      {
			if (throughStarter_Topic1 == true)
			{
				TimePFilter_to_ON.text = ((Spare1.d[8].value - Spare1.d[0].value)/1000).ToString();
				TimePFilter_to_OFF.text = ((Spare1.d[9].value - Spare1.d[1].value)/1000).ToString();
				TimePFlow_to_ON.text = ((Spare1.d[13].value - Spare1.d[2].value)/1000).ToString();
				TimePFlow_to_OFF.text = ((Spare1.d[14].value - Spare1.d[3].value)/1000).ToString();
				TimeVal_to_ON.text = ((Spare1.d[10].value - Spare1.d[4].value)/1000).ToString();
				TimeVal1_to_OFF.text = ((Spare1.d[11].value - Spare1.d[5].value)/1000).ToString();
				TimeVal2_to_OFF.text = ((Spare1.d[11].value - Spare1.d[6].value)/1000).ToString();
				TimeBreak.text = ((Spare1.d[12].value - Spare1.d[7].value)/1000).ToString();
			}
			if (throughStarter_Topic2 == true)
			{
				if (lightValidation2 == true)
				{
					lightVal2.GetComponent<Image>().color = new Color32(255, 160, 122, 225);
				}
				else lightVal2.GetComponent<Image>().color = new Color32(170, 183, 180, 255);
			}
			if (throughStarter_Topic2 == true)
			{
				if (lightValidation2 == true)
				{
					lightVal2.GetComponent<Image>().color = new Color32(255, 160, 122, 225);
				}
				else lightVal2.GetComponent<Image>().color = new Color32(170, 183, 180, 255);
				if (lightValidation1 == true)
				{
					lightVal1.GetComponent<Image>().color = new Color32(255, 160, 122, 225);
				}
				else lightVal1.GetComponent<Image>().color = new Color32(170, 183, 180, 255);
				if (lightTank1_temp == true)
				{
					lightTank1.GetComponent<Image>().color = new Color32(255, 160, 122, 225);
				}
				else lightTank1.GetComponent<Image>().color = new Color32(170, 183, 180, 255);
				if (lightTank2_temp == true)
				{
					lightTank2.GetComponent<Image>().color = new Color32(255, 160, 122, 225);
				}
				else lightTank2.GetComponent<Image>().color = new Color32(170, 183, 180, 255);
				if (lightTank3_temp == true)
				{
					lightTank3.GetComponent<Image>().color = new Color32(255, 160, 122, 225);
				}
				else lightTank3.GetComponent<Image>().color = new Color32(170, 183, 180, 255);
				if (light_Motor == true)
				{
					lightMotor.GetComponent<Image>().color = new Color32(255, 160, 122, 225);
				}
				else lightMotor.GetComponent<Image>().color = new Color32(170, 183, 180, 255);
				if (lightVal3Way_temp == true)
				{
					lightValve3Way.GetComponent<Image>().color = new Color32(255, 160, 122, 225);
				}
				else lightValve3Way.GetComponent<Image>().color = new Color32(170, 183, 180, 255);
				if (light_PurgeFilter_Temp == true)
				{
				lightPurgeFilter.GetComponent<Image>().color = new Color32(255, 160, 122, 225);
				}
				else lightPurgeFilter.GetComponent<Image>().color = new Color32(170, 183, 180, 255);

			//}
			//if (countTimer == 50)
   //         {
			//	countTimer = 0;
   //         }				
		}			
		
	}
	public void publish_On_Validation1()
    {
        Debug.Log("sending...");
        client.Publish("BerData/VPSandSITE", System.Text.Encoding.UTF8.GetBytes("{\"w\":[{\"tag\":\"Valve_Validation_1_Site\",\"value\":1}]}"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
        Debug.Log("sent");
		//Light1.GetComponent<Image>().color = new Color32(255, 160, 122, 225);
	}
	public void publish_OFF__Validation1()
    {
		Debug.Log("sending...");
		client.Publish("BerData/VPSandSITE", System.Text.Encoding.UTF8.GetBytes("{\"w\":[{\"tag\":\"Valve_Validation_1_Site\",\"value\":0}]}"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
		Debug.Log("sent");
		//Light1.GetComponent<Image>().color = new Color32(170, 183, 180, 255);
	}
	public void publish_On_Validation2()
	{
		Debug.Log("sending...");
		client.Publish("BerData/VPSandSITE", System.Text.Encoding.UTF8.GetBytes("{\"w\":[{\"tag\":\"Valve_Validation_2_Site\",\"value\":1}]}"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
		Debug.Log("sent");
		//Light2.GetComponent<Image>().color = new Color32(255, 160, 122, 225);
		//client.Publish("hello/world", System.te); 
		
	}
	public void publish_OFF_Validation2()
	{
		Debug.Log("sending...");
		client.Publish("BerData/VPSandSITE", System.Text.Encoding.UTF8.GetBytes("{\"w\":[{\"tag\":\"Valve_Validation_2_Site\",\"value\":0}]}"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
		//Debug.Log("{\"w\":[{\"tag\":\"Valve_Tank_1_VPS\",\"value\":0}]}");
		//Light2.GetComponent<Image>().color = new Color32(170, 183, 180, 255);
	}
	public void publish_Auto()
	{
		Debug.Log("sending...");
		client.Publish("BerData/VPSandSITE", System.Text.Encoding.UTF8.GetBytes("{\"w\":[{\"tag\":\"Auto_Manu_Site\",\"value\":0}]}"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
		//Debug.Log("{\"w\":[{\"tag\":\"Valve_Tank_1_VPS\",\"value\":0}]}");
		//Auto_bt.GetComponent<Button>().colors.normalColor = Color.red;
		var colors = Auto_bt.GetComponent<Button>().colors;
		colors.normalColor = Color.red;
		Auto_bt.GetComponent<Button>().colors = colors;

		var colors1 = Manu_bt.GetComponent<Button>().colors;
		colors1.normalColor = Color.white;
		Manu_bt.GetComponent<Button>().colors = colors;

	}
	public void publish_Manu()
	{
		Debug.Log("sending...");
		client.Publish("BerData/VPSandSITE", System.Text.Encoding.UTF8.GetBytes("{\"w\":[{\"tag\":\"Auto_Manu_Site\",\"value\":1}]}"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
		//Debug.Log("{\"w\":[{\"tag\":\"Valve_Tank_1_VPS\",\"value\":0}]}");
		//Light2.GetComponent<Image>().color = new Color32(170, 183, 180, 255);
		var colors = Auto_bt.GetComponent<Button>().colors;
		colors.normalColor = Color.white;
		Auto_bt.GetComponent<Button>().colors = colors;

		var colors1 = Manu_bt.GetComponent<Button>().colors;
		colors1.normalColor = Color.red;
		Manu_bt.GetComponent<Button>().colors = colors;
	}
	public void publish_ValveTank1_ON()
	{
		Debug.Log("sending...");
		client.Publish("BerData/VPSandSITE", System.Text.Encoding.UTF8.GetBytes("{\"w\":[{\"tag\":\"Valve_Tank_1_Site\",\"value\":1}]}"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
		//Debug.Log("{\"w\":[{\"tag\":\"Valve_Tank_1_VPS\",\"value\":0}]}");
		//Light2.GetComponent<Image>().color = new Color32(170, 183, 180, 255);
	}
	public void publish_ValveTank1_OFF()
	{
		Debug.Log("sending...");
		client.Publish("BerData/VPSandSITE", System.Text.Encoding.UTF8.GetBytes("{\"w\":[{\"tag\":\"Valve_Tank_1_Site\",\"value\":0}]}"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
		//Debug.Log("{\"w\":[{\"tag\":\"Valve_Tank_1_VPS\",\"value\":0}]}");
		//Light2.GetComponent<Image>().color = new Color32(170, 183, 180, 255);
	}
	public void publish_ValveTank2_ON()
	{
		Debug.Log("sending...");
		client.Publish("BerData/VPSandSITE", System.Text.Encoding.UTF8.GetBytes("{\"w\":[{\"tag\":\"Valve_Tank_2_Site\",\"value\":1}]}"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
		//Debug.Log("{\"w\":[{\"tag\":\"Valve_Tank_1_VPS\",\"value\":0}]}");
		//Light2.GetComponent<Image>().color = new Color32(170, 183, 180, 255);
	}
	public void publish_ValveTank2_OFF()
	{
		Debug.Log("sending...");
		client.Publish("BerData/VPSandSITE", System.Text.Encoding.UTF8.GetBytes("{\"w\":[{\"tag\":\"Valve_Tank_2_Site\",\"value\":0}]}"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
		//Debug.Log("{\"w\":[{\"tag\":\"Valve_Tank_1_VPS\",\"value\":0}]}");
		//Light2.GetComponent<Image>().color = new Color32(170, 183, 180, 255);
	}
	public void publish_ValveTank3_ON()
	{
		Debug.Log("sending...");
		client.Publish("BerData/VPSandSITE", System.Text.Encoding.UTF8.GetBytes("{\"w\":[{\"tag\":\"Valve_Tank_3_Site\",\"value\":1}]}"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
		//Debug.Log("{\"w\":[{\"tag\":\"Valve_Tank_1_VPS\",\"value\":0}]}");
		//Light2.GetComponent<Image>().color = new Color32(170, 183, 180, 255);
	}
	public void publish_ValveTank3_OFF()
	{
		Debug.Log("sending...");
		client.Publish("BerData/VPSandSITE", System.Text.Encoding.UTF8.GetBytes("{\"w\":[{\"tag\":\"Valve_Tank_3_Site\",\"value\":0}]}"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
		//Debug.Log("{\"w\":[{\"tag\":\"Valve_Tank_1_VPS\",\"value\":0}]}");
		//Light2.GetComponent<Image>().color = new Color32(170, 183, 180, 255);
	}
	public void publish_Valve_3_Way_ON()
	{
		Debug.Log("sending...");
		client.Publish("BerData/VPSandSITE", System.Text.Encoding.UTF8.GetBytes("{\"w\":[{\"tag\":\"Valve_3_Way_Site\",\"value\":1}]}"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
		//Debug.Log("{\"w\":[{\"tag\":\"Valve_Tank_1_VPS\",\"value\":0}]}");
		//Light2.GetComponent<Image>().color = new Color32(170, 183, 180, 255);
	}
	public void publish_Valve_3_Way_OFF()
	{
		Debug.Log("sending...");
		client.Publish("BerData/VPSandSITE", System.Text.Encoding.UTF8.GetBytes("{\"w\":[{\"tag\":\"Valve_3_Way_Site\",\"value\":0}]}"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
		//Debug.Log("{\"w\":[{\"tag\":\"Valve_Tank_1_VPS\",\"value\":0}]}");
		//Light2.GetComponent<Image>().color = new Color32(170, 183, 180, 255);
	}
	public void publish_Motor_ON()
	{
		Debug.Log("sending...");
		client.Publish("BerData/VPSandSITE", System.Text.Encoding.UTF8.GetBytes("{\"w\":[{\"tag\":\"Motor_Site\",\"value\":1}]}"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
		//Debug.Log("{\"w\":[{\"tag\":\"Valve_Tank_1_VPS\",\"value\":0}]}");
		//Light2.GetComponent<Image>().color = new Color32(170, 183, 180, 255);
	}
	public void publish_Motor_OFF()
	{
		Debug.Log("sending...");
		client.Publish("BerData/VPSandSITE", System.Text.Encoding.UTF8.GetBytes("{\"w\":[{\"tag\":\"Motor_Site\",\"value\":0}]}"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
		//Debug.Log("{\"w\":[{\"tag\":\"Valve_Tank_1_VPS\",\"value\":0}]}");
		//Light2.GetComponent<Image>().color = new Color32(170, 183, 180, 255);
	}
	public void publish_PurgeFilter_ON()
	{
		Debug.Log("sending...");
		client.Publish("BerData/VPSandSITE", System.Text.Encoding.UTF8.GetBytes("{\"w\":[{\"tag\":\"Purge_Filter_Site\",\"value\":1}]}"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
		//Debug.Log("{\"w\":[{\"tag\":\"Valve_Tank_1_VPS\",\"value\":0}]}");
		//Light2.GetComponent<Image>().color = new Color32(170, 183, 180, 255);
	}
	public void publish_PurgeFilter_OFF() 
	{
		Debug.Log("sending...");
		client.Publish("BerData/VPSandSITE", System.Text.Encoding.UTF8.GetBytes("{\"w\":[{\"tag\":\"Purge_Filter_Site\",\"value\":0}]}"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
		//Debug.Log("{\"w\":[{\"tag\":\"Valve_Tank_1_VPS\",\"value\":0}]}");
		//Light2.GetComponent<Image>().color = new Color32(170, 183, 180, 255);
	}


}
