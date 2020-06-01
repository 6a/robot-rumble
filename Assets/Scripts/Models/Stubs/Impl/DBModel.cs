using System;
using System.Text.RegularExpressions;
using UnityEngine.Networking;
using UnityEngine;
using System.Collections;
using System.Text;
using RR.Utility.Gameplay;

namespace RR.Models.DBModel.Impl
{
    public class DBModel : Model, IDBModel
    {
        public event Action<Response> CredentialValidationResult = delegate { };
        public event Action<Response> AccountCreationResult = delegate { };
        public event Action<Response> UpdateResult = delegate { };
        public event Action<Response> LeaderboardsResult = delegate { };
        
        private const string API_URL = "https://i072trjf57.execute-api.eu-west-2.amazonaws.com/default/rr-api";

        private CoroutineOwner _co;

        public void Initialize()
        {
            var _coroutineOwnerGameObject = new GameObject("DBModel_CoroutineOwner");
            _coroutineOwnerGameObject.AddComponent<CoroutineOwner>();
            _co = _coroutineOwnerGameObject.GetComponent<CoroutineOwner>();
        }

        public void ValidateCredentials(User user)
        {
            var auth = MakeAuthheader(user);
            _co.StartCoroutine(CallAPI("GET", null, auth, CredentialValidationResult));
        }

        public void CreateAccount(User user)
        {
            _co.StartCoroutine(CallAPI("PUT", user, null, AccountCreationResult));
        }

        public void AddWin(User user)
        {
            var auth = MakeAuthheader(user);
            var updateBody = new Update();
            updateBody.name = user.name;
            updateBody.wins = 1;

            _co.StartCoroutine(CallAPI("PATCH", updateBody, auth, UpdateResult));
        }

        public void AddDraw(User user)
        {
            var auth = MakeAuthheader(user);
            var updateBody = new Update();
            updateBody.name = user.name;
            updateBody.draws = 1;

            _co.StartCoroutine(CallAPI("PATCH", updateBody, auth, UpdateResult));
        }

        public void AddLoss(User user)
        {
            var auth = MakeAuthheader(user);
            var updateBody = new Update();
            updateBody.name = user.name;
            updateBody.losses = 1;

            _co.StartCoroutine(CallAPI("PATCH", updateBody, auth, UpdateResult));
        }

        public void GetLeaderboards(User user)
        {
            user.password = "";

            _co.StartCoroutine(CallAPI("POST", user, null, LeaderboardsResult));
        }

        private string SanitizeUsername(string input)
        {
            return Regex.Replace(input, DBConstants.USERNAME_ILLEGAL_CHARS_REGEX, "_");
        }

        private IEnumerator CallAPI(string method, object body, string b64Credentials, Action<Response> callback) 
        {
            var request = new UnityWebRequest(API_URL, method);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

            if (body != null)
            {
                var jsonBody = JsonUtility.ToJson(body);
                byte[] rawBody = Encoding.UTF8.GetBytes(jsonBody);
                request.uploadHandler = (UploadHandler)new UploadHandlerRaw(rawBody);
                request.SetRequestHeader("Content-Type", "application/json");
            }

            if (b64Credentials != null)
            {
                request.SetRequestHeader("Authorization", $"Basic {b64Credentials}");
            }

            yield return request.SendWebRequest();

            callback(new Response { Code = (int)request.responseCode, Message = request.downloadHandler.text });
        }

        private string MakeAuthheader(User user)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user.name}:{user.password}"));
        }
    }
}