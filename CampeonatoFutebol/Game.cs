using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampeonatoFutebol
{
    internal class Game
    {
        int Code;
        int HouseTeam;
        int VisitTeam;
        int HouseGoals;
        int VisitGoals;
        int TotalGoals;

        public Game(int code,int houseTeam, int visitTeam,int houseGoals, int visitGoals)
        {
            Code = code;
            HouseTeam = houseTeam;
            VisitTeam = visitTeam;
            HouseGoals = houseGoals;
            VisitGoals = visitGoals;
            TotalGoals = visitGoals + houseGoals;
        }
        
        public int GetHouseTeam()
        {
            return HouseTeam;
        }
        public int GetVisitTeam()
        {
            return VisitTeam;
        }
    }
}
