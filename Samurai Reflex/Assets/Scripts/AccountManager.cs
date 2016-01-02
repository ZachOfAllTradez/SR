using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;

public class AccountManager : MonoBehaviour 
{
	public static string TITLE_ID = "DE34";
	public static string SESSION_TICKET = "";

	public static string SCREEN_NAME;
    public static bool SCREEN_NAME_VERIFIED;
    public static bool EMAIL_VERIFIED;
    public static int XP_RED;
    public static int XP_YELLOW;
    public static int XP_BLUE;
    public static int STATSET_1;
    public static int STATSET_2;

    //private static uint currentDataVersion = 0;

	void Awake()
	{
        DontDestroyOnLoad(this);
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


    public static void InitializePlayerData()
    {
        SCREEN_NAME_VERIFIED = false;
        EMAIL_VERIFIED = false;
        XP_RED = 0;
        XP_YELLOW = 0;
        XP_BLUE = 0;
        STATSET_1 = 0;
        STATSET_2 = 0;

        UpdateUserDataRequest request = new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
            {
                { "screenNameVerified", SCREEN_NAME_VERIFIED.ToString() },
                { "emailVerified", EMAIL_VERIFIED.ToString() },
                { "xpRed", XP_RED.ToString() },
                { "xpYellow", XP_YELLOW.ToString() },
                { "xpBlue", XP_BLUE.ToString() },
                { "statSet1", STATSET_1.ToString() },
                { "statSet2", STATSET_2.ToString() }
            }, 
            Permission = UserDataPermission.Private
        };

        PlayFabClientAPI.UpdateUserData(request, (result) =>
            {
                Debug.Log("User data initialized.");
                //currentDataVersion = result.DataVersion;
            },
            (error) =>
            {
                Debug.LogError(error.ErrorMessage + " " + error.Error.GetHashCode());
            });
    }


	public static void DownloadPlayerData()
    {
        UserDataRecord temp;
        if (!SESSION_TICKET.Equals(""))
        {
            // Get player data using session ticket
            GetUserDataRequest request = new GetUserDataRequest()
            {
                // IfChangedFromDataVersion = (int)currentDataVersion
            };

            PlayFabClientAPI.GetUserData(request, (result) =>
                {
                    result.Data.TryGetValue("screenNameVerified", out temp);
                    SCREEN_NAME_VERIFIED = Convert.ToBoolean(temp.Value);
                    result.Data.TryGetValue("emailVerified", out temp);
                    EMAIL_VERIFIED = Convert.ToBoolean(temp.Value);
                    result.Data.TryGetValue("xpRed", out temp);
                    XP_RED = Convert.ToInt32(temp.Value);
                    result.Data.TryGetValue("xpYellow", out temp);
                    XP_YELLOW = Convert.ToInt32(temp.Value);
                    result.Data.TryGetValue("xpBlue", out temp);
                    XP_BLUE = Convert.ToInt32(temp.Value);
                    result.Data.TryGetValue("statSet1", out temp);
                    STATSET_1 = Convert.ToInt32(temp.Value);
                    result.Data.TryGetValue("statSet2", out temp);
                    STATSET_2 = Convert.ToInt32(temp.Value);

                    // currentDataVersion = result.DataVersion;
                }, (error) =>
                {
                    Debug.LogError(error.ErrorMessage + " " + error.Error.GetHashCode());
                });
        }
    }


    public static void DownloadPlayerData(List<string> keys)
    {
        UserDataRecord temp;
        if (!SESSION_TICKET.Equals(""))
        {
            // Get player data using session ticket
            GetUserDataRequest request = new GetUserDataRequest()
            {
                // IfChangedFromDataVersion = (int)currentDataVersion,
                Keys = keys
            };

            PlayFabClientAPI.GetUserData(request, (result) =>
                {
                    foreach (string key in keys)
                    {
                        switch (key)
                        {
                            case "screenNameVerified":
                                result.Data.TryGetValue("screenNameVerified", out temp);
                                SCREEN_NAME_VERIFIED = Convert.ToBoolean(temp.Value);
                                break;
                            case "emailVerified":
                                result.Data.TryGetValue("emailVerified", out temp);
                                EMAIL_VERIFIED = Convert.ToBoolean(temp.Value);
                                break;
                            case "xpRed":
                                result.Data.TryGetValue("xpRed", out temp);
                                XP_RED = Convert.ToInt32(temp.Value);
                                break;
                            case "xpYellow":
                                result.Data.TryGetValue("xpYellow", out temp);
                                XP_YELLOW = Convert.ToInt32(temp.Value);
                                break;
                            case "xpBlue":
                                result.Data.TryGetValue("xpBlue", out temp);
                                XP_BLUE = Convert.ToInt32(temp.Value);
                                break;
                            case "statSet1":
                                result.Data.TryGetValue("statSet1", out temp);
                                STATSET_1 = Convert.ToInt32(temp.Value);
                                break;
                            case "statSet2":
                                result.Data.TryGetValue("statSet2", out temp);
                                STATSET_2 = Convert.ToInt32(temp.Value);
                                break;
                        }
                    }

                    // currentDataVersion = result.DataVersion;
                }, (error) =>
                {
                    Debug.LogError(error.ErrorMessage + " " + error.Error.GetHashCode());
                });
        }
    }


    public static void UploadPlayerData()
    {        
        UpdateUserDataRequest request = new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
            {
                { "screenNameVerified", SCREEN_NAME_VERIFIED.ToString() },
                { "emailVerified", EMAIL_VERIFIED.ToString() },
                { "xpRed", XP_RED.ToString() },
                { "xpYellow", XP_YELLOW.ToString() },
                { "xpBlue", XP_BLUE.ToString() },
                { "statSet1", STATSET_1.ToString() },
                { "statSet2", STATSET_2.ToString() }
            }, 
            Permission = UserDataPermission.Private
        };

        PlayFabClientAPI.UpdateUserData(request, (result) =>
            {
                Debug.Log("User data uploaded.");
                // currentDataVersion = result.DataVersion;
            },
            (error) =>
            {
                Debug.LogError(error.ErrorMessage + " " + error.Error.GetHashCode());
            });
    }


    public static void UploadPlayerData(List<string> keys)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();

        foreach (string key in keys)
        {
            switch (key)
            {
                case "screenNameVerified":
                    data.Add(key, SCREEN_NAME_VERIFIED.ToString());
                    break;
                case "emailVerified":
                    data.Add(key, EMAIL_VERIFIED.ToString());
                    break;
                case "xpRed":
                    data.Add(key, XP_RED.ToString());
                    break;
                case "xpYellow":
                    data.Add(key, XP_YELLOW.ToString());
                    break;
                case "xpBlue":
                    data.Add(key, XP_BLUE.ToString());
                    break;
                case "statSet1":
                    data.Add(key, STATSET_1.ToString());
                    break;
                case "statSet2":
                    data.Add(key, STATSET_2.ToString());
                    break;
            }
        }

        UpdateUserDataRequest request = new UpdateUserDataRequest()
        {
            Data = data, 
            Permission = UserDataPermission.Private
        };

        PlayFabClientAPI.UpdateUserData(request, (result) =>
            {
                Debug.Log("User data uploaded.");
            },
            (error) =>
            {
                Debug.LogError(error.ErrorMessage + " " + error.Error.GetHashCode());
            });        
    }


}
