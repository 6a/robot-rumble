using System;
using System.Collections.Generic;

namespace RR.Models.DBModel
{
    public static class DBConstants
    {
        public static string USERNAME_ILLEGAL_CHARS_REGEX { get { return @"[^A-z0-9]"; } }
    }

    public struct User 
    {
        public string name;
        public string password;
    }

    public struct Update
    {
        public string name;
        public int wins;
        public int draws;
        public int losses;
    }

    public struct Response 
    {
        public int Code;
        public string Message;

        public override string ToString() { return $"Code: {this.Code}\nMessage: {this.Message}"; }
    }

    [System.Serializable]
    public struct LeaderboardRow
    {
        public int rank;
        public int outof;
        public string name;
        public int wins;
        public int draws;
        public int losses;
        public float ratio;
        public int played;
    }

    [System.Serializable]
    public struct Leaderboards
    {
        public LeaderboardRow user;
        public LeaderboardRow[] leaderboard;
    }

    public interface IDBModel : IModel
    {
        event Action<Response> CredentialValidationResult;
        event Action<Response> AccountCreationResult;
        event Action<Response> UpdateResult;
        event Action<Response> LeaderboardsResult;

        void ValidateCredentials(User user);
        void CreateAccount(User user);
        void AddWin(User user);
        void AddDraw(User user);
        void AddLoss(User user);
        void GetLeaderboards(User user);
    }
}

