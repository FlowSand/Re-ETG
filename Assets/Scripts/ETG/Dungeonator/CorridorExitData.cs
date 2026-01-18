using System.Collections.Generic;

#nullable disable
namespace Dungeonator
{
    public class CorridorExitData
    {
        public List<CellData> cells;
        public RoomHandler linkedRoom;

        public CorridorExitData(List<CellData> c, RoomHandler rh)
        {
            this.cells = c;
            this.linkedRoom = rh;
        }
    }
}
