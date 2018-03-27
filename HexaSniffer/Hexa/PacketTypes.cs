using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexaSniffer.Hexa
{
    enum PacketTypes
    {
        KEEPALIVE = ((byte) 0x00),
        REGISTER = ((byte) 0x01),
        JOIN = ((byte) 0x02),
        LEAVE = ((byte) 0x03),
        BUILD = ((byte) 0x04),
        DESTROY = ((byte) 0x05),
        TRADE = ((byte) 0x06),
        MAPUPDATE = ((byte) 0x07),
        TERMINATE = ((byte) 0x08),
        SERVER_LIST = ((byte) 0x09),

    /*
    * Everything from 0xE0 to 0xFF is reserved for tools
    * 
    */
        GET_ALL_PLAYER =((byte) 0xE0),
        GET_ALL_ROOMS =((byte) 0xE1),
        GET_ALL_PLAYER_OF_ROOM = ((byte) 0xE1)
    }
}
