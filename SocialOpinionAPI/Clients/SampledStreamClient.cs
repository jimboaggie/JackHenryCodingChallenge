﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace SocialOpinionAPI.Clients
{
    public class SampledStreamClient
    {
        private string _ConsumerKey ="";
        private string _ConsumerSecret = "";
        private string _BearerToken = "";

        public event EventHandler StreamDataReceivedEvent;
        public class TweetReceivedEventArgs : EventArgs
        {
            public string StreamDataResponse { get; set; }
        }

        protected void OnStreamDataReceivedEvent(TweetReceivedEventArgs dataReceivedEventArgs)
        {
            if (StreamDataReceivedEvent == null)
                return;
            StreamDataReceivedEvent(this, dataReceivedEventArgs);
        }

        public SampledStreamClient(string consumerKey, string ConsumerSecret, string bearerToken)
        {
            _ConsumerKey = consumerKey;
            _ConsumerSecret = ConsumerSecret;
            _BearerToken = bearerToken;

            if(string.IsNullOrEmpty(_BearerToken) == true)
            {
                //some accounts will not allow you to get the bearer token from an API call
                GetBearerToken();

            }
        }
        private void GetBearerToken()
        {
            //https://dev.twitter.com/oauth/application-only
            //Step 1
            string strBearerRequest = HttpUtility.UrlEncode(_ConsumerKey) + ":" + HttpUtility.UrlEncode(_ConsumerSecret);

            strBearerRequest = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(strBearerRequest));

            //Step 2
            WebRequest request = WebRequest.Create("https://api.twitter.com/oauth2/token");
            request.Headers.Add("Authorization", "Basic " + strBearerRequest);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";

            string strRequestContent = "grant_type=client_credentials";
            byte[] bytearrayRequestContent = System.Text.Encoding.UTF8.GetBytes(strRequestContent);
            System.IO.Stream requestStream = request.GetRequestStream();
            requestStream.Write(bytearrayRequestContent, 0, bytearrayRequestContent.Length);
            requestStream.Close();

            string responseJson = string.Empty;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                System.IO.Stream responseStream = response.GetResponseStream();
                responseJson = new StreamReader(responseStream).ReadToEnd();
            }

            JObject jobjectResponse = JObject.Parse(responseJson);

            _BearerToken = jobjectResponse["access_token"].ToString();
        }

        public void StartStream(string address, int maxTweets, int maxConnectionAttempts)
        {
            int maxTries = maxConnectionAttempts;
            int tried = 0;
            int requestCount = 0;

            while (tried < maxTries)
            {
                tried++;
                try
                {
                    Console.WriteLine("Entered LabsStartSampledStream at:" + DateTime.Now.ToString("F"));
                    int recordsFetch = 0;

                    WebRequest request = WebRequest.Create(address);
                    request.Headers.Add("Authorization", "Bearer " + _BearerToken);
                    request.Method = "GET";
                    request.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";

                    try
                    {
                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                        requestCount++;
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            //stream opened!
                            using (StreamReader str = new StreamReader(response.GetResponseStream()))
                            {
                                // loop through each item in the Filtered Stream API
                                do
                                {
                                    if (recordsFetch == maxTweets)
                                    {
                                        break;
                                    }

                                    string json = str.ReadLine();

                                    if (!string.IsNullOrEmpty(json))
                                    {
                                        // raise an event for a potential client to know we recieved data
                                        OnStreamDataReceivedEvent(new TweetReceivedEventArgs { StreamDataResponse = json });
                                        recordsFetch = recordsFetch + 1;
                                        Console.WriteLine("records fetched:" + recordsFetch + " at " + DateTime.Now.ToString("F"));
                                    }
                                } while (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable()
                                            && !str.EndOfStream && recordsFetch <= maxTweets);
                            }
                            Console.WriteLine("Exited LabsStartSampledStream at:" + DateTime.Now.ToString("F"));
                        }
                        else
                        {
                            Console.WriteLine("response.StatusCode not HttpStatusCode.OK. Currently: " + response.StatusCode + " " + response.StatusDescription);
                        }
                    }
                    catch (WebException ex)
                    {
                        Console.WriteLine(ex.Message);
                        throw;

                    }
                    catch (Exception ex)
                    {
                        // Something more serious happened. like for example you don't have network access
                        // we cannot talk about a server exception here as the server probably was never reached
                        Console.WriteLine(ex.Message);
                        throw;
                    }
                    //we double-check the tries here just so if we aren't "trying" again we don't unnecessarily wait a few seconds
                    if (tried < maxTries)
                        System.Threading.Thread.Sleep(System.TimeSpan.FromSeconds(10));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);

                    if (tried < maxTries)
                    {
                        System.Threading.Thread.Sleep(System.TimeSpan.FromSeconds(10));
                    }
                    else
                    {
                        throw;
                    }



                }
            }
        }

    }
}
