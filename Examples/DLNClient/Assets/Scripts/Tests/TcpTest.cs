using DeterministicLockstepNetworking;
using DLNSchema;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TcpTest : MonoBehaviour, IClientHandler
{
    SessionManager sessionManager;

    Client client;

    List<CubeController> cubes = new List<CubeController>();

    bool connected;

    float elapsedTime;

    void Start()
    {
        this.sessionManager = new SessionManager();
        var session1 = this.sessionManager.AddSession(1);
        var session2 = this.sessionManager.AddSession(2);
        var session3 = this.sessionManager.AddSession(3);

        this.cubes.Add(CubeController.Initialize(session1));
        this.cubes.Add(CubeController.Initialize(session2));
        this.cubes.Add(CubeController.Initialize(session3));

        this.client = new Client("192.168.0.103", 4000, this);
    }

    // Update is called once per frame
    void Update()
    {
        if (connected)
        {
            ChangePlayerCubeColor();

            connected = false;
        }

        this.elapsedTime += Time.deltaTime;

        if (this.elapsedTime > 0.05f)
        {
            SynchronizeCommandFrames();

            this.elapsedTime -= 0.05f;
        }
    }

    private void ChangePlayerCubeColor()
    {
        var cube = this.cubes.Find(c => c.SessionId == this.client.SessionId);
        if (cube == null)
        {
            throw new InvalidOperationException("Cube not exist");
        }

        cube.ChangeColor(true);
    }

    private void SynchronizeCommandFrames()
    {
        var frames = this.client.FetchCommandFrames();

        if (frames != null && frames.Length > 0)
        {
            foreach (var frame in frames)
            {
                if (!this.sessionManager.ReceiveCommands(frame.Ticks, frame.Commands))
                {
                    Debug.LogWarning(string.Format("Received unmatched frame: {0} vs {1}", frame.Ticks, this.sessionManager.CurrentTicks));
                }
            }
        }
    }

    void OnGUI()
    {
        if (this.client.SessionId == 0)
            return;

        if (Event.current.isKey)
        {
            CubeController.CommandId commandId = 0;
            switch (Event.current.keyCode)
            {
                case KeyCode.UpArrow:
                    commandId = CubeController.CommandId.Forward;
                    break;

                case KeyCode.DownArrow:
                    commandId = CubeController.CommandId.Back;
                    break;

                case KeyCode.LeftArrow:
                    commandId = CubeController.CommandId.Left;
                    break;

                case KeyCode.RightArrow:
                    commandId = CubeController.CommandId.Right;
                    break;

                default:
                    break;
            }

            if (commandId != 0)
            {
                var command = new Command((uint)commandId, this.client.SessionId);

                this.client.SendCommand(this.sessionManager.CurrentTicks + 1, command);
            }
        }
    }

    #region IClientHandler

    void IClientHandler.Log(string msg, params object[] args)
    {
        Debug.Log(string.Format(msg, args));
    }

    void IClientHandler.LogError(string msg, params object[] args)
    {
        Debug.LogError(string.Format(msg, args));
    }

    void IClientHandler.OnConnect()
    {
        this.connected = true;
    }

    void IClientHandler.OnSync()
    {
    }

    #endregion
}
