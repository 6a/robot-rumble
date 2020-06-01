using System;

namespace RR.Models.ScoresModel.Impl
{
    public class ScoreModel : Model, IScoreModel
    {
        public event Action ScoreUpdated = delegate { };
        
        public int CurrentScore { get; private set; }

        public void Initialize()
        {
            CurrentScore = 0;
        }
        
        public void IncreaseScore(int scoreValue)
        {
            CurrentScore += scoreValue;

            ScoreUpdated();
        }
        
        public void Reset()
        {
            CurrentScore = 0;
        }
    }
}
