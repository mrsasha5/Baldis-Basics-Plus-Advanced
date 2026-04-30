using System.Collections.Generic;

namespace BaldisBasicsPlusAdvanced.Game.Rooms.Functions
{
    // New 0.14.2 open area group behaviour works strange for a rooms (seems to have a logical issues)
    // But this function patches that
    // (Intentionally invented for the Farm maze special room to make MapLegacyFillingPatch fills the map in the old way)
    public class ProperOpenAreaCellGroupFunction : RoomFunction
    {
        public override void OnGenerationFinished()
        {
            base.OnGenerationFinished();
            for (int i = 0; i < room.cells.Count; i++)
            {
                room.cells[i].openTileGroup = null;
            }
            List<OpenTileGroup> openTileGroups = new List<OpenTileGroup>();
            Queue<Cell> openTilesToCheck = new Queue<Cell>();
            for (int x = 0; x < room.size.x; x++)
            {
                for (int z = 0; z < room.size.z; z++)
                {
                    Cell cell3 = room.ec.cells[room.position.x + x, room.position.z + z];
                    if (cell3.open)
                    {
                        bool inTileGroup = false;
                        foreach (OpenTileGroup item14 in openTileGroups)
                        {
                            if (cell3.openTileGroup == item14)
                            {
                                inTileGroup = true;
                            }
                        }
                        if (!inTileGroup)
                        {
                            OpenTileGroup currentGroup = new OpenTileGroup();
                            openTileGroups.Add(currentGroup);
                            openTilesToCheck.Clear();
                            openTilesToCheck.Enqueue(cell3);
                            while (openTilesToCheck.Count > 0)
                            {
                                cell3 = openTilesToCheck.Dequeue();
                                cell3.openTileGroup = currentGroup;
                                currentGroup.cells.Add(cell3);
                                foreach (Cell openTile in cell3.openTiles)
                                {
                                    if (openTile.openTileGroup != currentGroup && !openTilesToCheck.Contains(openTile))
                                    {
                                        openTilesToCheck.Enqueue(openTile);
                                    }
                                }
                            }
                            foreach (Cell tileToConnect in currentGroup.cells)
                            {
                                foreach (Direction item15 in Directions.OpenDirectionsFromBin(tileToConnect.ConstBin))
                                {
                                    if (room.ec.ContainsCoordinates(tileToConnect.position + item15.ToIntVector2()) && 
                                        !room.ec.CellFromPosition(tileToConnect.position + item15.ToIntVector2()).Null && 
                                        !currentGroup.cells.Contains(room.ec.CellFromPosition(tileToConnect.position + item15.ToIntVector2())))
                                    {
                                        currentGroup.exits.Add(new OpenGroupExit(currentGroup, tileToConnect, item15));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
