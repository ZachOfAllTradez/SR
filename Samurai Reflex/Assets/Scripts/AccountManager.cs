using UnityEngine;
using System.Collections;
using PlayFab;

public class AccountManager : MonoBehaviour 
{
	public static string TITLE_ID = "DE34";
	public static string SESSION_TICKET;
	public static string SCREEN_NAME;

	void Awake()
	{
		PlayFabSettings.TitleId = TITLE_ID;
	}

	// Use this for initialization
	void Start()
	{
	
	}
	
	// Update is called once per frame
	void Update()
	{
	
	}
}
