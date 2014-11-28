using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using ExifLib;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure;
using System.Configuration;
using System.IO;

namespace WebApplication1
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        SqlCommand command;
        SqlDataReader rdr = null;
        int status_int = 0;
        string op = "";
        string category = "";
        string desc = "";
        int category_num = 0;
        string fileLocation = "";
        string saveName = "";
        string saveNameThumb = "";
        Int32 newId;
        // Provide the following information
        private static string userName = "brance";
        private static string password = "SnowBallFight123";
        private static string dataSource = "r4blr6m1h1.database.windows.net";
        private static string sampleDatabaseName = "tuzibabaDB";
        SqlConnectionStringBuilder connString1Builder;
        System.Data.SqlClient.SqlCommand cmd;
        System.Data.SqlClient.SqlConnection sqlConnection1;
        SqlConnection cnn;
        SqlDataReader reader;

        SqlConnection conn;
        protected void Page_Load(object sender, EventArgs e)
        {
            op = Request.QueryString["OP"];
            if (string.Equals(op, "Get"))
            {
                string status = Request.QueryString["status"];
                Console.Write("Status" + status);
                //  Response.Write("Status: " + status);
                // ToInt32 can throw FormatException or OverflowException. 
                try
                {
                    status_int = Convert.ToInt32(status);
                }
                catch (FormatException ex)
                {
                    Console.WriteLine("Input string is not a sequence of digits.");
                }
                catch (OverflowException ex)
                {
                    Console.WriteLine("The number cannot fit in an Int32.");
                }
                finally
                {
                    if (status_int < Int32.MaxValue)
                    {
                        Console.WriteLine("The new value is {0}", status_int + 1);
                    }
                    else
                    {
                        Console.WriteLine("numVal cannot be incremented beyond its current value");
                    }
                }

                try
                {

                    
                        SqlConnectionStringBuilder connString1Builder;
                        connString1Builder = new SqlConnectionStringBuilder();
                        connString1Builder.DataSource = dataSource;
                        connString1Builder.InitialCatalog = sampleDatabaseName;
                        connString1Builder.Encrypt = true;
                        connString1Builder.TrustServerCertificate = false;
                        connString1Builder.UserID = userName;
                        connString1Builder.Password = password;
                        // Connect to the sample database and perform various operations
                        using (conn = new SqlConnection(connString1Builder.ToString()))
                        {
                            using (SqlCommand command = conn.CreateCommand())
                            {
                                conn.Open();
                                command.CommandText = ("Select * from Table_2 WHERE status=' " + status_int + "' ");
                                rdr = command.ExecuteReader();

                                
                                StringBuilder JSON = new StringBuilder();
                                // we got the answer now make json 
                                JSON.Append("[");
                                while (rdr.Read())
                                {
                                    JSON.Append("{");
                                    JSON.Append("\"first\":\"" + rdr["first"].ToString() + "\", ");
                                    JSON.Append("\"last\":\"" + rdr["last"].ToString() + "\", ");
                                    JSON.Append("\"ID\":\"" + rdr["ID"].ToString() + "\", ");
                                    JSON.Append("\"imageName\":\"" + rdr["imageName"].ToString() + "\", ");
                                    JSON.Append("\"status\":\"" + rdr["status"].ToString() + "\", ");
                                    JSON.Append("\"longitude\":\"" + rdr["longitude"].ToString() + "\", ");
                                    JSON.Append("\"latitude\":\"" + rdr["latitude"].ToString() + "\", ");
                                    JSON.Append("\"date\":\"" + rdr["date"].ToString() + "\" ");
                                    JSON.Append("},");
                                    //Response.Write(rdr["first"].ToString() + "  ");
                                    // Response.Write(rdr["last"].ToString() + "\r\n");

                                }
                                if (JSON.ToString().EndsWith(","))
                                    JSON = JSON.Remove(JSON.Length - 1, 1);
                                JSON.Append("]");
                                Response.Clear();
                                Response.Write(JSON);
                                Response.End(); // This throws exception every time
                               // HttpContext.Current.ApplicationInstance.CompleteRequest();
                                

                            }
                        }

                        //  sqlConnection1.Close();
                    
   
                   
                }
                catch (Exception ex)
                {
                    // Response.End() Throws exception!!!
                   // Response.Write("Can not open connection ! " + ex.ToString());
                }
               
                finally
                {
                    // Close data reader object and database connection
                    if (rdr != null)
                    {
                        rdr.Close();
                    }
                  //  if (cnn.State == System.Data.ConnectionState.Open)
                      //  cnn.Close();
                    if (conn.State == System.Data.ConnectionState.Open)
                        conn.Close();
                        
                }
            }
            else
                if (string.Equals(op, "Put"))
                {
                    // Get Details from Form which is in html page 
                    try
                    {
                        category = Request.Form["Kategorija"].ToString();
                        desc = Request.Form["Opis"].ToString();
                        switch (category)
                        {
                            case "Ostalo":
                                category_num = 0;
                                break;
                            case "Rupa na putu":
                                category_num = 1;
                                break;
                            case "Smeće":
                                category_num = 2;
                                break;
                            case "Grafiti":
                                category_num = 3;
                                break;
                            case "Ulična rasveta":
                                category_num = 4;
                                break;
                            case "Nepropisno parkiranje":
                                category_num = 5;
                                break;
                            case "Zapušeni slivnici":
                                category_num = 6;
                                break;
                            default:
                                category_num = 0;
                                break;
                        }
                    }
                    catch (Exception ee)
                    {
                    }
                    // add stuff to db
                    try
                    {

                        SqlConnectionStringBuilder connString1Builder;
                        connString1Builder = new SqlConnectionStringBuilder();
                        connString1Builder.DataSource = dataSource;
                        connString1Builder.InitialCatalog = sampleDatabaseName;
                        connString1Builder.Encrypt = true;
                        connString1Builder.TrustServerCertificate = false;
                        connString1Builder.UserID = userName;
                        connString1Builder.Password = password;
                        // Connect to the sample database and perform various operations
                        using (SqlConnection conn = new SqlConnection(connString1Builder.ToString()))
                        {
                            using (SqlCommand command = conn.CreateCommand())
                            {
                                conn.Open();
                                command.CommandText = "INSERT Table_2 (first, last, latitude, longitude, status ) output INSERTED.ID VALUES ('" + category_num + "', '" + desc + "',0,0,0) ";
                            //    command.ExecuteNonQuery();
                                newId = (Int32)command.ExecuteScalar();
                                conn.Close();

                            }
                        }

                        //  sqlConnection1.Close();
                    }
                    catch (Exception db_ex)
                    {
                        Response.Write("Exception 1: " + db_ex.ToString());
                    }

                    // Get image from form, upload it
                    if (Request.Files["userfile"] != null)
                    {
                        HttpPostedFile MyFile = Request.Files["userfile"];
                        fileLocation = "";
                        //Setting location to upload files
                   //     string TargetLocation = Server.MapPath("images/");
                        try
                        {
                            if (MyFile.ContentLength > 0)
                            {
                              
                                saveName = "uploaded_image" + newId + ".jpg";
                                saveNameThumb = saveName = "uploaded_image_t" + newId + ".jpg";
                                try
                                {
                                    // Retrieve storage account from connection string.                    
                                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                                        CloudConfigurationManager.GetSetting("StorageConnectionString"));


                                    // Create the blob client.
                                    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                                    // Retrieve reference to a previously created container.
                                    CloudBlobContainer container = blobClient.GetContainerReference("mycontainer");

                                    // Retrieve reference to a blob named "myblob".
                                    CloudBlockBlob blockBlob = container.GetBlockBlobReference(saveName);

                                    blockBlob.UploadFromStream(MyFile.InputStream);
                                   
                                    // Create thumbnail of uploaded image
                                    int width = 300;
                                    int height = 300;
                                    Bitmap srcBmp = new Bitmap(MyFile.InputStream);
                                    float ratio = srcBmp.Width / srcBmp.Height;
                                    SizeF newSize = new SizeF(width, height * ratio);
                                    Bitmap target = new Bitmap((int)newSize.Width, (int)newSize.Height);

                                    /* *********************************
                                     *  upload thumbnail to blobStorage
                                     * ********************************** */ 
                                    // Retrieve reference to a blob named "myblob".
                                    blockBlob = container.GetBlockBlobReference(saveNameThumb);
                                    ImageConverter converter = new ImageConverter();
                                    byte[] byteArray = new byte[0];
                                    using (MemoryStream stream = new MemoryStream())
                                    {
                                        target.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                                        stream.Close();
                                        byteArray = stream.ToArray();
                                    }
                                    using (var stream = new MemoryStream(byteArray, writable: false))
                                    {
                                        blockBlob.UploadFromStream(stream);
                                    }


                                    // Update DB
                                    // Read GeoTag from image : coord_ret[0] = latitude; coord_ret[1] = longitude;
                                    // disabled double[] myCoordinates = Test(MyFile);
                                    double[] myCoordinates = null;
                                    if (myCoordinates != null)
                                    {
                                        //Update db
                                   //     Response.Write("longitude = " + myCoordinates[1] + "latitude" + myCoordinates[0]);
                                   //     cmd.CommandText = ("Update Table_1 Set imageName = 'uploaded_image" + newId + ".jpg', longitude = " + myCoordinates[1]
                                   //                      + " ,latitude =" + myCoordinates[0] + " Where ID='" + newId + "'");
                                        SqlConnectionStringBuilder connString1Builder;
                                        connString1Builder = new SqlConnectionStringBuilder();
                                        connString1Builder.DataSource = dataSource;
                                        connString1Builder.InitialCatalog = sampleDatabaseName;
                                        connString1Builder.Encrypt = true;
                                        connString1Builder.TrustServerCertificate = false;
                                        connString1Builder.UserID = userName;
                                        connString1Builder.Password = password;
                                        using (SqlConnection conn = new SqlConnection(connString1Builder.ToString()))
                                        {
                                            using (SqlCommand command = conn.CreateCommand())
                                            {
                                                conn.Open();
                                                command.CommandText = ("Update Table_2 Set imageName = 'uploaded_image" + newId + ".jpg', longitude = " + myCoordinates[1]
                                                         + " ,latitude =" + myCoordinates[0] + " Where ID='" + newId + "'");
                                                command.ExecuteNonQuery();
                                               // newId = (Int32)command.ExecuteScalar();
                                                conn.Close();
                                                Response.Write("Successfully uploaded!");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //Update db
                                      //  cmd.CommandText = ("Update Table_1 Set imageName = 'uploaded_image" + newId + ".jpg' Where ID='" + newId + "'");
                                        SqlConnectionStringBuilder connString1Builder;
                                        connString1Builder = new SqlConnectionStringBuilder();
                                        connString1Builder.DataSource = dataSource;
                                        connString1Builder.InitialCatalog = sampleDatabaseName;
                                        connString1Builder.Encrypt = true;
                                        connString1Builder.TrustServerCertificate = false;
                                        connString1Builder.UserID = userName;
                                        connString1Builder.Password = password;
                                        using (SqlConnection conn = new SqlConnection(connString1Builder.ToString()))
                                        {
                                            using (SqlCommand command = conn.CreateCommand())
                                            {
                                                conn.Open();
                                                command.CommandText = ("Update Table_2 Set imageName = 'uploaded_image" + newId + ".jpg' Where ID='" + newId + "'");
                                                command.ExecuteNonQuery();
                                                // newId = (Int32)command.ExecuteScalar();
                                                conn.Close();
                                                Response.Write("Successfully uploaded!");
                                            }
                                        }
                                    }

                                }
                                catch (Exception e22)
                                {
                                    Response.Write("Error storage1 :" + " File path: " + fileLocation + " error log : ********" + e22.ToString());
                                }

                                // save image to Azure
                            }
                        } 
                        catch (Exception e111)
                        {
                            Response.Write("Image Path : " + e111.ToString());
                        }
                    }
                  
                }

        }
     
    }

 
}

 