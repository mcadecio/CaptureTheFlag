using System;
using System.Collections;
using System.Collections.Generic;
//using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using UnityEngine;
using UnityEngine.UI;

public class TestDB : MonoBehaviour
{
    MySqlConnection mySQLConnection;

    public Button button;
    // Start is called before the first frame update
    void Start()
    {
        Button btn = button.GetComponent<Button>();
        btn.onClick.AddListener(onClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void onClick()
    {


        string server = "uk-ctf.uksouth.cloudapp.azure.com";
        string database = "demodb";
        string user = "root";
        string password = "pass";
        string port = "3306";
        string sslM = "none";
        string connectionString = String.Format("server={0};port={1};user id={2}; password={3}; database={4}; SslMode={5}",
            server, port, user, password, database, sslM);

        mySQLConnection = new MySqlConnection(connectionString);
        try
        {
            mySQLConnection.Open();

            MySqlCommand command = mySQLConnection.CreateCommand();
            
            

            command.CommandText = "INSERT INTO demotb (id, name)" +
                "values (1, 'amen');";
            try
            {
                command.ExecuteNonQuery();

                Debug.Log("Success");

            }

            catch (Exception e)
            {
                Debug.Log(e.Message);

            }


        }
        catch (Exception E)
        {
            Debug.Log(E.Message);
            Debug.Log("No success");
        }
    }
}
