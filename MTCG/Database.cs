﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace MTCG
{
    class Database
    {
        const string connectionstring = "Host=localhost;Username=postgres;Password=;Database=postgres";
        private NpgsqlConnection connection; 

        public NpgsqlConnection connect()
        {
            connection = new NpgsqlConnection(connectionstring);
            connection.Open();
            return connection; 
        }

        public void disconnect()
        {
            connection.Close(); 
        }

        public void addUser(string name, string password, int coins, int elo)
        {
            connect(); 
            using (var cmd = new NpgsqlCommand("INSERT INTO users (name, password, coins, elo) VALUES (@u, @p, @c, @e)", connection))
            {
                cmd.Parameters.AddWithValue("u", name);
                cmd.Parameters.AddWithValue("p", password);
                cmd.Parameters.AddWithValue("c", coins);
                cmd.Parameters.AddWithValue("e", elo);
                cmd.ExecuteNonQuery();
            }
            disconnect(); 
        }

        public bool userExists(string name)
        {
            connect(); 
            using (var cmd = new NpgsqlCommand("SELECT * FROM users WHERE name = @n ", connection))
            {
                cmd.Parameters.AddWithValue("n", name);
                NpgsqlDataReader reader = cmd.ExecuteReader();
                //string name; 
                
                if(reader.HasRows)
                {
                    //reader.Read();
                    disconnect(); 
                    return true; 
                } else
                {
                    disconnect(); 
                    return false; 
                }

                /*
                name = reader["name"].ToString();

                Console.WriteLine(reader["coins"].ToString());

                if (name == "Simon")
                {
                    Console.WriteLine("USER EXISTS");
                    return 1; 
                } 
                */

            }
        }

        public int loginUser(string name, string password)
        {
            connect();
            using (var cmd = new NpgsqlCommand("SELECT * FROM users WHERE name = @n AND password = @p", connection))
            {
                cmd.Parameters.AddWithValue("n", name);
                cmd.Parameters.AddWithValue("p", password);
                NpgsqlDataReader reader = cmd.ExecuteReader();
                //string name; 

                if (reader.HasRows)
                {
                    reader.Read();
                    //Console.WriteLine("UserID: " + reader["id"]);
                    int result = Convert.ToInt32(reader["id"]);
                    disconnect();
                    return result;
                }
                else
                {
                    disconnect();
                    return 0;
                }

                /*
                name = reader["name"].ToString();

                Console.WriteLine(reader["coins"].ToString());

                if (name == "Simon")
                {
                    Console.WriteLine("USER EXISTS");
                    return 1; 
                } 
                */
            }
        }

        public void addCard(string name, int damage, int cardtype, int elementtype, int racetype)
        {
            connect();
            using (var cmd = new NpgsqlCommand("INSERT INTO cards (name, damage, cardtype, elementtype, racetype) VALUES (@n, @d, @c, @e, @r)", connection))
            {
                cmd.Parameters.AddWithValue("n", name);
                cmd.Parameters.AddWithValue("d", damage);
                cmd.Parameters.AddWithValue("c", cardtype);
                cmd.Parameters.AddWithValue("e", elementtype);
                cmd.Parameters.AddWithValue("r", racetype);
                cmd.ExecuteNonQuery();
            }
            disconnect();
        }


        public Card getCardByID(int cardid)
        {
            connect();
            using (var cmd = new NpgsqlCommand("SELECT * FROM cards WHERE id = @i", connection))
            {
                cmd.Parameters.AddWithValue("i", cardid);
                NpgsqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();                    
                }
                Card card = new Card((string)reader["name"], (int)reader["damage"], (CardTypesEnum.CardTypes)reader["cardtype"], (ElementTypesEnum.ElementTypes)reader["elementtype"], (RaceTypesEnum.RaceTypes)reader["racetype"]);
                disconnect();
                return card; 

            }
        }
        /*
        public Card getCard(int cardid)
        {
            int id = 381; //delete later
            connect();
            using (var cmd = new NpgsqlCommand("SELECT * FROM cards WHERE id = @i", connection))
            {
                cmd.Parameters.AddWithValue("i", id);
                NpgsqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    Card card = new Card((string)reader["name"], (int)reader["damage"], (CardTypesEnum.CardTypes)reader["cardtype"], (ElementTypesEnum.ElementTypes)reader["elementtype"], (RaceTypesEnum.RaceTypes)reader["racetype"]);
                    Console.WriteLine("CARD FROM DB:");
                    card.PrintCard();
                    disconnect();

                }

            }
        }
        */

        public List<Card> getStack(int userid)
        {

            connect();
            using (var cmd = new NpgsqlCommand("SELECT * FROM cards JOIN stack ON cards.id=stack.cardid WHERE userid = @i", connection))
            {
                cmd.Parameters.AddWithValue("i", userid);
                NpgsqlDataReader reader = cmd.ExecuteReader();
                List<Card> stack = new List<Card>();

                if (reader.HasRows)
                {
                    
                    while (reader.Read())
                    {
                        Card card = new Card((int)reader["cardid"], (string)reader["name"], (int)reader["damage"], (CardTypesEnum.CardTypes)reader["cardtype"], (ElementTypesEnum.ElementTypes)reader["elementtype"], (RaceTypesEnum.RaceTypes)reader["racetype"]);
                        stack.Add(card);
                    }
                }
                disconnect(); 
                return stack; 
            }
        }

        public void addCardToStack(int userid, int cardid)
        {
            connect();
            using (var cmd = new NpgsqlCommand("INSERT INTO stack (userid, cardid) VALUES (@uid, @cid)", connection))
            {
                cmd.Parameters.AddWithValue("uid", userid);
                cmd.Parameters.AddWithValue("cid", cardid);
                cmd.ExecuteNonQuery();
            }
            disconnect();
        }

        public void selectCard(int userid, int cardid)
        {
            connect();
            using (var cmd = new NpgsqlCommand("UPDATE stack SET selected = true WHERE userid = @uid AND cardid = @cid", connection))
            {
                cmd.Parameters.AddWithValue("uid", userid);
                cmd.Parameters.AddWithValue("cid", cardid);
                NpgsqlDataReader reader = cmd.ExecuteReader();
            }
            disconnect(); 
        }

        public void deselectCards(int userid)
        {
            connect();
            using (var cmd = new NpgsqlCommand("UPDATE stack SET selected = false WHERE userid = @uid", connection))
            {
                cmd.Parameters.AddWithValue("uid", userid);
                NpgsqlDataReader reader = cmd.ExecuteReader();
            }
            disconnect();
        }

        public int getCardCount(int userid)
        {
            int ocards = 0;
            connect();
            using (var cmd = new NpgsqlCommand("SELECT COUNT(userid) AS ownedcards FROM stack WHERE userid = @uid", connection))
            {
                cmd.Parameters.AddWithValue("uid", userid);
                NpgsqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                ocards = Convert.ToInt32(reader["ownedcards"]);
                Console.WriteLine(reader["ownedcards"]);
            }
            disconnect();
            return ocards;
        }

        public int getSelectedCardCount(int userid)
        {
            int selcards = 0;
            connect();
            using (var cmd = new NpgsqlCommand("SELECT COUNT(selected) AS selectedcards FROM stack WHERE userid = @uid AND selected = true", connection))
            {
                cmd.Parameters.AddWithValue("uid", userid);
                NpgsqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                selcards = Convert.ToInt32(reader["selectedcards"]);
            }
            disconnect();
            return selcards; 
        }

        public int getRandomCardID()
        {
            connect();
            using (var cmd = new NpgsqlCommand("SELECT * FROM cards ORDER BY RANDOM()", connection))
            {
                NpgsqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                }
                int cardid = Convert.ToInt32(reader["id"]);
                disconnect();
                return cardid; 
                /*
                Card card = new Card((string)reader["name"], (int)reader["damage"], (CardTypesEnum.CardTypes)reader["cardtype"], (ElementTypesEnum.ElementTypes)reader["elementtype"], (RaceTypesEnum.RaceTypes)reader["racetype"]);
                Console.WriteLine("CARD FROM DB:");
                card.PrintCard();
                disconnect();
                return card; 
                */
            }
        }
        
        public int getCoinsByUserID(int userid)
        {
            connect();
            using (var cmd = new NpgsqlCommand("SELECT coins FROM users WHERE id = @i", connection))
            {
                cmd.Parameters.AddWithValue("i", userid);
                NpgsqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                }
                int coins = Convert.ToInt32(reader["coins"]);
                Console.WriteLine("COINS:" + coins);
                disconnect();
                return coins;
            }
        }

        public int getEloByUserID(int userid)
        {
            connect();
            using (var cmd = new NpgsqlCommand("SELECT elo FROM users WHERE id = @i", connection))
            {
                cmd.Parameters.AddWithValue("i", userid);
                NpgsqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                }
                int elo = Convert.ToInt32(reader["elo"]);
                Console.WriteLine("ELO:" + elo);
                disconnect();
                return elo;
            }
        }


    }
}
