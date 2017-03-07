using System;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using NetMQ;
using NetMQ.Sockets;
using Afrl.Lmcp;

/// <summary>
/// Subscribes to IMPACT Hub for vehicle state updates.
/// </summary>
public class ImpactHubSubscriber : MonoBehaviour
{
    private NetworkManagerFSM m_networkManagerFSM;
    private bool m_stopThread;
    private Thread m_thread;
    private object m_lock = new object();
    private Queue<byte[]> m_messages = new Queue<byte[]>();

    public Vector3 mapCenter = new Vector3(-117.1611f, 0, 32.7157f); // San Diego
    public string publisherAddress = "tcp://127.0.0.1:5556";
    public float messagePollRate = 1;
    public int timeoutSeconds = 2;
    public string[] subscribeTopics =
    {
        "lmcp:CMASI:GroundVehicleState",
        "lmcp:CMASI:SurfaceVehicleState",
        "lmcp:CMASI:AirVehicleState"
    };

    public delegate void OnImpactMessageReceivedDelegate(ILmcpObject impactMessage);
    public delegate void OnImpactHubSubscriberStartedDelegate(Vector3 mapCenter);
    public delegate void OnImpactHubSubscriberStoppedDelegate();

    public OnImpactMessageReceivedDelegate onImpactMessageReceivedDelegate;
    public OnImpactHubSubscriberStartedDelegate onImpactHubSubscriberStartedDelegate;
    public OnImpactHubSubscriberStoppedDelegate onImpactHubSubscriberStoppedDelegate;

    private bool CheckPreconditions()
    {
        var manager = MyNetworkManager.Instance;

        if (manager == null)
        {
            return false;
        }

        m_networkManagerFSM = manager.GetComponent<NetworkManagerFSM>();
        m_networkManagerFSM.AddOnEnterListener(StartSubscriber, NetworkManagerState.HostConnected);
        m_networkManagerFSM.AddOnExitListener(StopSubscriber, NetworkManagerState.HostConnected);

        return true;
    }

    protected virtual void Start()
    {
        if (!CheckPreconditions())
        {
            enabled = false;
        }
    }
    
    protected virtual void Update()
    {
        if (m_stopThread)
        {
            StopSubscriber();
        }
    }

    private void OnApplicationQuit()
    {
        StopSubscriber();
    }

    private void StartSubscriber()
    {
        if (m_thread == null)
        {
            lock (m_lock) m_stopThread = false;
            m_thread = new Thread(SubscribeToImpactHub);
            m_thread.Start();
            InvokeRepeating("ProcessImpactMessages", messagePollRate, messagePollRate);
        }

        if (onImpactHubSubscriberStartedDelegate != null) onImpactHubSubscriberStartedDelegate(mapCenter);
    }

    private void StopSubscriber()
    {
        if (onImpactHubSubscriberStoppedDelegate != null) onImpactHubSubscriberStoppedDelegate();

        if (m_thread != null)
        {
            lock (m_lock) m_stopThread = true;
            CancelInvoke();
            m_thread.Join();
            m_thread = null;
        }

        m_messages.Clear();
    }

    private void SubscribeToImpactHub()
    {
        AsyncIO.ForceDotNet.Force();

        string topic;
        byte[] message;
        bool isConnected;

        var timeout = new TimeSpan(0, 0, timeoutSeconds);
        var subSocket = new SubscriberSocket();

        subSocket.Connect(publisherAddress);

        foreach (var i in subscribeTopics)
        {
            subSocket.Subscribe(i);
        }

        while (!m_stopThread)
        {
            isConnected = subSocket.TryReceiveFrameString(timeout, out topic);
            isConnected = subSocket.TryReceiveFrameBytes(timeout, out message);

            if (!isConnected)
            {
                m_stopThread = true;
            }
            else if (message != null)
            {
                lock (m_lock) m_messages.Enqueue(message);
            }
        }

        subSocket.Close();
        subSocket.Dispose();
        NetMQConfig.Cleanup();
    }

    private void ProcessImpactMessages()
    {
        byte[] message;
        while (m_messages.Count > 0)
        {
            lock (m_lock) message = m_messages.Dequeue();
            var impactObject = LmcpFactory.GetObject(message);
            if (onImpactMessageReceivedDelegate != null) onImpactMessageReceivedDelegate(impactObject);
        }
    }
}