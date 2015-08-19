using UnityEngine;
using System.Collections;
using DeterministicLockstepNetworking;

public class CubeController : MonoBehaviour
{
    public enum Command : uint
    {
        Forward = 1,
        Back,
        Left,
        Right,
    }

    public float thrust = 20;

    Session session;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (session != null)
        {
            var commands = session.FetchCommands();
            if (commands != null)
            {
                foreach (var command in commands)
                {
                    ReplayCommand((Command)command.CommandId);
                }
            }
        }
    }

    void ReplayCommand(Command command)
    {
        var rb = GetComponent<Rigidbody>();

        switch (command)
        {
            case Command.Forward:
                {
                    var force = Vector3.forward * thrust + Vector3.up * thrust;
                    Debug.Log(string.Format("{0} {1}", command, force));

                    rb.AddForce(force);
                }
                break;

            case Command.Back:
                {
                    var force = -1 * Vector3.forward * thrust + Vector3.up * thrust;
                    Debug.Log(string.Format("{0} {1}", command, force));

                    rb.AddForce(force);
                }
                break;

            case Command.Left:
                {
                    var torque = Vector3.left * thrust + Vector3.up * thrust;
                    Debug.Log(string.Format("{0} {1}", command, torque));

                    rb.AddTorque(torque);
                    rb.AddForce(torque);
                }
                break;

            case Command.Right:
                {
                    var torque = Vector3.right * thrust + Vector3.up * thrust;
                    Debug.Log(string.Format("{0} {1}", command, torque));

                    rb.AddTorque(torque);
                    rb.AddForce(torque);
                }
                break;

            default:
                Debug.LogError(string.Format("Unknown command: {0}", command));
                break;
        }
    }

    internal static void Initialize(Session session)
    {

        var resource = Resources.Load<GameObject>("Models/Cube/Cube");
        Debug.Log(string.Format("{0} {1}", session == null, resource == null));

        var cube = GameObject.Instantiate(resource);
        var controller = cube.GetComponent<CubeController>();
        controller.session = session;
    }
}
