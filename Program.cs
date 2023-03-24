using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace GooglePlaces
{
    class Program
    {
        public static string GPPREFIX = @"https://maps.googleapis.com/maps/api/place/textsearch/json";
        
        //My Own Key. Will have to get your own if you want to use this down the line

        public static string GPKEY = @"";
        
        public static string GPURL = "";
        static void Main(string[] args)
        {
            LongLat Result = new LongLat();
            Console.WriteLine("Please input the long thing.");
            Result.Long = Console.ReadLine();
            Console.WriteLine("Please input the lat thing.");
            Result.Lat = Console.ReadLine();
            foreach (string item in RestruantListing(Result))
            {
                Console.WriteLine(item);
            }
            Console.WriteLine("End!");
            Console.ReadLine();
        }
        public static void ResetGooglePlacesKey()
        {
            GPURL = @"https://maps.googleapis.com/maps/api/place/textsearch/json";
        }
        public static void LocalPlacesURL(string lat, string lng, string radius)
        {
            GPURL = GPPREFIX.Replace("textsearch", "nearbysearch") + @"?location=" + lat + "," + lng + "&radius=" + radius + @"&type=restaurant" + GPKEY;
            using (StreamWriter sw = new StreamWriter("output.txt"))
            {
                sw.WriteLine(GPURL);
            }
        }
        public static List<string> RestruantListing(LongLat SearchParams)
        {
            List<string> RestNames = new List<string>();
            ResetGooglePlacesKey();
            LocalPlacesURL(SearchParams.Lat, SearchParams.Long, "1500");
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(GPURL);
            myRequest.Method = "GET";
            WebResponse myResponse = myRequest.GetResponse();
            StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
            string result = sr.ReadToEnd();
            sr.Close();
            myResponse.Close();
            using (StringReader reader = new StringReader(result))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {

                    if (line.Trim().Replace("\"", "").ToLower().Contains("name"))
                    {
                        string input = line.Replace("\"", "").Replace("name :", "").Trim();
                        RestNames.Add(input.Substring(0, input.Length - 1));
                    }
                }
            }
            return RestNames;
        }
        public static LongLat PlacesID()
        {
            LongLat ReturnLongLat = new LongLat();
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(GPURL);
            myRequest.Method = "GET";
            WebResponse myResponse = myRequest.GetResponse();
            StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
            string result = sr.ReadToEnd();
            sr.Close();
            myResponse.Close();
            using (StringReader reader = new StringReader(result))
            {
                string line;
                Boolean reading = false;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Trim() == "},")
                    {
                        reading = false;
                    }
                    if (reading == true)
                    {
                        if (line.Trim().Contains("lat"))
                        {
                            ReturnLongLat.Lat = line.Replace("\"", "").Replace(":", "").Replace("lat", "").Trim().Replace(",","");
                        }
                        if (line.Trim().Contains("lng"))
                        {
                            ReturnLongLat.Long = line.Replace("\"", "").Replace(":", "").Replace("lng", "").Trim();
                        }
                    }
                    if (line.Trim().Replace("\"", "") == "location : {")
                    {
                        reading = true;
                        Console.WriteLine("Reading set to true");
                    }
                }
            }
            return ReturnLongLat;
        }
        public static void AssignSearchLocationParameter (string input)
        {
            input = input.Replace(" ", "+");
            GPURL = GPPREFIX + @"?query=" + input + GPKEY;
        }
    }
}
