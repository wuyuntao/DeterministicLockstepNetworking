using DeterministicLockstepNetworking;
using UnityEngine;

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

    public static void Initialize(Session session, bool isPlayer)
    {
        var resource = Resources.Load<GameObject>("Models/Cube/Cube");

        var position = new Vector3(Random.Range(-10f, 10f), Random.Range(1f, 10f), Random.Range(-10f, 10f));
        var cube = (GameObject)GameObject.Instantiate(resource, position, Quaternion.identity);
        var controller = cube.GetComponent<CubeController>();
        controller.session = session;

        if (!isPlayer)
        {
            var material = Resources.Load<Material>("Models/Cube/Materials/Friend");
            var renderer = cube.GetComponent<MeshRenderer>();
            renderer.material = material;
        }
    }

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
                    //Debug.Log(string.Format("{0} {1}", command, force));

                    rb.AddForce(force);
                }
                break;

            case Command.Back:
                {
                    var force = -1 * Vector3.forward * thrust + Vector3.up * thrust;
                    //Debug.Log(string.Format("{0} {1}", command, force));

                    rb.AddForce(force);
                }
                break;

            case Command.Left:
                {
                    var force = Vector3.left * thrust + Vector3.up * thrust;
                    //Debug.Log(string.Format("{0} {1}", command, torque));

                    rb.AddTorque(force);
                    rb.AddForce(force);
                }
                break;

            case Command.Right:
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
}