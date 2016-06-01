using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary> 
/// The world coordinate system in Unity3D is left handed (as Direct X) where x positive axis is right, y positive is up and z is positive into the screen.
/// 
/// +z -> forward   -> North
/// +x -> right     -> East
/// -x -> left      -> West 
/// -z -> back      -> South
/// 
/// </summary>
public class OrientationDefinition
{
    private static OrientationDefinition current;
    public static OrientationDefinition Current
    {
        get
        {

            if (current == null)
            {
                current = new OrientationDefinition();
            }

            return current;
        }
    }

    public const string NORTH = "North";
    public const string SOUTH = "South";
    public const string EAST  = "East";
    public const string WEST  = "West";

    public const float RIGHT_TURN_DIRECTION = 1;
    public const float LEFT_TURN_DIRECTION  = -1;

    public const float NORTH_DEGREE = 0;
    public const float SOUTH_DEGREE = 180;
    public const float EAST_DEGREE = 90;
    public const float WEST_DEGREE = -90;

    public float GetEulerDirection(string direction)
    {
        if (direction.Equals(NORTH))
            return NORTH_DEGREE;

        if (direction.Equals(SOUTH))
            return SOUTH_DEGREE;

        if (direction.Equals(EAST))
            return EAST_DEGREE;

        if (direction.Equals(WEST))
            return WEST_DEGREE;

        throw new ArgumentException(string.Format("Direction name \"{0}\" not defined in this implementation of a orientation definition", direction), direction);
    }
    
    public string GetDirectionNameFromEuler(float eulerTurning)
    {
        if (eulerTurning == NORTH_DEGREE || eulerTurning == 360)
            return NORTH;

        if(eulerTurning > 0)
        {
            if(eulerTurning == 90)
                return EAST;

            if(eulerTurning == 180)
                return SOUTH;

            if (eulerTurning == 270)
                return WEST;
        }

        if (eulerTurning < 0)
        {
            if (eulerTurning == -270 )
                return EAST;

            if (eulerTurning == -180)
                return SOUTH;

            if (eulerTurning == -90 )
                return WEST;
        }

        throw new ArgumentException(string.Format("The given degree {0}° are not defined!", eulerTurning));
    }
}
