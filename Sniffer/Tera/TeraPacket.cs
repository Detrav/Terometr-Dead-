using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.Sniffer.Tera
{
    public class TeraPacket
    {
        public PacketType type;
        public ushort size;
        public ushort opCode;
        public byte[] data;

        public TeraPacket(byte[] _data, Type _type)
        {
            data = (byte[])_data.Clone();
            size = BitConverter.ToUInt16(data, 0);//Размер
            opCode = BitConverter.ToUInt16(data, 2);//id пакета или OpCode
            type = _type;
        }
        public enum PacketType { Send, Recv }

        /*
         * В предыдущей версии я пробовал считывать данные из файла, но потом всёже передумал
         * и решил что ридер будет в самом пакете и сразу будет проходить возможный парсинг.
         * Сделаю наследование от TeraPacket
         */

        public override string ToString()
        {
            return String.Format("{0,6} {1,6} {2}", size, opCode, TeraPacketParser.byteArrayToHexStringRightToLeft(data,0,data.Length));
        }
    }
}
