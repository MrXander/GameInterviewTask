using System;
using System.Collections.Generic;

namespace GameWebApplication.Models
{
    public class Scores
    {
        public Scores()
        {
            ScoreList = new List<Score>();
        }

        public TimeSpan Duration { get; set; }
        public List<Score> ScoreList { get; set; }
    }
}