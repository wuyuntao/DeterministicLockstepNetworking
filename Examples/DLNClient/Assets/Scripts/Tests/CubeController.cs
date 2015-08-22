using DeterministicLockstepNetworking;
using DLNSchema;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    public enum CommandId : uint
    {
        Forward = 1,
        Back,
        Left,
        Right,
    }

    public float thrust = 20;

    Session session;

    public static CubeController Initialize(Session session)
    {
        var resource = Resources.Load<GameObject>("Models/Cube/Cube");

        var position = new Vector3(session.SessionId * 10, 10, session.SessionId * 10);
        var cube = (GameObject)GameObject.Instantiate(resource, position, Quaternion.identity);
        cube.name = string.Format("{0}_{1}", cube.name, session.SessionId);

        var controller = cube.AddComponent<CubeController>();
        controller.session = session;

        controller.ChangeColor(false);

        return controller;
    }

    public void ChangeColor(bool isPlayer)
    {
        var materialPath = string.Format("Models/Cube/Materials/{0}", isPlayer ? "Player" : "Friend");
        var material = Resources.Load<Material>(materialPath);
        var renderer = GetComponent<MeshRenderer>();
        
        renderer.material = material;
    }

    void Update()
    {
        if (this.session != null)
        {
            var commands = this.session.FetchCommands();
            if (commands != null)
            {
                foreach (var c in commands)
                {
                    var command = (Command)c;
                    ReplayCommand((CommandId)command.CommandId);
                }
            }
        }
    }

    void ReplayCommand(CommandId command)
    {
        var rb = GetComponent<Rigidbody>();

        switch (command)
        {
            case CommandId.Forward:
                {
                    var force = Vector3.forward * thrust + Vector3.up * thrust;
                    //Debug.Log(string.Format("{0} {1}", command, force));

                    rb.AddForce(force);
                }
                break;

            case CommandId.Back:
                {
                    var force = -1 * Vector3.forward * thrust + Vector3.up * thrust;
                    //Debug.Log(string.Format("{0} {1}", command, force));

                    rb.AddForce(force);
                }
                break;

            case CommandId.Left:
                {
                    var force = Vector3.left * thrust + Vector3.up * thrust;
                    //Debug.Log(string.Format("{0} {1}", command, torque));

                    rb.AddTorque(force);
                    rb.AddForce(force);
                }
                break;

            case CommandId.Right:
                {
                    var force = Vector3.right * thrust + Vector3.up * thrust;
                    //Debug.Log(string.Format("{0} {1}", command, force));

                    rb.AddTorque(force);
                    rb.AddForce(force);
                }
                break;

            default:
                Debug.LogError(string.Format("Unknown command: {0}", command));
                break;
        }
    }

    public uint SessionId
    {
        get { return this.session.SessionId; }
    }
}