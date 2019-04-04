using System.Collections;
using UnityEngine;
using MySql.Data.MySqlClient;
using System;
using UnityEngine.UI;

public class LoadScores : MonoBehaviour
{
	public Text usernames;
	public Text scores;

	private MySqlConnection connection;

    // Start is called before the first frame update
    void Start()
    {
		usernames.text = "";
		scores.text = "";

		string server = "172.31.82.35";
		string database = "capture_flag_game";
		string user = "root";
		string password = "main_container_pw";
		string port = "3306";
		string sslM = "none";
		string connectionString = String.Format("server={0};port={1};user id={2}; password={3}; database={4}; SslMode={5}",
			server, port, user, password, database, sslM);

		connection = new MySqlConnection(connectionString);
		connection.Open();
    }

    // Update is called once per frame
    void Update()
    {

		updateLeaderboard();

        
    }

	void updateLeaderboard(){

		string query = "select * from Leaderboard order by totalScore DESC";

		MySqlCommand command =  new MySqlCommand(query, connection);
		MySqlDataReader rdr = command.ExecuteReader();

		string names = "";
		string scores = "";
		while(rdr.Read()){
			Debug.Log(rdr.GetString("name"));
		
			names = String.Concat(names, String.Format("{0}\n", rdr.GetString("name")));
			scores = String.Concat(scores, String.Format("{0}\n", rdr.GetInt64("totalScore")));

		}
		usernames.text = names;
		this.scores.text = scores;

		if(rdr != null){
			rdr.Close();
		}

		/*
        try
        {
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }

        catch (Exception e)
        {
            Debug.Log(e.Message);

        }
        */

	}
}
