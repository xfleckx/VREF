using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Assets.BeMoBI.Unity3D.Editor.Maze
{
    public static class EditorVisualUtils
    {
        public static void DrawFloorGrid(beMobileMaze maze)
        {
            // store map width, height and position
            var mapWidth = maze.MazeWidthInMeter;
            var mapHeight = maze.MazeLengthInMeter;

            //var position = maze.transform.position;
            var position = new Vector3(0, 0, 0);

            // draw layer border
            Gizmos.color = Color.white;
            Gizmos.DrawLine(position, position + new Vector3(mapWidth, 0, 0));
            Gizmos.DrawLine(position, position + new Vector3(0, 0, mapHeight));
            Gizmos.DrawLine(position + new Vector3(mapWidth, 0, 0), position + new Vector3(mapWidth, 0, mapHeight));
            Gizmos.DrawLine(position + new Vector3(0, 0, mapHeight), position + new Vector3(mapWidth, 0, mapHeight));

            Vector3 lineStart;
            Vector3 lineEnde;
            // draw tile cells
            Gizmos.color = Color.green;

            for (float i = 1; i <= maze.Columns; i++)
            {
                float xOffset = i * maze.RoomDimension.x;

                lineStart = position + new Vector3(xOffset, 0, 0);
                lineEnde = position + new Vector3(xOffset, 0, mapHeight);

                var labelPos = position + new Vector3(xOffset, 0, 0);
                Handles.Label(labelPos, i.ToString());

                Gizmos.DrawLine(lineStart, lineEnde);
            }

            for (float i = 1; i <= maze.Rows; i++)
            {
                float yoffset = i * maze.RoomDimension.z;

                lineStart = position + new Vector3(0, 0, yoffset);
                lineEnde = position + new Vector3(mapWidth, 0, yoffset);

                var labelPos = position + new Vector3(0, 0, yoffset);
                Handles.Label(labelPos, i.ToString());

                Gizmos.DrawLine(lineStart, lineEnde);
            }

            var zeroField = new Vector3(position.x + (maze.RoomDimension.x / 2), 0, position.x + (maze.RoomDimension.x / 2));

            Gizmos.color = new Color(Color.green.r, Color.green.g, Color.green.b, 0.1f);

            Gizmos.DrawCube(zeroField, new Vector3(maze.RoomDimension.x - 0.1f, 0, maze.RoomDimension.z - 0.1f));
        }
    }
}
