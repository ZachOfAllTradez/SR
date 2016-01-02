using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
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
	public Text messageObject;
    public Toggle emailToggle;
    public Text validationText;
    public Image validationImage;
    public Image validationButton;
    public Image emailToggleImage;
    public Text emailToggleLabel;
    public Text forgotText;
    public Text newAcctText;

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

	public int validationKeyLength = 10;
	private string charSet = "123456789ABCDEFGHJKLMNOPQRSTUVWXYZ";
	private string validationKey = "";


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
        loginButton.GetComponent<Button>().interactable = true;
		messageObject.text = "";

        if (PlayerPrefs.HasKey("RememberEmail"))
        {
            if (PlayerPrefs.GetInt("RememberEmail") == 1)
            {
                if (PlayerPrefs.HasKey("Email"))
                {
                    //Debug.Log("Loaded email: " + PlayerPrefs.GetString("Email"));
                    emailObject.FindChild("InputField").GetComponent<InputField>().text = PlayerPrefs.GetString("Email");
                }
                emailToggle.isOn = true;
            }
            else
            {
                emailToggle.isOn = false;
            }
        }
        else
        {
            PlayerPrefs.SetInt("RememberEmail", 0);
            emailToggle.isOn = false;
        }
	}
	
	// Update is called once per frame
	void Update() 
	{
		if(Input.GetButtonDown("Submit"))
		{
			Login();
		}
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // Tab through menu fields
            Selectable next = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
            if (next != null)
            {
                InputField nextField = next.GetComponent<InputField>();
                if (nextField != null)
                {
                    nextField.OnPointerClick(new PointerEventData(EventSystem.current));
                }
                EventSystem.current.SetSelectedGameObject(next.gameObject, new BaseEventData(EventSystem.current));
            }
        }
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


    IEnumerator HideAll()
    {
        lowerButtonsObject.FindChild("Login Button").gameObject.SetActive(false);
        while((emailObject.GetComponent<Text>().color.a>0.001f) || (emailObject.FindChild("InputField").GetComponent<Image>().color.a>0.001f))
        {
            lowerButtonsObject.FindChild("New").GetComponent<Button>().interactable = false;
            lowerButtonsObject.FindChild("Forgot").GetComponent<Button>().interactable = false;
            lowerButtonsObject.FindChild("Login Button").GetComponent<Button>().interactable = false;
            emailObject.FindChild("InputField").GetComponent<InputField>().interactable = false;
            emailObject.FindChild("Toggle").GetComponent<Toggle>().interactable = false;
            passwordObject.FindChild("InputField").GetComponent<InputField>().interactable = false;

            emailObject.FindChild("InputField").GetComponent<InputField>().text = "";
            screenNameObject.FindChild("InputField").GetComponent<InputField>().text = "";
            passwordObject.FindChild("InputField").GetComponent<InputField>().text = "";
            confirmPasswordObject.FindChild("InputField").GetComponent<InputField>().text = "";

            emailObject.GetComponent<Text>().color = new Color(emailObject.GetComponent<Text>().color.r, emailObject.GetComponent<Text>().color.g, emailObject.GetComponent<Text>().color.b, Mathf.Lerp(emailObject.GetComponent<Text>().color.a, 0f, fadeOutSpeed));
            emailObject.FindChild("InputField").GetComponent<Image>().color = new Color(emailObject.FindChild("InputField").GetComponent<Image>().color.r, emailObject.FindChild("InputField").GetComponent<Image>().color.g, emailObject.FindChild("InputField").GetComponent<Image>().color.b, Mathf.Lerp(emailObject.FindChild("InputField").GetComponent<Image>().color.a, 0f, fadeOutSpeed));
            passwordObject.GetComponent<Text>().color = new Color(passwordObject.GetComponent<Text>().color.r, passwordObject.GetComponent<Text>().color.g, passwordObject.GetComponent<Text>().color.b, Mathf.Lerp(passwordObject.GetComponent<Text>().color.a, 0f, fadeOutSpeed));
            passwordObject.FindChild("InputField").GetComponent<Image>().color = new Color(passwordObject.FindChild("InputField").GetComponent<Image>().color.r, passwordObject.FindChild("InputField").GetComponent<Image>().color.g, passwordObject.FindChild("InputField").GetComponent<Image>().color.b, Mathf.Lerp(passwordObject.FindChild("InputField").GetComponent<Image>().color.a, 0f, fadeOutSpeed));
            emailToggleImage.color = new Color(emailToggleImage.color.r, emailToggleImage.color.g, emailToggleImage.color.b, Mathf.Lerp(emailToggleImage.color.a, 0f, fadeOutSpeed));
            emailToggleLabel.color = new Color(emailToggleLabel.color.r, emailToggleLabel.color.g, emailToggleLabel.color.b, Mathf.Lerp(emailToggleLabel.color.a, 0f, fadeOutSpeed));
            forgotText.color = new Color(forgotText.color.r, forgotText.color.g, forgotText.color.b, Mathf.Lerp(forgotText.color.a, 0f, fadeOutSpeed));
            newAcctText.color = new Color(newAcctText.color.r, newAcctText.color.g, newAcctText.color.b, Mathf.Lerp(newAcctText.color.a, 0f, fadeOutSpeed));

            screenNameText.color = new Color(screenNameText.color.r, screenNameText.color.g, screenNameText.color.b, Mathf.Lerp(screenNameText.color.a, 0f, fadeOutSpeed));
            screenNameImage.color = new Color(screenNameImage.color.r, screenNameImage.color.g, screenNameImage.color.b, Mathf.Lerp(screenNameImage.color.a, 0f, fadeOutSpeed));
            confirmPasswordText.color = new Color(confirmPasswordText.color.r, confirmPasswordText.color.g, confirmPasswordText.color.b, Mathf.Lerp(confirmPasswordText.color.a, 0f, fadeOutSpeed));
            confirmPasswordImage.color = new Color(confirmPasswordImage.color.r, confirmPasswordImage.color.g, confirmPasswordImage.color.b, Mathf.Lerp(confirmPasswordImage.color.a, 0f, fadeOutSpeed));
            yield return null;
        }
    }


    IEnumerator HideAllShowKey()
    {
        if (newAccount)
        {
            StartCoroutine("CollapseForm");
        }
        while((emailObject.GetComponent<Text>().color.a>0.001f) || (emailObject.FindChild("InputField").GetComponent<Image>().color.a>0.001f))
        {
            lowerButtonsObject.FindChild("New").GetComponent<Button>().interactable = false;
            lowerButtonsObject.FindChild("Forgot").GetComponent<Button>().interactable = false;
            lowerButtonsObject.FindChild("Login Button").GetComponent<Button>().interactable = false;
            emailObject.FindChild("InputField").GetComponent<InputField>().interactable = false;
            emailObject.FindChild("Toggle").GetComponent<Toggle>().interactable = false;
            passwordObject.FindChild("InputField").GetComponent<InputField>().interactable = false;

            emailObject.FindChild("InputField").GetComponent<InputField>().text = "";
            screenNameObject.FindChild("InputField").GetComponent<InputField>().text = "";
            passwordObject.FindChild("InputField").GetComponent<InputField>().text = "";
            confirmPasswordObject.FindChild("InputField").GetComponent<InputField>().text = "";

            emailObject.GetComponent<Text>().color = new Color(emailObject.GetComponent<Text>().color.r, emailObject.GetComponent<Text>().color.g, emailObject.GetComponent<Text>().color.b, Mathf.Lerp(emailObject.GetComponent<Text>().color.a, 0f, fadeOutSpeed));
            emailObject.FindChild("InputField").GetComponent<Image>().color = new Color(emailObject.FindChild("InputField").GetComponent<Image>().color.r, emailObject.FindChild("InputField").GetComponent<Image>().color.g, emailObject.FindChild("InputField").GetComponent<Image>().color.b, Mathf.Lerp(emailObject.FindChild("InputField").GetComponent<Image>().color.a, 0f, fadeOutSpeed));
            passwordObject.GetComponent<Text>().color = new Color(passwordObject.GetComponent<Text>().color.r, passwordObject.GetComponent<Text>().color.g, passwordObject.GetComponent<Text>().color.b, Mathf.Lerp(passwordObject.GetComponent<Text>().color.a, 0f, fadeOutSpeed));
            passwordObject.FindChild("InputField").GetComponent<Image>().color = new Color(passwordObject.FindChild("InputField").GetComponent<Image>().color.r, passwordObject.FindChild("InputField").GetComponent<Image>().color.g, passwordObject.FindChild("InputField").GetComponent<Image>().color.b, Mathf.Lerp(passwordObject.FindChild("InputField").GetComponent<Image>().color.a, 0f, fadeOutSpeed));
            emailToggleImage.color = new Color(emailToggleImage.color.r, emailToggleImage.color.g, emailToggleImage.color.b, Mathf.Lerp(emailToggleImage.color.a, 0f, fadeOutSpeed));
            emailToggleLabel.color = new Color(emailToggleLabel.color.r, emailToggleLabel.color.g, emailToggleLabel.color.b, Mathf.Lerp(emailToggleLabel.color.a, 0f, fadeOutSpeed));
            forgotText.color = new Color(forgotText.color.r, forgotText.color.g, forgotText.color.b, Mathf.Lerp(forgotText.color.a, 0f, fadeOutSpeed));
            newAcctText.color = new Color(newAcctText.color.r, newAcctText.color.g, newAcctText.color.b, Mathf.Lerp(newAcctText.color.a, 0f, fadeOutSpeed));

            screenNameText.color = new Color(screenNameText.color.r, screenNameText.color.g, screenNameText.color.b, Mathf.Lerp(screenNameText.color.a, 0f, fadeOutSpeed));
            screenNameImage.color = new Color(screenNameImage.color.r, screenNameImage.color.g, screenNameImage.color.b, Mathf.Lerp(screenNameImage.color.a, 0f, fadeOutSpeed));
            confirmPasswordText.color = new Color(confirmPasswordText.color.r, confirmPasswordText.color.g, confirmPasswordText.color.b, Mathf.Lerp(confirmPasswordText.color.a, 0f, fadeOutSpeed));
            confirmPasswordImage.color = new Color(confirmPasswordImage.color.r, confirmPasswordImage.color.g, confirmPasswordImage.color.b, Mathf.Lerp(confirmPasswordImage.color.a, 0f, fadeOutSpeed));
            yield return null;
        }
        lowerButtonsObject.FindChild("Login Button").gameObject.SetActive(false);
        validationText.transform.SetAsLastSibling();
        StartCoroutine("ShowEmailKeyInput");
    }


    IEnumerator ShowEmailKeyInput()
    {
        validationImage.gameObject.GetComponent<InputField>().interactable = true;
        while(((1f - validationText.color.a)>0.001f) || ((1f - validationImage.color.a)>0.001f))
        {
            validationText.color = new Color(validationText.color.r, validationText.color.g, validationText.color.b, Mathf.Lerp(validationText.color.a, 1f, fadeInSpeed));
            validationImage.color = new Color(validationImage.color.r, validationImage.color.g, validationImage.color.b, Mathf.Lerp(validationImage.color.a, 1f, fadeInSpeed));
            validationButton.color = new Color(validationButton.color.r, validationButton.color.g, validationButton.color.b, Mathf.Lerp(validationButton.color.a, 1f, fadeInSpeed));
            yield return null;
        }
        messageObject.text = "A verification email with a One-Time Key has been sent. Paste the Key into the field above and click 'Verify'.";
        messageObject.color = Color.green;
    }


    IEnumerator HideEmailKeyInput()
    {
        validationImage.gameObject.GetComponent<InputField>().text = "";
        validationImage.gameObject.GetComponent<InputField>().interactable = false;
        while((validationText.color.a>0.001f) || (validationImage.color.a>0.001f))
        {
            validationText.color = new Color(validationText.color.r, validationText.color.g, validationText.color.b, Mathf.Lerp(validationText.color.a, 0f, fadeInSpeed/2f));
            validationImage.color = new Color(validationImage.color.r, validationImage.color.g, validationImage.color.b, Mathf.Lerp(validationImage.color.a, 0f, fadeInSpeed/2f));
            validationButton.color = new Color(validationButton.color.r, validationButton.color.g, validationButton.color.b, Mathf.Lerp(validationButton.color.a, 0f, fadeInSpeed/2f));
            yield return null;
        }
        messageObject.text = "";
    }


    void SendVerifyEmail()
    {
        validationKey = "";
        Random.seed = (int)System.DateTime.UtcNow.Ticks;
        //Debug.Log("seed = "+Random.seed);
        for (int ii = 0; ii < validationKeyLength; ii++)
        {
            validationKey += charSet[Random.Range(0, charSet.Length)];            
        }
        //Debug.Log("validationKey = " + validationKey);

        MailMessage mail = new MailMessage ();
        mail.From = new MailAddress("alltradesgames@gmail.com");
        mail.To.Add(emailObject.FindChild("InputField").GetComponent<InputField>().text);
        mail.Subject = "AllTrades Games Email Validation Code";
        mail.Body = "Your one-time validation Key is : </br></br> " + validationKey +"</br></br> Copy this Key and paste it into the 'One-Time Key' field in your game.";
        mail.IsBodyHtml = true;

        SmtpClient smtpServer = new SmtpClient ("smtp.gmail.com");
        smtpServer.Port = 587;
        smtpServer.Credentials = new NetworkCredential ("alltradesgames@gmail.com", "Zondex3102010") as ICredentialsByHost;
        smtpServer.EnableSsl = true;
        ServicePointManager.ServerCertificateValidationCallback = delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
            return true;
        };
        smtpServer.Send (mail);
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
                case 1123:
                    messageObject.text = "Error: Cannot connect to server.";
                    messageObject.color = Color.red;
                    break;
            }
        });
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


    public void CheckToggle()
    {
        if (emailToggle.isOn)
        {
            //Debug.Log("Toggle on");
            PlayerPrefs.SetInt("RememberEmail", 1);
            //Debug.Log("set email as " + emailObject.FindChild("InputField").GetComponent<InputField>().text);
            PlayerPrefs.SetString("Email", emailObject.FindChild("InputField").GetComponent<InputField>().text);
        }
        else
        {
            //Debug.Log("Toggle off");
            PlayerPrefs.SetInt("RememberEmail", 0);
        }
    }


    public void Login()
    {
        loginButton.GetComponent<Button>().interactable = false;
		lowerButtonsObject.FindChild("New").GetComponent<Button>().interactable = false;
        // Save email in playerprefs 
        if (emailToggle.isOn)
        {
            PlayerPrefs.SetString("Email", emailObject.FindChild("InputField").GetComponent<InputField>().text);
        }

        if(newAccount)
        {
            CheckEqualPasswords();
            if(validPassword && validScreenName)
            {
                messageObject.text = "Connecting . . .";
                messageObject.color = Color.white;
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

                    // New account created
                    messageObject.text = "New Account Created.";
                    messageObject.color = Color.white;
                    AccountManager.InitializePlayerData();
                }, 
                (error) => {						
						Debug.LogError(error.ErrorMessage+" "+error.Error.GetHashCode());
                        loginButton.GetComponent<Button>().interactable = true;
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
                        case 1123:
                            messageObject.text = "Error: Cannot connect to server.";
                            messageObject.color = Color.red;
                            break;
						}
                });
            }
            else
            {
                loginButton.GetComponent<Button>().interactable = true;
				lowerButtonsObject.FindChild("New").GetComponent<Button>().interactable = true;
            }
        }
        else
        {
            messageObject.text = "Connecting . . .";
            messageObject.color = Color.white;
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

                // Login successful
                messageObject.text = "Login Successful.";
                messageObject.color = Color.white;
                AccountManager.DownloadPlayerData();
			},
			(error) => {
					Debug.LogError(error.ErrorMessage+" "+error.Error.GetHashCode());
                    loginButton.GetComponent<Button>().interactable = true;
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
                    case 1123:
                        messageObject.text = "Error: Cannot connect to server.";
                        messageObject.color = Color.red;
                        break;
					}
			});
        }
    }


    public void OnLoginSuccessful()
    {
        // Send verify email address if neccessary
        if (!AccountManager.EMAIL_VERIFIED)
        {
            SendVerifyEmail();
            StartCoroutine("HideAllShowKey");
        }
        else
        {
            // Start Game
            //TODO
            Debug.Log("Start Game");
            StartCoroutine("HideAll");
        }
    }


    public void SubmitEmailKey()
    {
        // Compare key input text to stored key
        if (validationImage.gameObject.GetComponent<InputField>().text.ToUpper().Equals(validationKey))
        {            
            AccountManager.EMAIL_VERIFIED = true;
            List<string> list = new List<string>();
            list.Add("emailVerified");
            AccountManager.UploadPlayerData(list);

            messageObject.text = "Validation successful.";
            messageObject.color = Color.green;

            // Start Game
            //TODO
            Debug.Log("Start Game");
            StartCoroutine("HideEmailKeyInput");
        }
        else
        {
            messageObject.text = "Incorrect validation Key. Please try again.";
            messageObject.color = Color.red;
        }
    }


}
