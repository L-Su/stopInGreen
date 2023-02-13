using System;
using System.Collections.Generic;
using System.Text;

namespace stopInGreen
{
    public class HighScores
    {
        public HighScores()
        {
            
        }

        public HighScores(int num1, int num2, int num3, int num4, int num5)
        {
            this.Num1 = num1;
            this.Num2 = num2;
            this.Num3 = num3;
            this.Num4 = num4;
            this.Num5 = num5;
        }

        public int Num1 { get; set; }

        public int Num2 { get; set; }

        public int Num3 { get; set; }

        public int Num4 { get; set; }

        public int Num5 { get; set; }
    }
}
