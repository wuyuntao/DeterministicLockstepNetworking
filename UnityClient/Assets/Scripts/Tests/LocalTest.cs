using UnityEngine;
using System.Collections;
using DeterministicLockstepNetworking;

public class LocalTest : MonoBehaviour
{
    SessionManager sessionManager;

    float elapsedTime;

    uint currentTicks;

    void Start()
    {
        this.sessionManager = new SessionManager(1);

        CubeController.Initialize(sessionManager.FindSession(1));
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= 0.1f)
        {
            currentTicks++;

            var frame = this.sessionManager.FetchSendCommands();
            var commands = frame != null ? frame.Commands : null;
            this.sessionManager.ReceiveCommands(new CommandFrame(currentTicks, commands));

            elapsedTime -= 0.1f;
        }
    }

    void OnGUI()
    {
        if (Event.current.isKey)
        {
            CubeController.Command command = 0;
            switch (Event.current.keyCode)
            {
                case KeyCode.UpArrow:
                    command = CubeController.Command.Forward;
                    break;

                case KeyCode.DownArrow:
                    command = CubeController.Command.Back;
                    break;

                case KeyCode.LeftArrow:
                    command = CubeController.Command.Left;
                    break;

                case KeyCode.RightArrow:
                    command = CubeController.Command.Right;
                    break;

                default:
                    break;
            }

            if (command != 0)
            {
                this.sessionManager.SendCommand((uint)command);

                Debug.Log(string.Format("NewCommand: {0}", command));
            }
        }
    }
}
