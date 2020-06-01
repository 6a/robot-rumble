using System;

namespace RR.Models.ScoresModel
{
    public interface IScoreModel : IModel
    {
        event Action ScoreUpdated;
        int CurrentScore { get; }
        void IncreaseScore(int scoreValue);
        void Reset();
    }
}

