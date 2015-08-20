using DeterministicLockstepNetworking;
using UnityEngine;

public class LocalTest : MonoBehaviour
{
    SessionManager sessionManager;

    float elapsedTime;

    uint currentTicks;

    void Start()
    {
        this.sessionManager = new SessionManager(1);
        var session1 = this.sessionManager.FindSession(1);
        var session2 = this.sessionManager.AddSession(2);
        var session3 = this.sessionManager.AddSession(3);

        CubeController.Initialize(session1, true);
        CubeController.Initialize(session2, false);
        CubeController.Initialize(session3, false);
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
