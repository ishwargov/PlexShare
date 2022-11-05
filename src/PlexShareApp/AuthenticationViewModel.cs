using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.PeopleService.v1.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PlexShareApp;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;
using System.Windows.Controls;
using System.Diagnostics;

namespace AuthViewModel;

public class AuthenticationViewModel
{
    private const string clientId = "1045131640492-c68nvtdorbftv7ejvkoh4h8flo7tl2q9.apps.googleusercontent.com";
    private const string clientSecret = "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";
    const string authorizationEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";

    // User Information
    string userName = "";
    string userEmail = "";
    string imageName = "";

    public AuthenticationViewModel()
    {
    }
    public static string EncodeInputBuffer(byte[] buffer)
    {
        string base64 = Convert.ToBase64String(buffer);
        // Converts base64 to base64url.
        base64 = base64.Replace("+", "-");
        base64 = base64.Replace("/", "_");
        // Strips padding.
        base64 = base64.Replace("=", "");

        return base64;
    }
    public static byte[] Sha256(string inputStirng)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(inputStirng);
        SHA256Managed sha256 = new SHA256Managed();
        return sha256.ComputeHash(bytes);
    }
    public static string GenerateDataBase(uint length)
    {
        RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
        byte[] bytes = new byte[length];
        rng.GetBytes(bytes);
        return EncodeInputBuffer(bytes);
    }
  
    public async Task<List<string>> AuthenticateUser()
    {
        string state = GenerateDataBase(32);
        string code_verifier = GenerateDataBase(32);
        string code_challenge = EncodeInputBuffer(Sha256(code_verifier));
        const string code_challenge_method = "S256";
        string redirectURI = string.Format("http://{0}:{1}/", IPAddress.Loopback, "8080");
        List<string> result = new List<string>();
        
        var http = new HttpListener();
        http.Prefixes.Add(redirectURI);
        Debug.WriteLine("Listening..");
        http.Start();

        string authorizationRequest = string.Format("{0}?response_type=code&scope=openid%20email%20profile&redirect_uri={1}&client_id={2}&state={3}&code_challenge={4}&code_challenge_method={5}",
                authorizationEndpoint,
                Uri.EscapeDataString(redirectURI),
                clientId,
                state,
                code_challenge,
                code_challenge_method);
        string vr = "https://www.google.com/";

        Debug.WriteLine("Debugging: " + vr);
        try
        {
            Process.Start(new ProcessStartInfo(authorizationRequest) { UseShellExecute = true});
        } 
        catch (System.ComponentModel.Win32Exception noBrowser)
        {
            if (noBrowser.ErrorCode == -2147467259)
            {
                Debug.WriteLine("Error finding browser");
            }
        }
        catch (Exception other)
        {
            Debug.WriteLine(other.Message);
        }

        var context = await http.GetContextAsync();
        var response = context.Response;
        string responseString = string.Format("<html><head><meta http-equiv='refresh' content='10;url=https://google.com'></head><body>Authentication is complete! You can return to your app.</body></html>");
        var buffer = Encoding.UTF8.GetBytes(responseString);
        response.ContentLength64 = buffer.Length;
        var responseOutput = response.OutputStream;

        Task responseTask = responseOutput.WriteAsync(buffer, 0, buffer.Length).ContinueWith((task) =>
        {
            responseOutput.Close();
            http.Stop();
            Debug.WriteLine("HTTP server stopped.");
        });

        if (context.Request.QueryString.Get("error") != null)
        {
            Debug.WriteLine(String.Format("OAuth authorization error: {0}.", context.Request.QueryString.Get("error")));
            result.Add("false");
            return result;
        }

        if (context.Request.QueryString.Get("code") == null || context.Request.QueryString.Get("state") == null)
        {
            Debug.WriteLine("Malformed authorization response. " + context.Request.QueryString);
            result.Add("false");
            return result;
        }

        var code = context.Request.QueryString.Get("code");
        var incoming_state = context.Request.QueryString.Get("state");

        if (incoming_state != state)
        {
            Debug.WriteLine(String.Format("Received request with invalid state ({0})", incoming_state));
            result.Add("false");
            return result;
        }

        Task task = Task.Factory.StartNew(() => GetUserData(code, code_verifier, redirectURI));
        Task.WaitAll(task);

        Debug.WriteLine("Above tasks are finished!\n" + userName + " " + userEmail + " " + imageName);
        result.Add("true");

        while(userName == "" || userEmail == "" || imageName == "" )
        {
            Thread.Sleep(100);
        }
        result.Add(userName);
        result.Add(userEmail);
        result.Add(imageName);
        return result;
    }

    async void GetUserData(string code, string code_verifier, string redirectURI)
    {
        // builds the  request
        string tokenRequestURI = "https://www.googleapis.com/oauth2/v4/token";
        string tokenRequestBody = string.Format("code={0}&redirect_uri={1}&client_id={2}&code_verifier={3}&client_secret={4}&scope=&grant_type=authorization_code",
            code,
            System.Uri.EscapeDataString(redirectURI),
            clientId,
            code_verifier,
            clientSecret
            );

        string tokenDisconnectUri = "https://www.googleapis.com/oauth2/v4/token=";
        // sends the request
        HttpWebRequest tokenRequest = (HttpWebRequest)WebRequest.Create(tokenRequestURI);
        tokenRequest.Method = "POST";
        tokenRequest.ContentType = "application/x-www-form-urlencoded";
        tokenRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
        byte[] _byteVersion = Encoding.ASCII.GetBytes(tokenRequestBody);
        tokenRequest.ContentLength = _byteVersion.Length;
        Stream stream = tokenRequest.GetRequestStream();
        await stream.WriteAsync(_byteVersion, 0, _byteVersion.Length);
        stream.Close();

        try
        {
            // gets the response
            WebResponse tokenResponse = await tokenRequest.GetResponseAsync();

            using (StreamReader reader = new StreamReader(tokenResponse.GetResponseStream()))
            {
                // reads response body
                string responseText = await reader.ReadToEndAsync();
                Debug.WriteLine(responseText);
                // converts to dictionary
                Dictionary<string, string> tokenEndpointDecoded = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseText);
                string access_token = tokenEndpointDecoded["access_token"];
                userinfoCall(access_token);

                tokenDisconnectUri += access_token;
                HttpWebRequest tokenDisconnect = (HttpWebRequest)WebRequest.Create(tokenDisconnectUri);
            }
        }
        catch (WebException ex)
        {
            if (ex.Status == WebExceptionStatus.ProtocolError)
            {
                var response = ex.Response as HttpWebResponse;
                if (response != null)
                {
                    Debug.WriteLine("HTTP: " + response.StatusCode);
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        // reads response body
                        string responseText = await reader.ReadToEndAsync();
                        Debug.WriteLine(responseText);
                    }
                }
            }
        }

    }

    void DownloadImage(string url)
    {
        imageName = "UserProfilePicture.png";
        string localFileName = "../../../Resources/" + imageName;
        WebClient webClient = new WebClient();
        webClient.DownloadFile(url, localFileName);
    }

    async void userinfoCall(string access_token)
    {
        // builds the  request
        string userinfoRequestURI = "https://www.googleapis.com/oauth2/v3/userinfo";

        // sends the request
        HttpWebRequest userinfoRequest = (HttpWebRequest)WebRequest.Create(userinfoRequestURI);
        userinfoRequest.Method = "GET";
        userinfoRequest.Headers.Add(string.Format("Authorization: Bearer {0}", access_token));
        userinfoRequest.ContentType = "application/x-www-form-urlencoded";
        userinfoRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";

        // gets the response
        WebResponse userinfoResponse = await userinfoRequest.GetResponseAsync();
        using (StreamReader userinfoResponseReader = new StreamReader(userinfoResponse.GetResponseStream()))
        {
            // reads response body
            string userinfoResponseText = await userinfoResponseReader.ReadToEndAsync();
            System.Diagnostics.Debug.WriteLine("USER INFO:\n" + userinfoResponseText);
            var json = JObject.Parse(userinfoResponseText);

            // Storing Data from Json file received
            userName = json["name"].ToString();
            userEmail = json["email"].ToString();
            string imageURL = json["picture"].ToString();
            DownloadImage(imageURL);
        }
    }
}
