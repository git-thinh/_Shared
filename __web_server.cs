using System;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;
using System.Linq;

namespace System.Web
{
    public class WebServer : HttpServer
    {
        readonly Action<Tuple<string, COMMANDS, string>> __action;
        public WebServer(Action<Tuple<string, COMMANDS, string>> action) => __action = action;

        protected override void ProcessRequest(HttpListenerContext Context)
        {
            HttpListenerRequest Request = Context.Request;
            HttpListenerResponse Response = Context.Response;
            string url = Request.RawUrl;
            string htm = "";
            byte[] bOutput;
            Stream OutputStream = Response.OutputStream;

            switch (Request.HttpMethod)
            {
                case "POST":
                    #region
                    htm = "{}";
                    //StreamReader stream = new StreamReader(Request.InputStream);
                    //string data = stream.ReadToEnd();
                    //data = HttpUtility.UrlDecode(data);
                    // do something ...
                    bOutput = Encoding.UTF8.GetBytes(htm);
                    Response.ContentType = "application/json; charset=utf-8";
                    Response.ContentLength64 = bOutput.Length;
                    OutputStream.Write(bOutput, 0, bOutput.Length);
                    OutputStream.Close();
                    #endregion
                    break;
                case "GET":
                    #region 

                    string _type = "text/plain; charset=utf-8";
                    string command = Request.QueryString["cmd"];
                    string input = Request.QueryString["input"];

                    switch (url)
                    {
                        case "/favicon.ico":
                            OutputStream.Close();
                            return;
                        default:
                            if (!string.IsNullOrEmpty(command) && !string.IsNullOrEmpty(input))
                            {
                                command = command.ToUpper();
                                var ls = Enum.GetValues(typeof(COMMANDS)).Cast<COMMANDS>()
                                    .Select(v => new Tuple<string, COMMANDS>(v.ToString().ToUpper(), v)).ToList();
                                var c = ls.Where(x => x.Item1 == command).Take(1).SingleOrDefault();
                                if (c != null)
                                {
                                    new Thread(new ParameterizedThreadStart((o) =>
                                    __action((Tuple<string, COMMANDS, string>)o)))
                                        .Start(new Tuple<string, COMMANDS, string>(string.Empty, c.Item2, input));
                                }
                            }
                            break;
                    }

                    bOutput = Encoding.UTF8.GetBytes(htm);
                    Response.ContentType = _type;
                    Response.ContentLength64 = bOutput.Length;
                    OutputStream.Write(bOutput, 0, bOutput.Length);
                    OutputStream.Close();

                    #endregion
                    break;
            }
        }
    }

    /// <summary>
    /// Wrapper class for the HTTPListener to allow easier access to the
    /// server, for start and stop management and event routing of the actual
    /// inbound requests.
    /// </summary>
    public class HttpServer
    {
        protected HttpListener Listener;
        protected bool IsStarted = false;
        public event delReceiveWebRequest ReceiveWebRequest;
        public HttpServer() { }

        /// <summary>
        /// Starts the Web Service
        /// </summary>
        /// <param name="UrlBase">
        /// A Uri that acts as the base that the server is listening on.
        /// Format should be: http://127.0.0.1:8080/ or http://127.0.0.1:8080/somevirtual/
        /// Note: the trailing backslash is required! For more info see the
        /// HttpListener.Prefixes property on MSDN.
        /// </param>
        public void Start(string UrlBase)
        {
            // *** Already running - just leave it in place
            if (this.IsStarted)
                return;

            if (this.Listener == null)
            {
                this.Listener = new HttpListener();
            }

            this.Listener.Prefixes.Add(UrlBase);
            this.IsStarted = true;
            this.Listener.Start();

            IAsyncResult result = this.Listener.BeginGetContext(new AsyncCallback(WebRequestCallback), this.Listener);
        }

        /// <summary>
        /// Shut down the Web Service
        /// </summary>
        public void Stop()
        {
            if (Listener != null)
            {
                this.Listener.Close();
                this.Listener = null;
                this.IsStarted = false;
            }
        }

        protected void WebRequestCallback(IAsyncResult result)
        {
            if (this.Listener == null)
                return;

            // Get out the context object
            HttpListenerContext context = this.Listener.EndGetContext(result);

            // *** Immediately set up the next context
            this.Listener.BeginGetContext(new AsyncCallback(WebRequestCallback), this.Listener);
            if (this.ReceiveWebRequest != null)
                this.ReceiveWebRequest(context);
            this.ProcessRequest(context);
        }

        /// <summary>
        /// Overridable method that can be used to implement a custom hnandler
        /// </summary>
        /// <param name="Context"></param>
        protected virtual void ProcessRequest(HttpListenerContext Context)
        {
        }
    }
    public delegate void delReceiveWebRequest(HttpListenerContext Context);
}
