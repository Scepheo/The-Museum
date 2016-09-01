using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum Direction
{
    Left, Right, Up, Down
}

public class Movement : MonoBehaviour
{
    public bool Moving { get; private set; }
    public bool Falling { get; private set; }

    private bool active = true;
    public bool Active
    {
        get { return active; }
        set
        {
            GetComponent<MeshRenderer>().enabled = value;

            foreach (var block in Blocks)
            {
                block.GetComponent<MeshRenderer>().enabled = value;
            }

            active = value;
        }
    }

    public bool InPhysics
    {
        get { return Moving || Falling || FallingBlocks.Any(); }
    }

    public bool HasAttachments
    {
        get { return Blocks.Any(); }
    }

    public float stepTime = 1f;
    private float passedTime = 0f;

    private Vector3 rotationPoint, angles;
    public List<Transform> Blocks;
    private List<Transform> FallingBlocks;

    private UndoStack undoes;

    private void Start()
    {
        Blocks = new List<Transform>();
        FallingBlocks = new List<Transform>();

        // Create undo stack
        undoes = new UndoStack();
    }

    private void Update()
    {
        if (Controls.Restart)
        {
            Transitions.FadeRestart(0.3f);
        }

        if (InPhysics)
        {
            RunPhysics();
        }
        else
        {
            if (Controls.Undo)
            {
                undoes.Undo();
            }
            else if (Active)
            {
                if (Controls.Up)
                {
                    TryMovement(Direction.Up);
                }
                else if (Controls.Down)
                {
                    TryMovement(Direction.Down);
                }
                else if (Controls.Left)
                {
                    TryMovement(Direction.Left);
                }
                else if (Controls.Right)
                {
                    TryMovement(Direction.Right);
                }
                else if (Controls.Attach)
                {
                    Attach();
                }
            }
        }
    }

    private void Snap(Transform t)
    {
        var r = t.rotation.eulerAngles;

        t.rotation = Quaternion.Euler(
            (90f * Mathf.Round(r.x / 90f)) % 360f,
            (90f * Mathf.Round(r.y / 90f)) % 360f,
            (90f * Mathf.Round(r.z / 90f)) % 360f
            );

        var p = t.position;

        t.position = new Vector3(
            0.5f + Mathf.Round(p.x - 0.5f),
            0.5f + Mathf.Round(p.y - 0.5f),
            0.5f + Mathf.Round(p.z - 0.5f)
            );
    }

    private void TryMovement(Direction dir)
    {
        var blocks = FindGroundedBlocks(dir).OrderBy(t => t.position.y);

        foreach (var block in blocks)
        {
            SetMovement(dir, block);

            if (TestMovement())
            {
                undoes.Do();
                Moving = true;
                break;
            }
        }
    }

    private void SetMovement(Direction dir, Transform around)
    {
        switch (dir)
        {
            case Direction.Up:
                rotationPoint = around.position + new Vector3(0.5f, -0.5f, 0f);
                angles = new Vector3(0f, 0f, -90f);
                break;
            case Direction.Down:
                rotationPoint = around.position + new Vector3(-0.5f, -0.5f, 0f);
                angles = new Vector3(0f, 0f, 90f);
                break;
            case Direction.Left:
                rotationPoint = around.position + new Vector3(0f, -0.5f, 0.5f);
                angles = new Vector3(90f, 0f, 0f);
                break;
            case Direction.Right:
                rotationPoint = around.position + new Vector3(0f, -0.5f, -0.5f);
                angles = new Vector3(-90f, 0f, 0f);
                break;
        }
    }

    private bool TestMovement()
    {
        // Check collisions
        var canMove = true;

        // Test halfway position
        Rotate(0.5f);
        canMove = !CheckCollisions();

        // Test final position
        Rotate(0.5f);

        if (canMove)
        {
            canMove = !CheckCollisions();
        }

        // Undo movement
        Rotate(-1f);

        // Temporary hack, should implement try-once logic and handle there
        foreach (var body in FindObjectsOfType<Rigidbody>())
        {
            Snap(body.transform);
        }

        return canMove;
    }

    private bool CheckCollisions()
    {
        var collision = false;

        var solids = new List<Collider>();
        var bodies = new List<Collider>();
        var looses = new List<Collider>();

        bodies.Add(GetComponent<Collider>());
        solids.AddRange(FindObjectsOfType<Wall>().Select(w => w.GetComponent<Collider>()));

        foreach (var attachable in FindObjectsOfType<Attachable>())
        {
            if (attachable.IsAttached)
            {
                bodies.Add(attachable.GetComponent<Collider>());
            }
            else
            {
                looses.Add(attachable.GetComponent<Collider>());
            }
        }

        foreach (var loose in looses)
        {
            foreach (var body in bodies)
            {
                if (loose.bounds.Intersects(body.bounds))
                {
                    collision = true;
                    break;
                }
            }

            foreach (var solid in solids)
            {
                if (loose.bounds.Intersects(solid.bounds))
                {
                    collision = true;
                    break;
                }
            }

            foreach (var other in looses)
            {
                if (!ReferenceEquals(loose, other) && loose.bounds.Intersects(other.bounds))
                {
                    collision = true;
                    break;
                }
            }

            if (collision)
            {
                break;
            }
        }

        foreach (var body in bodies)
        {
            foreach (var solid in solids)
            {
                if (solid.bounds.Intersects(body.bounds))
                {
                    collision = true;
                    break;
                }
            }

            if (collision)
            {
                break;
            }
        }

        return collision;
    }

    private void Rotate(float fraction)
    {
        transform.position = rotationPoint
                + Quaternion.Euler(angles * fraction)
                * (transform.position - rotationPoint);
        transform.Rotate(angles * fraction, Space.World);

        foreach (var attached in Blocks)
        {
            attached.position = rotationPoint
                + Quaternion.Euler(angles * fraction)
                * (attached.position - rotationPoint);
            attached.Rotate(angles * fraction, Space.World);
        }
    }

    private IEnumerable<Transform> FindGroundedBlocks(Direction dir)
    {
        bool collision = false;

        // Test player
        transform.position += Vector3.down;
        collision = CheckCollisions();
        transform.position += Vector3.up;

        if (collision)
        {
            yield return transform;
        }

        // Test all attached blocks
        foreach (var t in Blocks)
        {
            t.position += Vector3.down;
            collision = CheckCollisions();
            t.position += Vector3.up;

            if (collision)
            {
                yield return t;
            }
        }
    }

    private void Attach()
    {
        if (HasAttachments)
        {
            undoes.Do();

            foreach (var block in Blocks)
            {
                block.gameObject.GetComponent<Attachable>().Detach();
            }

            Blocks.Clear();
            CheckPhysics();
        }
        else
        {
            var attachables = FindObjectsOfType<Attachable>();

            var nearAttachables = attachables.Where(attachable => Vector3.Distance(transform.position, attachable.transform.position) <= 1.1f).ToList();

            if (nearAttachables.Count > 0)
            {
                undoes.Do();

                foreach (var nearAttachable in nearAttachables)
                {
                    nearAttachable.Attach();
                    Blocks.Add(nearAttachable.transform);
                }
            }
        }
    }

    private bool CheckOnGround()
    {
        var onGround = true;

        transform.position += Vector3.down;

        foreach (var block in Blocks)
        {
            block.position += Vector3.down;
        }

        onGround = CheckCollisions();

        transform.position += Vector3.up;

        foreach (var block in Blocks)
        {
            block.position += Vector3.up;
        }

        return onGround;
    }

    private void RunPhysics()
    {
        if (!InPhysics)
        {
            return;
        }

        passedTime += Time.deltaTime;

        if (passedTime > stepTime)
        {
            passedTime = 0f;

            var rigidbodies = FindObjectsOfType<Rigidbody>();

            foreach (var body in rigidbodies)
            {
                Snap(body.transform);
            }

            // Check physics again...
            CheckPhysics();
        }
        else
        {
            if (Moving)
            {
                Rotate(Time.deltaTime / stepTime);
            }

            var gravity = Vector3.down * Time.deltaTime / stepTime;

            if (Falling)
            {
                // Make self fall
                transform.position += gravity;

                foreach (var block in Blocks)
                {
                    block.position += gravity;
                }
            }

            foreach (var fallingBlock in FallingBlocks)
            {
                fallingBlock.position += gravity;
            }
        }
    }

    private void CheckPhysics()
    {
        // Stop everything
        Moving = false;
        Falling = false;
        FallingBlocks.Clear();

        // Check if we are on the ground
        if (transform.position.y < -5f)
        {
            Active = false;
        }
        else if (Active)
        {
            transform.position += Vector3.down;
            foreach (var block in Blocks)
            {
                block.position += Vector3.down;
            }

            Falling = !CheckCollisions();

            transform.position += Vector3.up;
            foreach (var block in Blocks)
            {
                block.position += Vector3.up;
            }
        }

        // Check if any other objects should fall
        var looses = FindObjectsOfType<Attachable>().Where(a => !a.IsAttached);

        foreach (var loose in looses)
        {
            // Check if falling out of level
            if (loose.transform.position.y < -5f)
            {
                loose.gameObject.SetActive(false);
            }
            else
            {
                loose.transform.position += Vector3.down;

                if (!CheckCollisions())
                {
                    FallingBlocks.Add(loose.transform);
                }

                loose.transform.position += Vector3.up;
            }
        }
    }
}