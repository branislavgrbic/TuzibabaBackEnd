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

                // string hello = "i need more practice";
                //  Response.Write(hello);


                

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
                    /*
                    if (Request.Files["userfile"] != null)
                    {
                        HttpPostedFile MyFile = Request.Files["userfile"];

                        //Setting location to upload files
                        string TargetLocation = Server.MapPath("images/");
                        try
                        {
                            if (MyFile.ContentLength > 0)
                            {
                                //Determining file name. You can format it as you wish.
                                string FileName = MyFile.FileName;
                                //Determining file size.
                                int FileSize = MyFile.ContentLength;
                                //Creating a byte array corresponding to file size.
                                byte[] FileByteArray = new byte[FileSize];
                                //Posted file is being pushed into byte array.
                                MyFile.InputStream.Read(FileByteArray, 0, FileSize);
                                //Uploading properly formatted file to server.
                                MyFile.SaveAs(TargetLocation + "uploaded_image" + newId + ".jpg");
                                string fileLoc = TargetLocation + "uploaded_image" + newId + ".jpg";

                                //      Response.Write("Success: Kat: " + category + " desc: " + desc + " Id i got: " + newId);

                                // Read GeoTag from image : coord_ret[0] = latitude; coord_ret[1] = longitude;
                                double[] myCoordinates = Test(fileLoc);
                                /*
                                if (myCoordinates != null)
                                {
                                    //Update db
                                    Response.Write("longitude = " + myCoordinates[1] + "latitude" + myCoordinates[0]);
                                    cmd.CommandText = ("Update Table_1 Set imageName = 'uploaded_image" + newId + ".jpg', longitude = " + myCoordinates[1]
                                                     + " ,latitude =" + myCoordinates[0] + " Where ID='" + newId + "'");

                                }
                                else
                                {
                                    //Update db
                                    cmd.CommandText = ("Update Table_1 Set imageName = 'uploaded_image" + newId + ".jpg' Where ID='" + newId + "'");
                                }
                                */
                    //         cmd.ExecuteNonQuery();
                    //      sqlConnection1.Close();

                    // try to save image on Azure
                    /*
                                                // Retrieve storage account from connection string.
                                                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                                                    CloudConfigurationManager.GetSetting("StorageConnectionString"));

                                                // Create the blob client.
                                                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                                                // Retrieve a reference to a container. 
                                                CloudBlobContainer container = blobClient.GetContainerReference("mycontainer");

                                                // Create the container if it doesn't already exist.
                                                container.CreateIfNotExists();

                                                // Retrieve reference to a blob named "myblob".
                                                CloudBlockBlob blockBlob = container.GetBlockBlobReference("myblob");

                                                // Create or overwrite the "myblob" blob with contents from a local file.
                                                using (var fileStream = System.IO.File.OpenRead(fileLoc))
                                                {
                                                    blockBlob.UploadFromStream(fileStream);
                                                } 
                                       */
                    /*
}
}
catch (Exception BlueScreen)
{
//Handle errors
Response.Write("Exception" + BlueScreen.ToString());
}
}*/

                    // Get image from form, upload it
                    if (Request.Files["userfile"] != null)
                    {
                        HttpPostedFile MyFile = Request.Files["userfile"];
                        fileLocation = "";
                        //Setting location to upload files
                        string TargetLocation = Server.MapPath("images/");
                        try
                        {
                            if (MyFile.ContentLength > 0)
                            {
                                //Determining file name. You can format it as you wish.
                              //  string FileName = MyFile.FileName;

                                //Determining file size.
                              //  int FileSize = MyFile.ContentLength;

                                //Creating a byte array corresponding to file size.
                               // byte[] FileByteArray = new byte[FileSize];

                                //Posted file is being pushed into byte array.
                               // MyFile.InputStream.Read(FileByteArray, 0, FileSize);

                                //Uploading properly formatted file to server.
                             //   MyFile.SaveAs(TargetLocation + "uploaded_image" + newId + ".jpg");
                               
                                string fileLoc = TargetLocation + "uploaded_image" + newId + ".jpg";
                                saveName = "uploaded_image" + newId + ".jpg";
                              //  fileLocation = FileName;

                                // Save image to Azure
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
                                    // Update DB
                                    // Read GeoTag from image : coord_ret[0] = latitude; coord_ret[1] = longitude;
          // disabled                          double[] myCoordinates = Test(MyFile);
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



                /*else 
                {
                    try
                    {
                        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                            CloudConfigurationManager.GetSetting("StorageConnectionString"));
    
                        Response.Write("Key: " + ConfigurationManager.AppSettings["StorageConnectionString"]);
                        // Define the connection-string with your values
                            
                    }
                    catch (Exception e22)
                    {
                        Response.Write("Error storage2 :" + e22.ToString());
                    }
                }*/

        }















/*
        protected void Page_Load1(object sender, EventArgs e)
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

                // string hello = "i need more practice";
                //  Response.Write(hello);

       
                string connetionString = null;
    
                connetionString = "Database=Test;Server=BRANCE-DESKTOP\\SQLEXPRESS;Integrated Security=True;connect timeout = 30";
                cnn = new SqlConnection(connetionString);

                try
                {


                    cnn.Open(); 
                    command = new SqlCommand("Select * from Table_1 WHERE status=' " + status_int + "' ");
                    command.Connection = cnn;
                    command.CommandType = System.Data.CommandType.Text;


                    //Response.Write("Connection Open ! "); //

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
                    Response.End();
                    cnn.Close(); //
                }
                catch (Exception ex)
                {
                     Response.Write("Can not open connection ! " + ex.ToString());
                }
                finally
                {
                    // Close data reader object and database connection
                    if (rdr != null)
                    {
                        rdr.Close();
                    }
                    if (cnn.State == System.Data.ConnectionState.Open)
                        cnn.Close();
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
                    switch (category) {
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

                    // Explained on http://msdn.microsoft.com/en-us/library/ms233812.aspx
                    sqlConnection1 = new System.Data.SqlClient.SqlConnection("Database=Test;Server=BRANCE-DESKTOP\\SQLEXPRESS;Integrated Security=True;connect timeout = 30");
                  

                    cmd = new System.Data.SqlClient.SqlCommand();
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = "INSERT Table_1 (first, last, latitude, longitude, status ) output INSERTED.ID VALUES ('" + category_num + "', '" + desc + "',0,0,0) ";
                    cmd.Connection = sqlConnection1;

                    sqlConnection1.Open();
                  //  newId = (Int32)cmd.ExecuteNonQuery();
                    newId = (Int32)cmd.ExecuteScalar();
                    
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
                    
                    //Setting location to upload files
                    string TargetLocation = Server.MapPath("images/");
                    try
                    {
                        if (MyFile.ContentLength > 0)
                        {
                            //Determining file name. You can format it as you wish.
                            string FileName = MyFile.FileName;
                            //Determining file size.
                            int FileSize = MyFile.ContentLength;
                            //Creating a byte array corresponding to file size.
                            byte[] FileByteArray = new byte[FileSize];
                            //Posted file is being pushed into byte array.
                            MyFile.InputStream.Read(FileByteArray, 0, FileSize);
                            //Uploading properly formatted file to server.
                            MyFile.SaveAs(TargetLocation + "uploaded_image" + newId + ".jpg");
                            string fileLoc = TargetLocation + "uploaded_image" + newId + ".jpg";

                      //      Response.Write("Success: Kat: " + category + " desc: " + desc + " Id i got: " + newId);

                            // Read GeoTag from image : coord_ret[0] = latitude; coord_ret[1] = longitude;
                            double[] myCoordinates = Test(fileLoc);
                            if (myCoordinates != null)
                            {
                                //Update db
                                Response.Write("longitude = " + myCoordinates[1] + "latitude" + myCoordinates[0]);
                                cmd.CommandText = ("Update Table_1 Set imageName = 'uploaded_image" + newId + ".jpg', longitude = " + myCoordinates[1]
                                                 + " ,latitude =" + myCoordinates[0] + " Where ID='" + newId + "'");
                                
                            }
                            else
                            {
                                //Update db
                                cmd.CommandText = ("Update Table_1 Set imageName = 'uploaded_image" + newId + ".jpg' Where ID='" + newId + "'");
                            }
          
                   //         cmd.ExecuteNonQuery();
                            sqlConnection1.Close();
                            
                            // try to save image on Azure
/*
                            // Retrieve storage account from connection string.
                            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                                CloudConfigurationManager.GetSetting("StorageConnectionString"));

                            // Create the blob client.
                            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                            // Retrieve a reference to a container. 
                            CloudBlobContainer container = blobClient.GetContainerReference("mycontainer");

                            // Create the container if it doesn't already exist.
                            container.CreateIfNotExists();

                            // Retrieve reference to a blob named "myblob".
                            CloudBlockBlob blockBlob = container.GetBlockBlobReference("myblob");

                            // Create or overwrite the "myblob" blob with contents from a local file.
                            using (var fileStream = System.IO.File.OpenRead(fileLoc))
                            {
                                blockBlob.UploadFromStream(fileStream);
                            } 
                   */     
    /*
                        }
                    }
                    catch (Exception BlueScreen)
                    {
                        //Handle errors
                        Response.Write("Exception" + BlueScreen.ToString());
                    }
                }
            }
        }
*/
        // cordinates that we read from Picture (GeoTag);
        /*
        double[] coord_ret;
        private double[] Test(HttpPostedFile fileLoc)
        {
            
            try
            {
                // Create an Image object.
                System.Drawing.Image theImage = System.Drawing.Image.FromStream(fileLoc.InputStream);
                Bitmap bmp = new Bitmap(theImage);
             //   System.Drawing.Image theImage = new Bitmap(fileLoc);

                // Get the PropertyItems property from image.
                PropertyItem[] propItems = theImage.PropertyItems;

                double[] picture_latitude;
                double latitude;
                double longitude;
                double[] picture_longitude;
                
                string picture_date = string.Empty;
                using (ExifReader reader = new ExifReader(fileLoc)
                {
                    // Extract the tag data using the ExifTags enumeration
                    DateTime datePictureTaken;
                    if (reader.GetTagValue<DateTime>(ExifTags.DateTimeDigitized,
                                                    out datePictureTaken))
                    {
                        
                        // Do whatever is required with the extracted information
                       // Response.Write(string.Format("The picture was taken on {0}",
                       // datePictureTaken));
                     //   Response.Write("hi");
                    }
      
                    reader.GetTagValue(ExifTags.GPSLongitude, out picture_longitude);
                    reader.GetTagValue(ExifTags.GPSLatitude, out picture_latitude);
                    latitude = picture_latitude[0] + picture_latitude[1] / 60 + picture_latitude[2] / 3600;
                    longitude = picture_longitude[0] + picture_longitude[1] / 60 + picture_longitude[2] / 3600;
                   // Response.Write( "Latitude :" + latitude + " Longitude: " + longitude);
                    coord_ret = new Double[2];
                    coord_ret[0] = latitude;
                    coord_ret[1] = longitude;
                    return (coord_ret);
                }
            }
            catch (Exception e)
            {
                Response.Write(e.ToString());
                return (null);
            }

           
        }
        */
        private void azureConn()
        {
            // try Azure db
            // Create a connection string for the master database
            SqlConnectionStringBuilder connString1Builder;
            connString1Builder = new SqlConnectionStringBuilder();
            connString1Builder.DataSource = dataSource;
            connString1Builder.InitialCatalog = "master";
            connString1Builder.Encrypt = true;
            connString1Builder.TrustServerCertificate = false;
            connString1Builder.UserID = userName;
            connString1Builder.Password = password;

        }

        
    }


    

   
}

 