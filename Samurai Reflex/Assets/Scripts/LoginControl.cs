using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoginControl : MonoBehaviour
{
    public Transform emailObject;
    public Transform screenNameObject;
    public Transform passwordObject;
    public Transform confirmPasswordObject;
    public Transform lowerButtonsObject;
    public Transform passwordTarget;
    public Transform lowerButtonsTarget;

    public float expandSpeed = 0.1f;
    public float fadeInSpeed = 0.05f;
    public float fadeOutSpeed = 0.1f;

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
	}
	
	// Update is called once per frame
	void Update() 
	{
	
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


    public void ForgotPasswordPage()
    {
        Application.OpenURL("http://www.alltradesgames.com");
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


    public void Login()
    {
        if (newAccount)
        {

        }
        else
        {

        }
    }


}
