using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexaSniffer.GUI
{
    public class AvailablePackets : ObservableCollection<string>
    {
        public AvailablePackets()
        {
            Add("Test");
            Add("KeepAlive");
            Add("Server_List");
        }
    }
}
