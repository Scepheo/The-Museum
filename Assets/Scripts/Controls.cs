using UnityEngine;

public static class Controls
{
    public static bool Up
    {
        get { return Input.GetAxis("Up") > 0; }
    }

    public static bool Down
    {
        get { return Input.GetAxis("Down") > 0; }
    }

    public static bool Left
    {
        get { return Input.GetAxis("Left") > 0; }
    }

    public static bool Right
    {
        get { return Input.GetAxis("Right") > 0; }
    }

    private static bool attachPressed = false;
    public static bool Attach
    {
        get
        {
            if (Input.GetAxis("Attach") > 0)
            {
                if (!attachPressed)
                {
                    attachPressed = true;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                attachPressed = false;
                return false;
            }
        }
    }

    private static bool restartPressed = false;
    public static bool Restart
    {
        get
        {
            if (Input.GetAxis("Restart") > 0)
            {
                if (!restartPressed)
                {
                    restartPressed = true;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                restartPressed = false;
                return false;
            }
        }
    }

    private static bool undoPressed = false;
    public static bool Undo
    {
        get
        {
            if (Input.GetAxis("Undo") > 0)
            {
                if (!undoPressed)
                {
                    undoPressed = true;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                undoPressed = false;
                return false;
            }
        }
    }

    private static bool exitPressed = false;
    public static bool Exit
    {
        get
        {
            if (Input.GetAxis("Exit") > 0)
            {
                if (!exitPressed)
                {
                    exitPressed = true;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                exitPressed = false;
                return false;
            }
        }
    }
}
