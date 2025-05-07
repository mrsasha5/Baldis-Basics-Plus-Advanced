using System;
using System.Collections.Generic;
using System.Text;

namespace BaldisBasicsPlusAdvanced.Patches
{
    public static class RoomExtensions
    {

        public static void setCellsRange(this RoomAsset room, IntVector2 vector1, IntVector2 vector2, bool buildWallsAround = true)
        {
            int x1 = vector1.x;
            int z1 = vector1.z;
            while (x1 < vector2.x)
            {
                while (z1 < vector2.z)
                {
                    CellData cellData = new CellData()
                    {
                        pos = new IntVector2(x1, z1)
                    };
                    room.cells.Add(cellData);
                    if (buildWallsAround)
                    {
                        //0 - no walls
                        //12, 9, 6, 3 - angles
                        //8, 4, 2, 1 - walls

                        //angles
                        if (x1 == vector1.x && z1 == vector1.z) cellData.type = 12;
                        if (x1 == vector1.x && z1 == (vector2.z - 1)) cellData.type = 9;
                        if (x1 == (vector2.x - 1) && z1 == (vector2.z - 1)) cellData.type = 3;
                        if (x1 == (vector2.x - 1) && z1 == vector1.z) cellData.type = 6;

                        //walls
                        if (x1 == vector1.x && z1 != vector1.z && z1 != (vector2.z - 1)) cellData.type = 8;
                        if (x1 == (vector2.x - 1) && z1 != vector1.z && z1 != (vector2.z - 1)) cellData.type = 2;
                        if (z1 == vector1.z && x1 != vector1.x && x1 != (vector2.x - 1)) cellData.type = 4;
                        if (z1 == (vector2.z - 1) && x1 != vector1.x && x1 != (vector2.x - 1)) cellData.type = 1;
                    }
                    z1++;
                }
                z1 = vector1.z;
                x1++;
            }
        }

    }
}
