using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonteMonolith.Framework
{
    //sourced from: https://www.codeproject.com/Articles/32654/Monte-Carlo-Simulation
    public class MonteCarlo
    {
        public static double triangular(double Min, double Mode, double Max)
        {
            //   Declarations        
            double R = 0.0;
            //   Initialise     
            Random r = new Random();
            R = r.NextDouble();
            //    Triangular                        
            if (R == ((Mode - Min) / (Max - Min)))
            {
                return Mode;
            }
            else if (R < ((Mode - Min) / (Max - Min)))
            {
                return Min + Math.Sqrt(R * (Max - Min) * (Mode - Min));
            }
            else
            {
                return Max - Math.Sqrt((1 - R) * (Max - Min) * (Max - Mode));
            }
        }
        public static double[] simulate(int Total, double[] Tmin, double[] Tmod, double[] Tmax)
        {
            // Declarations
            int mlngEvals = 10000;
            int i = 0, i1 = 0, i2 = 0;
            double[] TMin = new double[Total];
            double[] TMod = new double[Total];
            double[] TMax = new double[Total];
            double[] mlngResults = new double[Total];
            double Time = 0.0;
            long lngWinner = 0;
            double Winner = 0;
            // Initialise            
            for (i = 0; i < Total; i++)
            {
                //     distribution parameters
                TMin[i] = Tmin[i];
                TMod[i] = Tmod[i];
                TMax[i] = Tmax[i];
                //     Results Array              
                mlngResults[i] = 0;
            }
            // The Tournament           
            for (i1 = 1; i1 <= mlngEvals; i1++)
            {
                //     Seed               
                lngWinner = 0;
                Winner = triangular(TMin[0], TMod[0], TMax[0]);
                //     And the Rest
                for (i2 = 1; i2 < Total; i2++)
                {
                    Time = triangular(TMin[i2], TMod[i2], TMax[i2]);
                    if (Time < Winner)
                    {
                        Winner = Time;
                        lngWinner = i2;
                    }
                }
                //     Bin
                mlngResults[lngWinner]++;
            }
            return mlngResults;
        }
    }
}
