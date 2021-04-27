using System;
using System.Web.UI;
using System.Threading;
using Timer = System.Web.UI.Timer;
using System.Net.Sockets;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using System.Net;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Diagnostics;

namespace BOD
{
    public partial class _Default : Page
    {

        public static Table aisTable;
        public static CancellationTokenSource source = new CancellationTokenSource();
        public static CancellationToken token = source.Token;
        private static AmazonDynamoDBClient client = new AmazonDynamoDBClient();
        private static string tableName = "bod-data";
        private static bool operationSucceeded, operationFailed;
        private const int COUNTER_MAX = 1000000;
        private const int OUTPUT_FREQUENCY = 1000;
        static int counter = 0;
        protected void Page_Load(object sender, EventArgs e)
        {


        }

        protected void btnProcess_Click(object sender, EventArgs e)
        {
            bodLoop();
        }

        private void bodLoop()
        {

            try
            {
                // Iterate counter.
                counter++;

                // Output counter value every so often.
                if (counter % OUTPUT_FREQUENCY == 0)
                {
                    Debug.WriteLine($"Current counter: {counter}.");
                }

                // Check if counter has reached maximum value; if not, allow recursion.
                if (counter <= COUNTER_MAX)
                {
                    // Recursively call self method.
                    WebClient webClient = new WebClient();
                    webClient.Credentials = new NetworkCredential("");
                    string myJSON = webClient.DownloadString("");

                    dynamic jsonObj = JsonConvert.DeserializeObject(myJSON);
                    for (int i = 0; i < jsonObj.Count; i++)
                    {
                        Document newItem = new Document();

                        newItem["ID"] = DateTime.Now.ToShortDateString().ToString() + "-" + jsonObj[i]["imo"].ToString() + "-" + DateTime.Now.ToLongTimeString().ToString() + "-" + jsonObj[i]["mmsi"].ToString(); ;
                        newItem["Received"] = jsonObj[i]["received"].ToString();
                        newItem["YearOfBuild"] = jsonObj[i]["yearOfBuild"].ToString();
                        newItem["VesselType"] = jsonObj[i]["vesselType"].ToString();
                        newItem["Draught"] = jsonObj[i]["draught"].ToString();
                        newItem["Ircs"] = jsonObj[i]["ircs"].ToString();
                        newItem["Deadweight"] = jsonObj[i]["deadweight"].ToString();
                        newItem["NavigationalStatus"] = jsonObj[i]["navigationalStatus"].ToString();
                        newItem["RateOfTurn"] = jsonObj[i]["rateOfTurn"].ToString();
                        newItem["Destination"] = jsonObj[i]["destination"].ToString();
                        newItem["ETA"] = jsonObj[i]["eta"].ToString();
                        newItem["Country"] = jsonObj[i]["country"].ToString();
                        newItem["CountryCode"] = jsonObj[i]["countryCode"].ToString();
                        newItem["Longitude"] = jsonObj[i]["longitude"].ToString();
                        newItem["Latitude"] = jsonObj[i]["latitude"].ToString();
                        newItem["Speed"] = jsonObj[i]["speed"].ToString();
                        newItem["Course"] = jsonObj[i]["course"].ToString();
                        newItem["Heading"] = jsonObj[i]["heading"].ToString();
                        newItem["Date"] = jsonObj[i]["date"].ToString();
                        newItem["Name"] = jsonObj[i]["name"].ToString();
                        newItem["IMO"] = jsonObj[i]["imo"].ToString();
                        newItem["MMSI"] = jsonObj[i]["mmsi"].ToString();

                        Table bodData = Table.LoadTable(client, tableName);

                        bodData.PutItem(newItem);
                        Debug.WriteLine("Item " + i + " created!");
                    }
                    Thread.Sleep(3600000); //1 hour interval
                    bodLoop();
                }
                else
                {
                    Debug.WriteLine("Recursion halted.");
                }
            }
            catch (StackOverflowException exception)
            {
                Debug.WriteLine(exception);
            }
        }


        /*--------------------------------------------------------------------------
     *          createClient
     *--------------------------------------------------------------------------*/
        public static bool createClient(bool useDynamoDBLocal)
        {

            if (useDynamoDBLocal)
            {
                operationSucceeded = false;
                operationFailed = false;

                // First, check to see whether anyone is listening on the DynamoDB local port
                // (by default, this is port 8000, so if you are using a different port, modify this accordingly)
                bool localFound = false;
                try
                {
                    using (var tcp_client = new TcpClient())
                    {
                        var result = tcp_client.BeginConnect("localhost", 8000, null, null);
                        localFound = result.AsyncWaitHandle.WaitOne(3000); // Wait 3 seconds
                        tcp_client.EndConnect(result);
                    }
                }
                catch
                {
                    localFound = false;
                }
                if (!localFound)
                {
                    Debug.WriteLine("\n      ERROR: DynamoDB Local does not appear to have been started..." +
                                      "\n        (checked port 8000)");
                    operationFailed = true;
                    return (false);
                }

                // If DynamoDB-Local does seem to be running, so create a client
                Debug.WriteLine("  -- Setting up a DynamoDB-Local client (DynamoDB Local seems to be running)");
                AmazonDynamoDBConfig ddbConfig = new AmazonDynamoDBConfig();
                ddbConfig.ServiceURL = "http://localhost:8000";
                try { client = new AmazonDynamoDBClient(ddbConfig); }
                catch (Exception ex)
                {
                    Debug.WriteLine("     FAILED to create a DynamoDBLocal client; " + ex.Message);
                    operationFailed = true;
                    return false;
                }
            }

            else
            {
                try { client = new AmazonDynamoDBClient(); }
                catch (Exception ex)
                {
                    Debug.WriteLine("     FAILED to create a DynamoDB client; " + ex.Message);
                    operationFailed = true;
                }
            }
            operationSucceeded = true;
            return true;
        }



    }
}