/******************************************************************************
 * Filename    = AuthenticationView.xaml.cs
 *
 * Author      = Parichita Das
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareApp
 *
 * Description = ViewModel for the Authentication Module which will authorize the app to useclient information.
 * 
 *****************************************************************************/

using Newtonsoft.Json;
using System;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace AuthViewModel;

public class AuthenticationViewModel
{
    // Our App Client information required for OAuth authentication
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

    /// <summary>
    /// This will be the main function called by the View to authenticate the user
    /// </summary>
    /// <returns>string with a bool denoting success of Authentication</returns>
    /// <returns>User Name</returns>
    /// <returns>Smail ID</returns>
    /// <returns>Profile Picture URL</returns>
    public async Task<List<string>> AuthenticateUser()
    {
        Trace.WriteLine("[UX] Creating State and Redirect URI on port 8080");
        // Creating state and redirect URI using port 8080 on Loopback address
        string state = GenerateDataBase(32);
        string code_verifier = GenerateDataBase(32);
        string code_challenge = EncodeInputBuffer(Sha256(code_verifier));
        const string code_challenge_method = "S256";
        string redirectURI = string.Format("http://{0}:{1}/", IPAddress.Loopback, "8080");
        List<string> result = new List<string>();

        Trace.WriteLine("[UX] Creating HTTP Listener");
        // Creating HTTP listener
        var http = new HttpListener();
        http.Prefixes.Add(redirectURI);
        Trace.WriteLine("[UX] Listening on HTTP");
        http.Start();

        Trace.WriteLine("[UX] Sending Authorization Request");
        // Creating an authorization request for OAuth 2.0
        string authorizationRequest = string.Format("{0}?response_type=code&scope=openid%20email%20profile&redirect_uri={1}&client_id={2}&state={3}&code_challenge={4}&code_challenge_method={5}",
                authorizationEndpoint,
                Uri.EscapeDataString(redirectURI),
                clientId,
                state,
                code_challenge,
                code_challenge_method);

        // Trying to open the request in a browser  
        try
        {
            Process.Start(new ProcessStartInfo(authorizationRequest) { UseShellExecute = true});
        } 
        catch (System.ComponentModel.Win32Exception noBrowser)
        {
            if (noBrowser.ErrorCode == -2147467259)
            {
                Trace.WriteLine("[UX] Error finding browser");
            }
        }
        catch (Exception other)
        {
            Trace.WriteLine(other.Message);
        }

        // Sending HTTP request to browser and displaying response
        var taskData = http.GetContextAsync();

        // If no response is recorded, then we do a timeout after 3 minutes.
        // This may happen because of closing the browser
        if (await Task.WhenAny(taskData, Task.Delay(180000)) != taskData)
        {
            Trace.WriteLine("[UX] Timeout occurred before getting response");
            http.Stop();
            result.Add("false");
            return result;
        }

        var context = taskData.Result;
        var response = context.Response;
        string responseString = string.Format("<html><head><meta http-equiv='refresh' content='10;url=https://google.com'></head><body><center><h1>Authentication is complete! You will be redirected to your app in a few seconds!</h1></center></body></html>");
        var buffer = Encoding.UTF8.GetBytes(responseString);
        response.ContentLength64 = buffer.Length;
        var responseOutput = response.OutputStream;
        Task responseTask = responseOutput.WriteAsync(buffer, 0, buffer.Length).ContinueWith((task) =>
        {
            responseOutput.Close();
            http.Stop();
            Trace.WriteLine("[UX] HTTP server stopped.");
        });

        // In case of errors, return to Sign In window
        if (context.Request.QueryString.Get("error") != null)
        {
            Trace.WriteLine(String.Format("OAuth authorization error: {0}.", context.Request.QueryString.Get("error")));
            result.Add("false");
            return result;
        }

        if (context.Request.QueryString.Get("code") == null || context.Request.QueryString.Get("state") == null)
        {
            Trace.WriteLine("[UX] Malformed authorization response. " + context.Request.QueryString);
            result.Add("false");
            return result;
        }

        // Extracting code and state
        var code = context.Request.QueryString.Get("code");
        var incoming_state = context.Request.QueryString.Get("state");

        // Comparing state to expected value
        if (incoming_state != state)
        {
            Trace.WriteLine(String.Format("Received request with invalid state ({0})", incoming_state));
            result.Add("false");
            return result;
        }

        Trace.WriteLine("[UX] No Errors Occurred");
        // A new thread to wait for the GetUserData to get all required information
        Task task = Task.Factory.StartNew(() => GetUserData(code, code_verifier, redirectURI));
        Task.WaitAll(task);

        result.Add("true");
        while(userName == "" || userEmail == "" || imageName == "" )
        {
            // Thread sleeps until information is received
            Thread.Sleep(100);
        }
        result.Add(userName);
        result.Add(userEmail);
        result.Add(imageName);
        return result;
    }

    /// <summary>
    /// Creating a non-padded base64 URL encoding
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
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

    /// <summary>
    /// For getting the SHA256 hashing of the inputString
    /// </summary>
    /// <param name="inputString"></param>
    /// <returns></returns>
    public static byte[] Sha256(string inputString)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(inputString);
        SHA256Managed sha256 = new SHA256Managed();
        return sha256.ComputeHash(bytes);
    }

    /// <summary>
    /// Generating a random cryptographic number oh length 32
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public static string GenerateDataBase(uint length)
    {
        RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
        byte[] bytes = new byte[length];
        rng.GetBytes(bytes);
        return EncodeInputBuffer(bytes);
    }

    /// <summary>
    /// This function is responsible for connecting to OAuth Client and requesting token to receive Profile Information of User who has authenticated.
    /// </summary>
    /// <param name="code"></param>
    /// <param name="code_verifier"></param>
    /// <param name="redirectURI"></param>
    async void GetUserData(string code, string code_verifier, string redirectURI)
    {
        Trace.WriteLine("[UX] Getting Data From User");
        // Building the  request
        string tokenRequestURI = "https://www.googleapis.com/oauth2/v4/token";
        string tokenRequestBody = string.Format("code={0}&redirect_uri={1}&client_id={2}&code_verifier={3}&client_secret={4}&scope=&grant_type=authorization_code",
            code,
            Uri.EscapeDataString(redirectURI),
            clientId,
            code_verifier,
            clientSecret
            );

        // Sending the request
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
            // Getting the response
            WebResponse tokenResponse = await tokenRequest.GetResponseAsync();

            using (StreamReader reader = new StreamReader(tokenResponse.GetResponseStream()))
            {
                // Reading response body
                string responseText = await reader.ReadToEndAsync();
                Trace.WriteLine(responseText);
                // Converting to dictionary
                Dictionary<string, string> tokenEndpointDecoded = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseText);
                string access_token = tokenEndpointDecoded["access_token"];
                UserInfoCall(access_token);
            }
        }
        catch (WebException ex)
        {
            if (ex.Status == WebExceptionStatus.ProtocolError)
            {
                var response = ex.Response as HttpWebResponse;
                if (response != null)
                {
                    Trace.WriteLine("[UX] HTTP: " + response.StatusCode);
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        // Reading response body
                        string responseText = await reader.ReadToEndAsync();
                        Trace.WriteLine(responseText);
                    }
                }
            }
        }

    }

    /// <summary>
    /// This will finally enable us to use the token to extract the required information
    /// </summary>
    /// <param name="access_token"></param>
    async void UserInfoCall(string access_token)
    {
        // Building the  request
        string userinfoRequestURI = "https://www.googleapis.com/oauth2/v3/userinfo";

        // Sending the request
        HttpWebRequest userinfoRequest = (HttpWebRequest)WebRequest.Create(userinfoRequestURI);
        userinfoRequest.Method = "GET";
        userinfoRequest.Headers.Add(string.Format("Authorization: Bearer {0}", access_token));
        userinfoRequest.ContentType = "application/x-www-form-urlencoded";
        userinfoRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";

        // Getting the response
        WebResponse userinfoResponse = await userinfoRequest.GetResponseAsync();
        using (StreamReader userinfoResponseReader = new StreamReader(userinfoResponse.GetResponseStream()))
        {
            // Reading response body
            string userinfoResponseText = await userinfoResponseReader.ReadToEndAsync();
            Trace.WriteLine("[UX] USER INFO:\n" + userinfoResponseText);
            var json = JObject.Parse(userinfoResponseText);

            // Storing Data from Json file received
            userName = json["name"].ToString();
            userEmail = json["email"].ToString();
            imageName = json["picture"].ToString();
        }
    }
}
