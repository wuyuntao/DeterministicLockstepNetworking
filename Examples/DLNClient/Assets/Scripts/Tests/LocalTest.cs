using DeterministicLockstepNetworking;
using DLNSchema;
using UnityEngine;

public class LocalTest : MonoBehaviour
{
    SessionManager sessionManager;

    float elapsedTime;

    uint currentTicks;

    void Start()
    {
        this.sessionManager = new SessionManager();
        var session1 = this.sessionManager.AddSession(1);
        var session2 = this.sessionManager.AddSession(2);
        var session3 = this.sessionManager.AddSession(3);

        var cube1 = CubeController.Initialize(session1);
        CubeController.Initialize(session2);
        CubeController.Initialize(session3);

        cube1.ChangeColor(true);
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= 0.1f)
        {
            currentTicks++;

            var commands = this.sessionManager.FetchCommands();

            this.sessionManager.ReceiveCommands(this.sessionManager.CurrentTicks + 1, commands);

            elapsedTime -= 0.1f;
        }
    }

    void OnGUI()
    {
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
                var command = new Command((uint)commandId, 1);

                this.sessionManager.SendCommand(command);

                //Debug.Log(string.Format("NewCommand: {0}", commandId));
            }
        }
    }
}
