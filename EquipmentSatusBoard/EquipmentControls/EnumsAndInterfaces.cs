using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquipmentSatusBoard.EquipmentControls
{
    internal enum EquipmentStatus
    {
        Operational,
        Degraded,
        Scheduled,
        Down
    }

    internal enum OperationalStatus
    {
        OnLine,
        OffLine
    }
}
