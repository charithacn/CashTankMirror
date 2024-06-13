using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Google;
using System;
using Firebase.Auth;
using Firebase;
using Firebase.Database;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

public class SignInManager : MonoBehaviour
{
    public Button signInButton;
    public Button logInButton;
    public Button logoutButton;

    public Button depositInstructionButton;
    public Button depositWithdrawButton;

    public InputField passwordTextField;
    public InputField passwordVerifiedTextField;
    public InputField emailTextField;

    public Text statusText;

    public GameObject logInObj;
    public string userId;

    string email;
    string password;

    FirebaseAuth auth;
    public FirebaseUser user;

    DependencyStatus dependencyStatus;
    //Firebase.Auth.FirebaseAuth auth;
    // Start is called before the first frame update
    public Text refText;
    public Text playerUserId;
    public Text playerUserIdWithdrawal;
    public InputField referrelInput;
    public Button referrelSubmitButton;
    public Text coinText;
    void Awake()
    {
        //Check that all of the necessary dependencies for Firebase are present on the system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                //If they are avalible Initialize Firebase
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }
    private void InitializeFirebase()
    {
        
        //Set the authentication instance object
        auth = FirebaseAuth.DefaultInstance;
        Debug.Log("Setting up Firebase Auth");
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
        logInObj.SetActive(false);
        SetSignIn();
        if (!PlayerPrefs.HasKey("Coins" + PlayerPrefs.GetInt("FirebaseUser")))
        {
            PlayerPrefs.SetInt("Coins" + PlayerPrefs.GetInt("FirebaseUser"), 1000);
            PlayerPrefs.Save();
        }
        coinText.text = PlayerPrefs.GetString("Coins" + PlayerPrefs.GetInt("FirebaseUser"));
    }
    void GenerateRef()
    {
        string lastFourCharacters = userId.Substring(Mathf.Max(0, userId.Length - 4));
    }
    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null
                && auth.CurrentUser.IsValid();
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            
            
            if (signedIn)
            {
                SetObjectActive();
                userId = user.UserId;
                Debug.Log("Signed in " + user.UserId);
                SendMessage("SetObjectActive");
                LoadDatabase(user);
                SetSignIn();
                // SendMessage("SetSignIn", false);
            }
        }
        userId = auth.CurrentUser.UserId;
        //LoadDatabase(auth.CurrentUser);
    }
    void SetSignIn()
    {
        PlayerPrefs.SetString("FirebaseUser", auth.CurrentUser.UserId);
        PlayerPrefs.Save();
    }
    public void SetUserId()
    {
        playerUserId.text ="Player User Id : "+ userId;
        playerUserIdWithdrawal.text = "Player User Id : " + userId;
    }
    void Start()
    {
        logoutButton.onClick.AddListener(LogoutAction);
        depositInstructionButton.onClick.AddListener(DepositIntoAction);
        depositWithdrawButton.onClick.AddListener(DepositWithdrawAction);
        referrelSubmitButton.onClick.AddListener(AddRefKey);
        signInButton.onClick.AddListener(SignInAction);
        logInButton.onClick.AddListener(LogIn);
        // Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        if (PlayerPrefs.GetString("FirebaseUser") !=null)
        {
           logInObj.SetActive(false);
        } else
        {
           logInObj.SetActive(true);
        }

    }
    void SetObjectActive()
    {
        logInObj.SetActive(false);
    }
    private void DepositIntoAction()
    {
        SendEmail("Deposit Intro ", userId);
    }

    private void DepositWithdrawAction()
    {
        SendEmail("DepositWithdraw", userId);
    }

    private void LogoutAction()
    {
        auth.StateChanged -= AuthStateChanged;
        auth = null;
       logInObj.SetActive(true);
        auth.SignOut();
        PlayerPrefs.SetString("FirebaseUser", "");
        PlayerPrefs.Save();
    }
    

    private void SignInAction()
    {
        email = emailTextField.text;
        password = passwordTextField.text;

        if(passwordTextField.text.Equals(password))
        {
            RegisterButton();
        } else
        {
            statusText.text = "Password not Matched!";
        }
        
    }

    public void RegisterButton()
    {
        //Call the register coroutine passing the email, password, and username
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                statusText.text = "Cancelled";

                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                statusText.text = "Create Account";
                return;
            }

            // Firebase user has been created.
            Firebase.Auth.AuthResult result = task.Result;
            
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);
            statusText.text = "User created successfully!";

        });
    }
    public void LogIn()
    {
        email = emailTextField.text;
        password = passwordTextField.text;
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);  
                return;
            }
            
            Firebase.Auth.AuthResult result = task.Result;
            logInObj.SetActive(false);
            SetSignIn();
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);
            
        });
        
    }


    void Update()
    {
        
    }
    void LoadDatabase (FirebaseUser users)
    {

        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        string refChar = userId.Substring(Mathf.Max(0, userId.Length - 4));
        User user = new User(users.Email, users.PhoneNumber , refChar);
        string json = JsonUtility.ToJson(user);
        reference.Child(users.UserId).SetRawJsonValueAsync(json);

        refText.text = refChar;
        
        print(reference);

    }
    void AddRefKey()
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        reference.Child(PlayerPrefs.GetString("FirebaseUser")).Child("Referrel").SetRawJsonValueAsync(referrelInput.text);
    }
    void SendEmail(string subject ,string body)
    {
        MailMessage mail = new MailMessage();

        mail.From = new MailAddress("yasithsaminthaka@gmail.com");
        mail.To.Add("autoplaybox@gmail.com");
        mail.Subject = subject;
        mail.Body = body;

        SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
        smtpServer.Port = 587;
        smtpServer.EnableSsl = true;
        smtpServer.Credentials = new System.Net.NetworkCredential("yasithsaminthaka@gmail.com", "vave arox dbts rbfn") as ICredentialsByHost;

        ServicePointManager.ServerCertificateValidationCallback =
            delegate (object obj, X509Certificate cert, X509Chain chain, SslPolicyErrors sslerrors)
            { return true; };

        smtpServer.Send(mail);

#if UNITY_EDITOR
        Debug.Log("email sent");
#endif

    }
}
[System.Serializable]
public class User
{
    public string username;
    public string email;
    public string refNumber;
    public User()
    {
    }

    public User(string username, string email , string refNumber)
    {
        this.username = username;
        this.email = email;
        this.refNumber = refNumber;
    }
}
