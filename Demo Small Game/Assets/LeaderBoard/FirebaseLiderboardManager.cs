using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using TMPro;

public class FirebaseLiderboardManager : MonoBehaviour
{
    //1.Inilize tge Firebase Data Base Reference
    //2.Get total User Score from the Database
    //3.Check user Exist in it or not
    //4.Fetch User Proffile Data 
    //5.Fetch Leader board Data and show it in the UI
    //6.Display Lider Board Ui 
    //7.Add Ui Event sine In ,SineOut , Close Button 

    public GameObject userNAmePanal, userProfilPanal, leaderBoardPanal, leaderboardContent;
    public TMP_InputField userNameInput;
    public TMP_Text profileUserNameText, profileUserScoreText, errorUserNameText;
    public int Score = 0, totalUser = 0;
    public string UserName = "";

    private DatabaseReference dbReference;


    void Start()
    {
        //initinalize the database reference
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SineInWithUserName()
    {

    }

    public void CloseLeaderBoard()
    {

    }
    public void SineOut()
    {

    }
    void FirebaseInitialization()
    {
        dbReference = FirebaseDatabase.DefaultInstance.GetReference("/Leader Board/");
        dbReference.ChildAdded += HanadalChiledAdded;

        GetTotalUser();

        StartCoroutine(FetchaUserProfileData(PlayerPrefs.GetString("PlayerID")));
    }

    void HanadalChiledAdded(object sender, ChildChangedEventArgs args)
    {
        if(args.DatabaseError != null)
        {
            return;
        }

        GetTotalUser();
    }

    void GetTotalUser()
    {

    }

    IEnumerator CheckUserExistInDatabase()
    { 
        var task = dbReference.OrderByChild("PlayerID").EqualTo(PlayerPrefs.GetString("PlayerID")).GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.IsFaulted)
        {
            Debug.LogError("Error in fetching data from database");
        }

        else if (task.IsCompleted)
        {
            DataSnapshot snapshot = task.Result;
            if (snapshot != null && snapshot.HasChildren)
            {
               Debug.Log ("User Exists in Database");
            }
            else
            {
               Debug.Log("User Not Exists in Database");
            }
        }
    }

    IEnumerator FetchaUserProfileData(string PlayrerID)
    {
        yield return null;
    }

    IEnumerator FetchLeaderBoardData()
    {
        yield return null;
    }

    void DisplayLiderboardData()
    {

    }
}
