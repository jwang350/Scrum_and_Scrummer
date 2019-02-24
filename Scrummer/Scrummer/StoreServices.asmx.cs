using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

//we need these to talk to mysql
using MySql.Data;
using MySql.Data.MySqlClient;
//and we need this to manipulate data from a db
using System.Data;

namespace Scrummer
{
    /// <summary>
    /// Summary description for StoreServices
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class StoreServices : System.Web.Services.WebService
    {

        [WebMethod]
        public Store[] GetStoreList()
        {
            DataTable sqlDt = new DataTable("stores");

            string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;
            string sqlSelect = "select * from stores";

            MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
            MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

            //gonna use this to fill a data table
            MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);
            //filling the data table
            sqlDa.Fill(sqlDt);

            //loop through each row in the dataset, creating instances
            //of our container class Account.  Fill each acciount with
            //data from the rows, then dump them in a list.
            List<Store> stores = new List<Store>();
            for (int i = 0; i < sqlDt.Rows.Count; i++)
            {
                stores.Add(new Store
                {
                    id = Convert.ToInt32(sqlDt.Rows[i]["Id"]),
                    storeName = sqlDt.Rows[i]["storeName"].ToString(),
                });
            }
            //convert the list of accounts to an array and return!
            return stores.ToArray();
        }

        // Add item to list in DB
        [WebMethod(EnableSession = true)]
        public void AddStoreToList(string store)
        {
            string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;
            //the only thing fancy about this query is SELECT LAST_INSERT_ID() at the end.  All that
            //does is tell mySql server to return the primary key of the last inserted row.
            string sqlSelect = "insert into stores (storeName) " +
                "values(@storevalue);";

            MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
            MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

            sqlCommand.Parameters.AddWithValue("@storevalue", HttpUtility.UrlDecode(store));

            //this time, we're not using a data adapter to fill a data table.  We're just
            //opening the connection, telling our command to "executescalar" which says basically
            //execute the query and just hand me back the number the query returns (the ID, remember?).
            //don't forget to close the connection!
            sqlConnection.Open();
            //we're using a try/catch so that if the query errors out we can handle it gracefully
            //by closing the connection and moving on
            try
            {
                int accountID = Convert.ToInt32(sqlCommand.ExecuteScalar());
                //here, you could use this accountID for additional queries regarding
                //the requested account.  Really this is just an example to show you
                //a query where you get the primary key of the inserted row back from
                //the database!
            }
            catch (Exception e)
            {
            }
            sqlConnection.Close();
        }

        //EXAMPLE OF AN UPDATE QUERY WITH PARAMS PASSED IN
        [WebMethod(EnableSession = true)]
        public void UpdateStore(string id, string store)
        {
            string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;
            //this is a simple update, with parameters to pass in values
            string sqlSelect = "update stores set storeName=@storeValue where id=@idValue";

            MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
            MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

            sqlCommand.Parameters.AddWithValue("@storeValue", HttpUtility.UrlDecode(store));
            sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(id));

            sqlConnection.Open();
            //we're using a try/catch so that if the query errors out we can handle it gracefully
            //by closing the connection and moving on
            try
            {
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
            }
            sqlConnection.Close();
        }

        //EXAMPLE OF A DELETE QUERY
        [WebMethod(EnableSession = true)]
        public void DeleteStore(string id)
        {
            string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;
            //this is a simple update, with parameters to pass in values
            string sqlSelect = "delete from stores where id=@idValue";

            MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
            MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

            sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(id));

            sqlConnection.Open();
            try
            {
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
            }
            sqlConnection.Close();
        }
    }
}
