using Microsoft.Data.SqlClient;
using System;
using System.Data;
namespace CampeonatoFutebol
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List <Team> teams = new List <Team> (); 
            List <Game> games = new List <Game> (); 

            int Menu()
            {
                Console.WriteLine("Digite sua Opção:");
                Console.WriteLine("Opção 1: Cadastrar Times");
                Console.WriteLine("Opção 2: Inserir Jogos");
                Console.WriteLine("Opção 3: Ranking dos Times");
                Console.WriteLine("Opção 4: Time que mais fez gols");
                Console.WriteLine("Opção 5: Time que mais sofreu gols");
                Console.WriteLine("Opção 6: Jogo que mais teve gols");
                Console.WriteLine("Opção 7: Número de gols que cada time fez em um único jogo");
                Console.WriteLine("Opção 8: Exibir Campeão");
                Console.WriteLine("Opção 9: Reiniciar campeonato com os mesmos times");
                Console.WriteLine("Opção 10: Reiniciar campeonato com times diferentes");
                Console.WriteLine("Opção 0: Sair");
                int op =-1;
                do
                {
                    try
                    {
                        op = int.Parse(Console.ReadLine());
                        if (op < 0 || op > 10)
                        {
                            Console.WriteLine("Digite uma opção valida!");
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Digite um valor valido");
                    }
                } while (op < 0 || op> 10);
                
                return op;
            }
            
            void VerifyTeams()
            {
                DB conn = new DB();

                SqlConnection conectionSql = new SqlConnection(conn.Path());

                conectionSql.Open();

                SqlCommand cmd = new SqlCommand("SELECT Id, Nome, Apelido,DataCriacao FROM Equipe", conectionSql);
                

                cmd.Connection = conectionSql;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        int id = int.Parse(reader["Id"].ToString());
                        string nome = reader["Nome"].ToString();
                        string nick = reader["Apelido"].ToString();
                        DateOnly data = DateOnly.Parse(reader["DataCriacao"].ToString().Substring(0,10));

                        teams.Add(new(nome,nick,data,id));
                        
                    }
                }

                conectionSql.Close();
            }

            void VerifyGames()
            {
                DB conn = new DB();

                SqlConnection conectionSql = new SqlConnection(conn.Path());

                conectionSql.Open();

                SqlCommand cmd = new SqlCommand("SELECT TimeDaCasa, TimeVisitante, Codigo, GolsDaCasa, GolsVisi  FROM Jogo", conectionSql);


                cmd.Connection = conectionSql;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        int TimeDaCasa = int.Parse(reader["TimeDaCasa"].ToString());
                        int TimeVisitante = int.Parse(reader["TimeVisitante"].ToString());
                        int code = int.Parse(reader["Codigo"].ToString());
                        int GolsDaCasa = int.Parse(reader["GolsDaCasa"].ToString());
                        int GolsVisi = int.Parse(reader["GolsVisi"].ToString());       

                        games.Add(new(code,TimeDaCasa,TimeVisitante,GolsDaCasa,GolsVisi));

                    }
                }

                conectionSql.Close();
            }

            Team CreateTeam()
            {
                //conectar no Banco
                DB conn = new DB();

                SqlConnection conectionSql = new SqlConnection(conn.Path());

                conectionSql.Open();
                //Inserir Time

                string Nome, Apelido =null;
                DateOnly dt;

                SqlCommand cmd = new SqlCommand("[dbo].[InserirEquipe]", conectionSql);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                SqlParameter name = new SqlParameter("@Nome",System.Data.SqlDbType.VarChar,50);
                SqlParameter nick = new SqlParameter("@apelido",System.Data.SqlDbType.VarChar,30);
                SqlParameter creationDate = new SqlParameter("@dataCriacao", System.Data.SqlDbType.Date);

                Console.WriteLine("Digite o nome do time:");
                Nome = Console.ReadLine();
                Console.WriteLine("Digite o Apelido do time");
                Apelido = Console.ReadLine();
                Console.WriteLine("Digite a Data de criação do time");
                dt = DateOnly.Parse(Console.ReadLine());

                string day = dt.ToString().Substring(0,2);
                string month = dt.ToString().Substring(3,2);
                string year = dt.ToString().Substring(6,4);
                string date = year + "-" + month + "-" + day;

                name.Value = Nome;
                nick.Value = Apelido;
                creationDate.Value = date;

                cmd.Parameters.Add(name);
                cmd.Parameters.Add(nick);
                cmd.Parameters.Add(creationDate);

                cmd.Connection = conectionSql;
                cmd.ExecuteNonQuery();
                conectionSql.Close();

                int id = ReturnId(teams);

                return new(Nome,Apelido,dt,id);
            }

            void PrintSimpleTeams()
            {
                int curr = 0;
                do
                {
                    teams[curr].PrintSimple();
                    curr++;
                } while (curr != teams.Count);
            }

            int ReturnId(List<Team> list)
            {
                if(list.Count == 0)
                {
                    return 1;
                }
                else
                {
                    return (list.Count + 1);
                }
            }

            int ReturnCode(List<Game> list)
            {
                if (list.Count == 0)
                {
                    return 1;
                }else
                    return (list.Count + 1);    
            }

            Game CreateGame()
            {
                // Exibe todos os times
                
                int House = 0, visit = 0, goalsHouse = 0, goalsVisit = 0;
                bool ok = false, exist = false;
                do
                {
                    //time da casa
                    do
                    {
                        try
                        {

                            Console.WriteLine("Insira o Id do time da Casa:");
                            House = int.Parse(Console.ReadLine());
                            if (teams.Exists(x => x.GetId() == House))
                            {
                                ok = true;
                            }
                            else
                            {
                                Console.WriteLine("Time não cadastrado!");
                            }

                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Digite um valor valido");
                        }

                    } while (ok == false);

                    ok = false;
                    //time visitante
                    do
                    {
                        try
                        {

                            Console.WriteLine("Insira o Id do time da Visitante:");
                            visit = int.Parse(Console.ReadLine());
                            if (teams.Exists(x => x.GetId() == visit))
                            {
                                if (House != visit)
                                {
                                    ok = true;
                                }
                                else
                                {
                                    Console.WriteLine("Um tim enão pode jogar com ele mesmo!");
                                }

                            }
                            else
                            {
                                Console.WriteLine("Time não cadastrado!");
                            }

                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Digite um valor valido");
                        }

                    } while (ok == false);

                    //verifica se o jogo ja existe
                    if ( games.Count > 0 && games.Exists(x => x.GetHouseTeam() == House && x.GetVisitTeam() == visit))
                    {
                        exist = true;
                    }

                } while (exist);

                ok = false;
                //gols da casa
                do
                {
                    try
                    {

                        Console.WriteLine("Insira os gols do time da Casa:");
                        goalsHouse = int.Parse(Console.ReadLine());

                        if (goalsHouse < 0)
                            Console.WriteLine("digite um valor valido");
                        else
                            ok = true;                   

                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Digite um valor valido");
                    }

                } while (ok == false);

                ok = false;
                //gols do visitante
                do
                {
                    try
                    {

                        Console.WriteLine("Insira os gols do time da Casa:");
                        goalsVisit = int.Parse(Console.ReadLine());

                        if (goalsVisit < 0)
                            Console.WriteLine("digite um valor valido");
                        else
                            ok = true;

                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Digite um valor valido");
                    }

                } while (ok == false);

                //total gols
                int totalGoals = goalsHouse + goalsVisit;

                //conecta no BD
                DB conn = new DB();

                SqlConnection conectionSql = new SqlConnection(conn.Path());

                conectionSql.Open();

                SqlCommand cmd = new SqlCommand("[dbo].[InserirJogo]", conectionSql);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                SqlParameter houseTeam = new SqlParameter("@idTimeCasa", System.Data.SqlDbType.Int);
                SqlParameter visitTeam = new SqlParameter("@idTimeVisitante", System.Data.SqlDbType.Int);
                SqlParameter houseGoals = new SqlParameter("@golsCasa", System.Data.SqlDbType.Int);
                SqlParameter visitGoals = new SqlParameter("@golsVisitante", System.Data.SqlDbType.Int);

                houseTeam.Value = House;
                visitTeam.Value = visit;
                houseGoals.Value = goalsHouse;
                visitGoals.Value = goalsVisit;

                cmd.Parameters.Add(houseTeam);
                cmd.Parameters.Add(visitTeam);
                cmd.Parameters.Add(houseGoals);
                cmd.Parameters.Add(visitGoals);

                cmd.Connection = conectionSql;
                cmd.ExecuteNonQuery();
                conectionSql.Close();
                int code =ReturnCode(games);

                return new(code,House,visit,goalsHouse,goalsVisit);
            }

            bool ExistTeams()
            {
                DB conn = new DB();

                SqlConnection conectionSql = new SqlConnection(conn.Path());

                conectionSql.Open();

                SqlCommand cmd = new SqlCommand("Select Count(*) From Equipe", conectionSql);
                cmd.CommandType = CommandType.Text;
                int count = (int)cmd.ExecuteScalar();
                if (count > 2)
                    return true;
                else
                    return false;
            }
            bool ExistGames()
            {
                DB conn = new DB();

                SqlConnection conectionSql = new SqlConnection(conn.Path());

                conectionSql.Open();

                SqlCommand cmd = new SqlCommand("Select Count(*) From Jogo", conectionSql);
                cmd.CommandType = CommandType.Text;
                int count = (int)cmd.ExecuteScalar();
                if (count > (teams.Count * teams.Count - teams.Count))
                    return true;
                else
                    return false;
            }

           void ReloadChampionshipWithTeams()
           {
                DB conn = new DB();
                SqlConnection connectionSql = new SqlConnection(conn.Path());
                connectionSql.Open();
                SqlCommand cmd = new SqlCommand("[dbo].[ReiniciarCampeonato]");
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Connection = connectionSql;

                cmd.ExecuteNonQuery();
                connectionSql.Close();
            }

            void ReloadChampionship()
            {
                DB conn = new DB();
                SqlConnection connectionSql = new SqlConnection(conn.Path());
                connectionSql.Open();
                SqlCommand cmd = new SqlCommand("[dbo].[ResetarTabelas]");
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Connection = connectionSql;

                cmd.ExecuteNonQuery();
                connectionSql.Close();
            }

            int op = 0;

            VerifyTeams();
            VerifyGames();
            do
            {                
                op = Menu();

                switch (op)
                {
                    case 1:

                        Console.WriteLine("Cadastre ao menos 3 times para começar o campeonato, e no maximo 5 times!");

                        int resp = 1;
                        // cadastrar Times
                        
                        do
                        {
                            int act = teams.Count;
                            Console.WriteLine($"Digite os dados do {teams.Count + 1}º time");
                            teams.Add(CreateTeam());
                            if (act < teams.Count)
                                Console.WriteLine("Time inserido com sucesso!");
                            if (teams.Count > 2)
                            {
                                Console.WriteLine($"Você ja cadastrou {teams.Count} times");
                                do
                                {
                                    try
                                    {
                                        Console.WriteLine("Deseja cadastrar outro? 1-sim, 0-não");
                                        resp = int.Parse(Console.ReadLine());
                                    }
                                    catch (Exception)
                                    {
                                        Console.WriteLine("Digite um valor valido!");
                                    }

                                }
                                while (resp != 1 && resp != 0);
                            }
                        } while (teams.Count < 5 && resp == 1);
                        break;
                    case 2:
                        if (ExistTeams())
                        {
                            //cadastrar jogos
                            Console.WriteLine("Agora Cadastre os jogos e os placares ocorridos");
                            do
                            {
                                int act = games.Count;
                                Console.WriteLine("Times Disponiveis");
                                PrintSimpleTeams();
                                Console.WriteLine($"Cadastre o {games.Count}ºjogo");
                                games.Add(CreateGame());
                                if (act < games.Count)
                                    Console.WriteLine("Jogo inserido com sucesso!");

                            } while (games.Count != (teams.Count * teams.Count - teams.Count));
                        }
                        else
                        {
                            Console.WriteLine("Primeiro você deve cadastrar os times do campeonato!");
                        }
                        break;
                    case 3:
                        if (ExistGames())
                        {
                            DB conn = new DB();

                            SqlConnection conectionSql = new SqlConnection(conn.Path());

                            conectionSql.Open();

                            SqlCommand cmd = new SqlCommand("[dbo].[RetornarRanking]", conectionSql);
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;

                            cmd.Connection = conectionSql;

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {

                                while (reader.Read())
                                {
                                    string nome = reader["Nome"].ToString();
                                    int pontuacao = int.Parse(reader["Pontuacao"].ToString());
                                    int totGolsMarcados = int.Parse(reader["TotalGolsMarcados"].ToString());
                                    int totGolsSofridos = int.Parse(reader["TotalGolsSofridos"].ToString());

                                    Console.WriteLine($"Nome:{nome}");
                                    Console.WriteLine($"Pontuãção:{pontuacao}");
                                    Console.WriteLine($"Total de Gols Marcados:{totGolsMarcados}");
                                    Console.WriteLine($"Total de gols sofridos:{totGolsSofridos}\n");
                                }
                            }

                            conectionSql.Close();
                        }
                        else
                        {
                            Console.WriteLine("Você deve cadastrar os jogos primeiramente");
                        }
                        break;
                    case 4:
                        if (ExistGames())
                        {
                            DB conn = new DB();

                            SqlConnection conectionSql = new SqlConnection(conn.Path());

                            conectionSql.Open();

                            SqlCommand cmd = new SqlCommand("[dbo].[RetornarTimeComMaisGols]", conectionSql);
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;

                            cmd.Connection = conectionSql;

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {

                                while (reader.Read())
                                {
                                    string nome = reader["Nome"].ToString();
                                    int totGolsMarcados = int.Parse(reader["TotalGolsMarcados"].ToString());


                                    Console.WriteLine($"Nome:{nome}");
                                    Console.WriteLine($"Total de Gols Marcados: {totGolsMarcados}");
                                }
                            }

                            conectionSql.Close();
                        }
                        else
                        {
                            Console.WriteLine("Você deve cadastrar os jogos primeiramente");
                        }
                        break;
                    case 5:
                        if (ExistGames())
                        {
                            DB conn = new DB();

                            SqlConnection conectionSql = new SqlConnection(conn.Path());

                            conectionSql.Open();

                            SqlCommand cmd = new SqlCommand("[dbo].[RetornarTimeComMaisGolsSofridos]", conectionSql);
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;

                            cmd.Connection = conectionSql;

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {

                                while (reader.Read())
                                {
                                    string nome = reader["Nome"].ToString();
                                    int totGolsSofridos = int.Parse(reader["TotalGolsSofridos"].ToString());

                                    Console.WriteLine($"Nome:{nome}");
                                    Console.WriteLine($"Total de gols sofridos:{totGolsSofridos}\n");
                                }
                            }

                            conectionSql.Close();
                        }
                        else
                        {
                            Console.WriteLine("Você deve cadastrar os jogos primeiramente");
                        }
                        break;
                    case 6:
                        if (ExistGames())
                        {
                            DB conn = new DB();

                            SqlConnection conectionSql = new SqlConnection(conn.Path());

                            conectionSql.Open();

                            SqlCommand cmd = new SqlCommand("[dbo].[RetornarJogoComMaisGols]", conectionSql);
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;

                            cmd.Connection = conectionSql;

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {

                                while (reader.Read())
                                {
                                    string TimeCasa = reader["TimeCasa"].ToString();
                                    string TimeVisitante = reader["TimeVisitante"].ToString();
                                    int golsDaCasa = int.Parse(reader["GolsDaCasa"].ToString());
                                    int golsVisi = int.Parse(reader["GolsVisi"].ToString());
                                    int TotalGols = int.Parse(reader["TotalGols"].ToString());

                                    Console.WriteLine($"Time da casa: {TimeCasa}");
                                    Console.WriteLine($"Time Visitante: {TimeVisitante}");
                                    Console.WriteLine($"Gols do time da casa: {golsDaCasa}");
                                    Console.WriteLine($"Gols do tme visitante: {golsVisi}");
                                    Console.WriteLine($"Total de gols do jogo: {TotalGols}");
                                }
                            }

                            conectionSql.Close();
                        }
                        else
                        {
                            Console.WriteLine("Você deve cadastrar os jogos primeiramente");
                        }
                        break;
                    case 7:
                        if (ExistGames())
                        {
                            DB conn = new DB();

                            SqlConnection conectionSql = new SqlConnection(conn.Path());

                            conectionSql.Open();

                            SqlCommand cmd = new SqlCommand("[dbo].[RetornarJogoMaisGolsTime]", conectionSql);
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;

                            cmd.Connection = conectionSql;

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {

                                while (reader.Read())
                                {
                                    string nome = reader["Nome"].ToString();
                                    int gols = int.Parse(reader["Gols"].ToString());


                                    Console.WriteLine($"Nome:{nome}");
                                    Console.WriteLine($"Gols no Jogo:{gols}\n");

                                }
                            }

                            conectionSql.Close();
                        }
                        else
                        {
                            Console.WriteLine("Você deve cadastrar os jogos primeiramente");
                        }
                        break;
                    case 8:
                        if (ExistGames())
                        {
                            DB conn = new DB();

                            SqlConnection conectionSql = new SqlConnection(conn.Path());

                            conectionSql.Open();

                            SqlCommand cmd = new SqlCommand("[dbo].[RetornarCampeao]", conectionSql);
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;

                            cmd.Connection = conectionSql;

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {

                                while (reader.Read())
                                {
                                    string nome = reader["Nome"].ToString();
                                    int pontuacao = int.Parse(reader["Pontuacao"].ToString());
                                    int golsMarcados = int.Parse(reader["TotalGolsMarcados"].ToString());
                                    int golsSofridos = int.Parse(reader["TotalGolsSofridos"].ToString());


                                    Console.WriteLine($"Nome:{nome}");
                                    Console.WriteLine($"Pontuação: {pontuacao} pontos");
                                    Console.WriteLine($"Gols marcados no campeonato: {golsMarcados} gols");
                                    Console.WriteLine($"Gols sofridos no campeonato: {golsSofridos} gols");


                                }
                            }

                            conectionSql.Close();
                        }
                        else
                        {
                            Console.WriteLine("Você deve cadastrar os jogos primeiramente");
                        }
                        break;
                    case 9:
                        if (ExistTeams())
                        {
                            Console.WriteLine("Reiniciando campeontato com os mesmos times!");

                            ReloadChampionshipWithTeams();
                        }
                        else
                        {
                            Console.WriteLine("Você não cadastrou times suficientes para um campeonato! reiniciando por completo...");
                            ReloadChampionship();
                        }
                        break;
                    case 10:
                        Console.WriteLine("Reiniciando campeonato por completo");
                        ReloadChampionship();
                        break;
                    case 0:
                        Console.WriteLine("Saindo...");
                        break;
                    default:
                        break;
                }
            } while (op != 0);            

        }

    }
}
