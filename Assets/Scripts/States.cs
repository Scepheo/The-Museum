using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockState
{
    public Attachable Attachable;
    public bool Active;
    public bool IsAttached;
    public Vector3 Position;
    public Quaternion Rotation;

    public static BlockState MakeFor(Attachable attachable)
    {
        var obj = attachable.gameObject;

        return new BlockState
        {
            Attachable = attachable,
            IsAttached = attachable.IsAttached,
            Active = obj.activeSelf,
            Position = obj.transform.position,
            Rotation = obj.transform.rotation
        };
    }

    public void Apply()
    {
        var obj = Attachable.gameObject;

        if (IsAttached)
        {
            Attachable.Attach();
        }
        else
        {
            Attachable.Detach();
        }

        var meshRenderer = obj.GetComponent<MeshRenderer>();
        meshRenderer.enabled = Active;

        obj.SetActive(Active);
        obj.transform.position = Position;
        obj.transform.rotation = Rotation;
    }
}

public class PlayerState
{
    public Movement Player;
    public bool Active;
    public List<Transform> Blocks;
    public Vector3 Position;
    public Quaternion Rotation;

    public static PlayerState MakeFor(Movement player)
    {
        var obj = player.gameObject;

        return new PlayerState
        {
            Player = player,
            Active = player.Active,
            Blocks = player.Blocks.ToList(),
            Position = obj.transform.position,
            Rotation = obj.transform.rotation
        };
    }

    public void Apply()
    {
        var obj = Player.gameObject;

        Player.Blocks = Blocks.ToList();
        Player.Active = Active;
        obj.transform.position = Position;
        obj.transform.rotation = Rotation;
    }
}

public class GameState
{
    public PlayerState PlayerState;
    public List<BlockState> BlockStates;

    public static GameState Make()
    {
        var blocks = Object.FindObjectsOfType<Attachable>();
        var player = Object.FindObjectOfType<Movement>();

        return new GameState
        {
            PlayerState = PlayerState.MakeFor(player),
            BlockStates = blocks.Select(b => BlockState.MakeFor(b)).ToList()
        };
    }

    public void Apply()
    {
        PlayerState.Apply();

        foreach (var state in BlockStates)
        {
            state.Apply();
        }
    }
}