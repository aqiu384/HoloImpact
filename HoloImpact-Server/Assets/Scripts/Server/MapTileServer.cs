using System;
using System.Net;
using System.IO;
using System.Threading;
using UnityEngine;

/// <summary>
/// Rudimentary tile server that sends terrain data to clients.
/// </summary>
public class MapTileServer : MonoBehaviour
{
    private NetworkManagerFSM m_networkManagerFSM;
    private bool m_stopThread;
    private Thread m_thread;
    private object m_lock = new object();

    public string listenAddress = "172.16.80.1";
    public int listenPort = 7775;
    public string mapTilesPath = "./MapTiles/";
    public int timeoutMilliseconds = 1000;
    public int bufferSize = 1024 * 8;

    // public LatLong mapCenter { get; private set; }

    private bool CheckPreconditions()
    {
        var manager = MyNetworkManager.Instance;

        if (manager == null)
        {
            return false;
        }

        m_networkManagerFSM = manager.GetComponent<NetworkManagerFSM>();
        m_networkManagerFSM.AddOnEnterListener(StartServer, NetworkManagerState.HostConnected);
        m_networkManagerFSM.AddOnExitListener(StopServer, NetworkManagerState.HostConnected);

        return true;
    }

    protected virtual void Start()
    {
        if (!CheckPreconditions())
        {
            enabled = false;
        }
    }

    private void OnApplicationQuit()
    {
        StopServer();
    }

    public void StartServer()
    {
        if (m_thread == null)
        {
            lock (m_lock) m_stopThread = false;
            m_thread = new Thread(ListenForMapClients);
            m_thread.Start();
        }
    }

    public void StopServer()
    {
        if (m_thread != null)
        {
            lock (m_lock) m_stopThread = true;
            m_thread.Join();
            m_thread = null;
        }

        enabled = false;
    }

    private void ListenForMapClients()
    {
        var listener = new HttpListener();
        listener.Prefixes.Add(string.Format("http://{0}:{1}/", listenAddress, listenPort));
        listener.Start();

        while (!m_stopThread)
        {
            var context = listener.BeginGetContext(new AsyncCallback(ProcessMapTileRequest), listener);
            context.AsyncWaitHandle.WaitOne(timeoutMilliseconds, true);
        }

        listener.Close();
    }

    private void ProcessMapTileRequest(IAsyncResult result)
    {
        var listener = (HttpListener)result.AsyncState;
        var context = listener.EndGetContext(result);
        var filename = Path.Combine(mapTilesPath, context.Request.Url.AbsolutePath.Substring(1));
        var response = context.Response;

        if (File.Exists(filename))
        {
            try
            {
                Stream input = new FileStream(filename, FileMode.Open);

                response.ContentType = "application/octet-stream";
                response.ContentLength64 = input.Length;
                response.AddHeader("Date", DateTime.Now.ToString("r"));
                response.AddHeader("Last-Modified", File.GetLastWriteTime(filename).ToString("r"));

                byte[] buffer = new byte[bufferSize];
                int len;
                while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    context.Response.OutputStream.Write(buffer, 0, len);
                }

                input.Close();
                response.StatusCode = (int)HttpStatusCode.OK;
                response.OutputStream.Flush();
            }
            catch (Exception)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
        }
        else
        {
            response.StatusCode = (int)HttpStatusCode.NotFound;
        }

        response.OutputStream.Close();
    }
}