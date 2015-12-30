using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text.RegularExpressions;
using PlayFab;
using PlayFab.ClientModels;

public class LoginControl : MonoBehaviour
{
    public Transform emailObject;
    public Transform screenNameObject;
    public Transform passwordObject;
    public Transform confirmPasswordObject;
    public Transform lowerButtonsObject;
    public Transform passwordTarget;
    public Transform lowerButtonsTarget;
	public GameObject loginButton;
	public GameObject loginText;
	public Text messageObject;

    public float expandSpeed = 0.1f;
    public float fadeInSpeed = 0.05f;
    public float fadeOutSpeed = 0.15f;

	private bool newAccount = false;
    private Vector3 passwordStartPosition;
    private Vector3 lowerStartPosition;
    private Text screenNameText;
    private Text confirmPasswordText;
    private Image screenNameImage;
    private Image confirmPasswordImage;
    private float screenNameStartAlpha;
    private float confirmStartAlpha;
    private float screenNameImageStartAlpha;
    private float confirmImageStartAlpha;
    private ColorBlock tempColorBlock;

	private string responsePFID;

	private bool validScreenName = false;
	private bool validPassword = false;


	// Use this for initialization
	void Start()
	{
        passwordStartPosition = passwordObject.position;
        lowerStartPosition = lowerButtonsObject.position;
        screenNameText = screenNameObject.GetComponent<Text>();
        confirmPasswordText = confirmPasswordObject.GetComponent<Text>();
        screenNameImage = screenNameObject.FindChild("InputField").GetComponent<Image>();
        confirmPasswordImage = confirmPasswordObject.FindChild("InputField").GetComponent<Image>();
        screenNameStartAlpha = screenNameText.color.a;
        confirmStartAlpha = confirmPasswordText.color.a;
        screenNameImageStartAlpha = screenNameImage.color.a;
        confirmImageStartAlpha = confirmPasswordImage.color.a;
		loginButton.SetActive(true);
		loginText.SetActive(false);
		messageObject.text = "";
	}
	
	// Update is called once per frame
	void Update() 
	{
		if(Input.GetButtonDown("Submit"))
		{
			Login();
		}
	}


	public void ToggleNewAccount()
	{
		newAccount = !newAccount;
        if(newAccount)
        {
            lowerButtonsObject.FindChild("New/Text").GetComponent<Text>().text = "Back";
            lowerButtonsObject.FindChild("Login Button/Text").GetComponent<Text>().text = "Create New";
            screenNameObject.FindChild("InputField").GetComponent<InputField>().interactable = true;
            confirmPasswordObject.FindChild("InputField").GetComponent<InputField>().interactable = true;
            StopCoroutine("CollapseForm");
            StopCoroutine("FadeOutForm");
            StartCoroutine("ExpandForm");
            StartCoroutine("FadeInForm");
        }
        else
        {
			//Debug.Log (screenNameObject.FindChild ("InputField").GetComponent<InputField> ().text);
            lowerButtonsObject.FindChild("New/Text").GetComponent<Text>().text = "New Account";
            lowerButtonsObject.FindChild("Login Button/Text").GetComponent<Text>().text = "Login";
            screenNameObject.FindChild("InputField").GetComponent<InputField>().text = "";
            confirmPasswordObject.FindChild("InputField").GetComponent<InputField>().text = "";
            screenNameObject.FindChild("InputField").GetComponent<InputField>().interactable = false;
            confirmPasswordObject.FindChild("InputField").GetComponent<InputField>().interactable = false;
            StopCoroutine("ExpandForm");
            StopCoroutine("FadeInForm");
            StartCoroutine("CollapseForm");
            StartCoroutine("FadeOutForm");
        }
	}


    public void ForgotPassword()
    {
        //Application.OpenURL("http://www.alltradesgames.com");
		messageObject.text = "";
		SendAccountRecoveryEmailRequest request = new SendAccountRecoveryEmailRequest()
		{
			TitleId = AccountManager.TITLE_ID,
			Email = emailObject.FindChild("InputField").GetComponent<InputField>().text,
		};

		PlayFabClientAPI.SendAccountRecoveryEmail(request, (result) => {					
			messageObject.text = "Password Recovery Email Sent From 'PlayFab'.";
			messageObject.color = Color.green;
		}, 
			(error) => {						
				Debug.LogError(error.ErrorMessage+" "+error.Error.GetHashCode());
				switch(error.Error.GetHashCode())
				{
				case 1000:
					messageObject.text = "Error: Invalid Email Address.";
					messageObject.color = Color.red;

                    tempColorBlock = emailObject.FindChild("InputField").GetComponent<InputField>().colors;
                    tempColorBlock.normalColor = Color.red;
                    emailObject.FindChild("InputField").GetComponent<InputField>().colors = tempColorBlock;
					break;
				case 1001:
					messageObject.text = "Error: No account with this email address exists.";
					messageObject.color = Color.red;

                    tempColorBlock = emailObject.FindChild("InputField").GetComponent<InputField>().colors;
                    tempColorBlock.normalColor = Color.red;
                    emailObject.FindChild("InputField").GetComponent<InputField>().colors = tempColorBlock;
					break;
				}
			});
    }


	IEnumerator ExpandForm()
	{
        while ((Vector3.Distance(passwordObject.position, passwordTarget.position)>0.01f) || (Vector3.Distance(lowerButtonsObject.position, lowerButtonsTarget.position)>0.01f))
        {
            passwordObject.position = Vector3.Lerp(passwordObject.position, passwordTarget.position, expandSpeed);
            lowerButtonsObject.position = Vector3.Lerp(lowerButtonsObject.position, lowerButtonsTarget.position, expandSpeed);
            yield return null;
        }
	}


    IEnumerator CollapseForm()
    {
        while ((Vector3.Distance(passwordObject.position, passwordStartPosition)>0.01f) || (Vector3.Distance(lowerButtonsObject.position, lowerStartPosition)>0.01f))
        {
            passwordObject.position = Vector3.Lerp(passwordObject.position, passwordStartPosition, expandSpeed);
            lowerButtonsObject.position = Vector3.Lerp(lowerButtonsObject.position, lowerStartPosition, expandSpeed);
            yield return null;
        }
    }


    IEnumerator FadeInForm()
    {
        while(((1f - screenNameText.color.a)>0.001f) || ((1f - screenNameImage.color.a)>0.001f))
        {
            screenNameText.color = new Color(screenNameText.color.r, screenNameText.color.g, screenNameText.color.b, Mathf.Lerp(screenNameText.color.a, 1f, fadeInSpeed));
            screenNameImage.color = new Color(screenNameImage.color.r, screenNameImage.color.g, screenNameImage.color.b, Mathf.Lerp(screenNameImage.color.a, 1f, fadeInSpeed));
            confirmPasswordText.color = new Color(confirmPasswordText.color.r, confirmPasswordText.color.g, confirmPasswordText.color.b, Mathf.Lerp(confirmPasswordText.color.a, 1f, fadeInSpeed));
            confirmPasswordImage.color = new Color(confirmPasswordImage.color.r, confirmPasswordImage.color.g, confirmPasswordImage.color.b, Mathf.Lerp(confirmPasswordImage.color.a, 1f, fadeInSpeed));
            yield return null;
        }
    }


    IEnumerator FadeOutForm()
    {
        while((screenNameText.color.a>0.001f) || (screenNameImage.color.a>0.001f))
        {
            screenNameText.color = new Color(screenNameText.color.r, screenNameText.color.g, screenNameText.color.b, Mathf.Lerp(screenNameText.color.a, 0f, fadeOutSpeed));
            screenNameImage.color = new Color(screenNameImage.color.r, screenNameImage.color.g, screenNameImage.color.b, Mathf.Lerp(screenNameImage.color.a, 0f, fadeOutSpeed));
            confirmPasswordText.color = new Color(confirmPasswordText.color.r, confirmPasswordText.color.g, confirmPasswordText.color.b, Mathf.Lerp(confirmPasswordText.color.a, 0f, fadeOutSpeed));
            confirmPasswordImage.color = new Color(confirmPasswordImage.color.r, confirmPasswordImage.color.g, confirmPasswordImage.color.b, Mathf.Lerp(confirmPasswordImage.color.a, 0f, fadeOutSpeed));
            yield return null;
        }
    }


	public void CheckPassword()
	{
        if(newAccount && !passwordObject.FindChild("InputField").GetComponent<InputField>().text.Equals(""))
        {
            validPassword = true;
            // Check password length
            if(passwordObject.FindChild("InputField").GetComponent<InputField>().text.Length < 6)
            {
                validPassword = false;
                messageObject.text = "Password must be at least 6 characters long.";
                messageObject.color = Color.red;
            }
            // Check if it contains a letter
            else if(!Regex.IsMatch(passwordObject.FindChild("InputField").GetComponent<InputField>().text, @"[a-zA-Z]"))
            {
                validPassword = false;
                messageObject.text = "Password must contain a letter.";
                messageObject.color = Color.red;
            }
            // Check if it contains a number
            else if(!Regex.IsMatch(passwordObject.FindChild("InputField").GetComponent<InputField>().text, @"[0-9]"))
            {
                validPassword = false;
                messageObject.text = "Password must contain a number.";
                messageObject.color = Color.red;
            }
            // Check if it contains a special character
            else if(Regex.IsMatch(passwordObject.FindChild("InputField").GetComponent<InputField>().text, "^[a-zA-Z0-9 ]*$"))
            {
                validPassword = false;
                messageObject.text = "Password must contain a special character.";
                messageObject.color = Color.red;
            }


            if(!validPassword)
            {
                // Turn password field red
                tempColorBlock = passwordObject.FindChild("InputField").GetComponent<InputField>().colors;
                tempColorBlock.normalColor = Color.red;
                passwordObject.FindChild("InputField").GetComponent<InputField>().colors = tempColorBlock;

            }
            else
            {
                // Turn password field green
                tempColorBlock = passwordObject.FindChild("InputField").GetComponent<InputField>().colors;
                tempColorBlock.normalColor = Color.green;
                passwordObject.FindChild("InputField").GetComponent<InputField>().colors = tempColorBlock;
            }
        }
	}

    public void CheckEqualPasswords()
    {
        if(passwordObject.FindChild("InputField").GetComponent<InputField>().text.Equals(confirmPasswordObject.FindChild("InputField").GetComponent<InputField>().text))
        {
            // Turn confirm password field green
            tempColorBlock = confirmPasswordObject.FindChild("InputField").GetComponent<InputField>().colors;
            tempColorBlock.normalColor = Color.green;
            confirmPasswordObject.FindChild("InputField").GetComponent<InputField>().colors = tempColorBlock;
        }
        else
        {       
            if(validPassword)
            {
                // Turn confirm password field red
                tempColorBlock = confirmPasswordObject.FindChild("InputField").GetComponent<InputField>().colors;
                tempColorBlock.normalColor = Color.red;
                confirmPasswordObject.FindChild("InputField").GetComponent<InputField>().colors = tempColorBlock;

                messageObject.text = "Passwords must match.";
                messageObject.color = Color.red;
            }
            validPassword = false;
        }
    }

	public void ClearPasswordError()
	{
		messageObject.text = "";
        // Turn password field white
        tempColorBlock = passwordObject.FindChild("InputField").GetComponent<InputField>().colors;
        tempColorBlock.normalColor = Color.white;
        passwordObject.FindChild("InputField").GetComponent<InputField>().colors = tempColorBlock;
        // Turn confirm password field white
        tempColorBlock = confirmPasswordObject.FindChild("InputField").GetComponent<InputField>().colors;
        tempColorBlock.normalColor = Color.white;
        confirmPasswordObject.FindChild("InputField").GetComponent<InputField>().colors = tempColorBlock;

	}


	public void CheckScreenName()
	{
		if(screenNameObject.FindChild("InputField").GetComponent<InputField>().text.Length < 3) 
		{
			messageObject.text = "Screen Name is too short.";
			messageObject.color = Color.red;
            tempColorBlock = screenNameObject.FindChild("InputField").GetComponent<InputField>().colors;
            tempColorBlock.normalColor = Color.red;
            screenNameObject.FindChild("InputField").GetComponent<InputField>().colors = tempColorBlock;
			validScreenName = false;
		} 
		else 
		{
			validScreenName = true;
		}

	}

	public void ClearScreenNameError()
	{
		messageObject.text = "";
        // Turn screen name field white
        tempColorBlock = screenNameObject.FindChild("InputField").GetComponent<InputField>().colors;
        tempColorBlock.normalColor = Color.white;
        screenNameObject.FindChild("InputField").GetComponent<InputField>().colors = tempColorBlock;
	}


    public void ClearEmailError()
    {
        messageObject.text = "";
        // Turn email field white
        tempColorBlock = emailObject.FindChild("InputField").GetComponent<InputField>().colors;
        tempColorBlock.normalColor = Color.white;
        emailObject.FindChild("InputField").GetComponent<InputField>().colors = tempColorBlock;
    }


    public void Login()
    {
		loginButton.SetActive(false);
		loginText.SetActive(true);
		lowerButtonsObject.FindChild("New").GetComponent<Button>().interactable = false;

        if(newAccount)
        {
            CheckEqualPasswords();
            if(validPassword && validScreenName)
            {
				Debug.Log("Registering new user with email: "+emailObject.FindChild("InputField").GetComponent<InputField>().text);

                RegisterPlayFabUserRequest request = new RegisterPlayFabUserRequest()
                {
					TitleId = AccountManager.TITLE_ID,
                    Username = screenNameObject.FindChild("InputField").GetComponent<InputField>().text,
					DisplayName = screenNameObject.FindChild("InputField").GetComponent<InputField>().text,
                    Email = emailObject.FindChild("InputField").GetComponent<InputField>().text,
                    Password = confirmPasswordObject.FindChild("InputField").GetComponent<InputField>().text
                };

                PlayFabClientAPI.RegisterPlayFabUser(request, (result) => {					
					responsePFID = result.PlayFabId;
					Debug.Log("Returned PlayFabId: "+responsePFID);
					AccountManager.SESSION_TICKET = result.SessionTicket;
					Debug.Log("Returned Session Ticket: "+ AccountManager.SESSION_TICKET);
					AccountManager.SCREEN_NAME = result.Username;
					Debug.Log("Returned Screen Name: "+ AccountManager.SCREEN_NAME);
					loginText.GetComponent<Text>().text = "New Account Created.";
                }, 
                (error) => {						
						Debug.LogError(error.ErrorMessage+" "+error.Error.GetHashCode());
						loginButton.SetActive(true);
						loginText.SetActive(false);
						lowerButtonsObject.FindChild("New").GetComponent<Button>().interactable = true;

						switch(error.Error.GetHashCode())
						{
						case 1000:
                            // Invalid inputs code, All other fields are validated except email 
                            messageObject.text = "Error: Invalid Email Address.";
							messageObject.color = Color.red;

                            tempColorBlock = emailObject.FindChild("InputField").GetComponent<InputField>().colors;
                            tempColorBlock.normalColor = Color.red;
                            emailObject.FindChild("InputField").GetComponent<InputField>().colors = tempColorBlock;
							break;
						case 1006:
							messageObject.text = "Error: An account with this Email Address has already been created.";
							messageObject.color = Color.red;

                            tempColorBlock = emailObject.FindChild("InputField").GetComponent<InputField>().colors;
                            tempColorBlock.normalColor = Color.red;
                            emailObject.FindChild("InputField").GetComponent<InputField>().colors = tempColorBlock;
							break;
						case 1005:
							messageObject.text = "Error: Invalid Email Address.";
							messageObject.color = Color.red;

                            tempColorBlock = emailObject.FindChild("InputField").GetComponent<InputField>().colors;
                            tempColorBlock.normalColor = Color.red;
                            emailObject.FindChild("InputField").GetComponent<InputField>().colors = tempColorBlock;
							break;
						case 1009:
							messageObject.text = "Error: An account with this Screen Name has already been created.";
							messageObject.color = Color.red;

                            tempColorBlock = screenNameObject.FindChild("InputField").GetComponent<InputField>().colors;
                            tempColorBlock.normalColor = Color.red;
                            screenNameObject.FindChild("InputField").GetComponent<InputField>().colors = tempColorBlock;
							break;
						case 1058:
							messageObject.text = "Error: An account with this Screen Name has already been created.";
							messageObject.color = Color.red;

                            tempColorBlock = screenNameObject.FindChild("InputField").GetComponent<InputField>().colors;
                            tempColorBlock.normalColor = Color.red;
                            screenNameObject.FindChild("InputField").GetComponent<InputField>().colors = tempColorBlock;
							break;
						case 1007:
							messageObject.text = "Error: Invalid Screen Name.";
							messageObject.color = Color.red;

                            tempColorBlock = screenNameObject.FindChild("InputField").GetComponent<InputField>().colors;
                            tempColorBlock.normalColor = Color.red;
                            screenNameObject.FindChild("InputField").GetComponent<InputField>().colors = tempColorBlock;
							break;
						case 1008:
							messageObject.text = "Error: Invalid Password.";
							messageObject.color = Color.red;
							break;
						}
                });
            }
            else
            {
				loginButton.SetActive(true);
				loginText.SetActive(false);
				lowerButtonsObject.FindChild("New").GetComponent<Button>().interactable = true;
            }
        }
        else
        {
			Debug.Log("Logging in with email: "+emailObject.FindChild("InputField").GetComponent<InputField>().text);

			LoginWithEmailAddressRequest request = new LoginWithEmailAddressRequest()
			{
				TitleId = AccountManager.TITLE_ID,
				Email = emailObject.FindChild("InputField").GetComponent<InputField>().text,
				Password = passwordObject.FindChild("InputField").GetComponent<InputField>().text
			};

			PlayFabClientAPI.LoginWithEmailAddress(request, (result) => {
				responsePFID = result.PlayFabId;
				Debug.Log("Returned PlayFabId: "+responsePFID);
				AccountManager.SESSION_TICKET = result.SessionTicket;
				Debug.Log("Returned Session Ticket: "+ AccountManager.SESSION_TICKET);
				loginText.GetComponent<Text>().text = "Login Successful.";
			},
			(error) => {
					Debug.LogError(error.ErrorMessage+" "+error.Error.GetHashCode());
					loginButton.SetActive(true);
					loginText.SetActive(false);
					lowerButtonsObject.FindChild("New").GetComponent<Button>().interactable = true;

					switch(error.Error.GetHashCode())
					{
					case 1000:
                        messageObject.text = "Error: Incorrect Email Address or Password.";
						messageObject.color = Color.red;
						break;
					case 1001:
						messageObject.text = "Error: No account with this email address exists.";
						messageObject.color = Color.red;

                        tempColorBlock = emailObject.FindChild("InputField").GetComponent<InputField>().colors;
                        tempColorBlock.normalColor = Color.red;
                        emailObject.FindChild("InputField").GetComponent<InputField>().colors = tempColorBlock;
						break;
					case 1002:
						messageObject.text = "Error: This account has been banned.";
						messageObject.color = Color.red;
						break;
					case 1142:
						messageObject.text = "Error: Incorrect Email Address or Password.";
						messageObject.color = Color.red;
						break;
					}
			});
        }
    }



}
