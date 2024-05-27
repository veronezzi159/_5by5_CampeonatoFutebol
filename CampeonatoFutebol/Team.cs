using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CampeonatoFutebol
{
    internal class Team
    {
        int Id;
        string Name;
        string NickName;
        DateOnly CreationDate;
        int Punctuation;
        int TotalGoalsScored;
        int TotalGoalsConceded;

        public Team(string n, string nick, DateOnly dt, int id)
        {
            Name = n; NickName = nick; CreationDate = dt;
            Id = id;
        }

        public void PrintSimple()
        {
            Console.WriteLine($"Id:{Id}");
            Console.WriteLine($"Nome do Time: {Name}");
            Console.WriteLine($"Apelido do Time: {NickName}");
            Console.WriteLine($"Data de Criação: {CreationDate}");
        }

        public int GetId()
        {
            return Id;
        }
            
    }
}
