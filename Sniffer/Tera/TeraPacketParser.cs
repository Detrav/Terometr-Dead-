using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.Sniffer.Tera
{
    public class TeraPacketParser
    {

        //MemoryStream ms;
        //BinaryReader br;
        //List<PacketElement> elements;
        [PacketInfo(0)]
        public ushort size { get; set; }
        [PacketInfo(0)]
        public ushort opCode;
        public TeraPacket.PacketType type;
        public byte[] data;
        Type t;

        public TeraPacketParser(TeraPacket packet)
        {
            t = this.GetType();
            data = packet.data;
            type = packet.type;
            size = readUInt16(0);
            opCode = readUInt16(2);
        }

        /*public object this[string str]
        {
            get
            {
                return t.GetProperty(str).
            }
            set
            {
                t.GetField(str).SetValue(this, value);
            }
        }*/

        protected System.Collections.BitArray readBitArray(ushort pos)
        {
            return new System.Collections.BitArray(new byte[1] { data[pos] });
        }
        protected byte readByte(ushort pos)
        {
            return data[pos];
        }
        protected sbyte readSByte(ushort pos)
        {
            return (sbyte)data[pos];
        }
        protected ushort readUInt16(ushort pos)
        {
            return BitConverter.ToUInt16(data, pos);
        }
        protected short readInt16(ushort pos)
        {
            return BitConverter.ToInt16(data, pos);
        }
        protected uint readUInt32(ushort pos)
        {
            return BitConverter.ToUInt32(data, pos);
        }
        protected int readInt32(ushort pos)
        {
            return BitConverter.ToInt32(data, pos);
        }
        protected ulong readUInt64(ushort pos)
        {
            return BitConverter.ToUInt64(data, pos);
        }
        protected long readInt64(ushort pos)
        {
            return BitConverter.ToInt64(data, pos);
        }
        protected float readSingle(ushort pos)
        {
            return BitConverter.ToSingle(data, pos);
        }
        protected double readDouble(ushort pos)
        {
            return BitConverter.ToDouble(data,pos);
        }
        protected char readChar(ushort pos)
        {
            return Convert.ToChar(BitConverter.ToUInt16(data,pos));
        }
        protected string readString(ushort pos, int len = int.MaxValue)
        {
            
            ushort start = pos;
            StringBuilder result = new StringBuilder();
            for (int i = start; i<len ; i += 2)
            {
                char c = Convert.ToChar(BitConverter.ToUInt16(data,i));
                if (c == '\0')
                    break;
                else
                    result.Append(c);
            }
            return result.ToString();
        }
        protected bool readBoolean(ushort pos, byte s = 0)
        {
            if (s > 7)
                return false;
            var temp = new System.Collections.BitArray(new byte[1] { data[pos] });
            return temp[s];
        }
        protected byte[] readHex(ushort pos , int lenght)
        {
            byte[] bb = new byte[lenght];
            for (int i = 0; i < lenght; i++)
                bb[i] = data[i + pos];
            return bb;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Offset 00 01 02 03 04 05 06 07 | 08 09 0A 0B 0C 0D 0E 0F  0123456789ABCDEF\n");
            for (int i = 0; i < data.Length; i += 16)
            {
                sb.AppendFormat(" {0:X4}: {1,-24}| {2,-24} {3,-16}\n", i, byteArrayToHexString(data, i, 8), byteArrayToHexString(data, i + 8, 8), byteArrayToCharArray(data, i, 16));
            }
            sb.Append("\n");
            foreach (var el in t.GetProperties())
            {
                if (Attribute.IsDefined(el, typeof(PacketInfo)))
                {
                    PacketInfo attr  = (PacketInfo)(el.GetCustomAttributes(typeof(PacketInfo),false)[0]);
                    string str = "{0:X4} - {1} : {2} : {3}\n";
                    string str_with_shift = "{0:X4}+{1} - {2} : {3} : {4}\n";
                    switch (el.PropertyType.ToString())
                    {
                        case "boolean"://Поменять
                            sb.AppendFormat(str_with_shift, attr.shift, attr.shift, el.Name, el.GetValue(this), el.PropertyType);
                            break;
                        case "bitarray"://Поменять
                            sb.AppendFormat(str, attr.start, el.Name, bitArrayToString(el.GetValue(this) as System.Collections.BitArray), el.PropertyType);
                            break;
                        case "hex"://Поменять
                            break;
                        default:
                            //Поменять
                            //sb.AppendFormat(str, attr.start, el.Name, el.GetValue(this), el.PropertyType);
                            sb.AppendFormat(str, attr.start, el.Name, "", el.PropertyType);
                            break;
                    }
                }
            }
            return sb.ToString();
        }

        public static string byteArrayToHexString(byte[] data, int start, int length)
        {
            StringBuilder result = new StringBuilder();
            for (int i = start; i < data.Length && i < start + length; i++)
                result.AppendFormat("{0:X2} ", data[i]);
            return result.ToString();
        }
        //Тоже самое что и предыдущий, только без пробела, назвал функцию с права на лево, т.к. переменная попадая в поток записывается для нас справа на лево
        public static string byteArrayToHexStringRightToLeft(byte[] data, int start, int length)
        {
            StringBuilder result = new StringBuilder();
            for (int i = start; i < data.Length && i < start + length; i++)
                result.AppendFormat("{0:X2}", data[i]);
            return result.ToString();
        }
        private static string byteArrayToCharArray(byte[] data, int start, int length)
        {
            StringBuilder result = new StringBuilder();
            for (int i = start; i < data.Length && i < start + length; i++)
            {
                char c = Convert.ToChar(data[i]);
                if (char.IsControl(c))
                {
                    result.Append('.');
                }
                else
                    result.Append(c);
            }
            return result.ToString();
        }
        public static string byteArrayToString(byte[] data, int start,int len = int.MaxValue)
        {
            StringBuilder result = new StringBuilder();
            for (int i = start; i < data.Length && i<len; i+=2)
            {
                char c = BitConverter.ToChar(data,i);
                if (c == '\0')
                    return result.ToString();
                else
                    result.Append(c);
            }
            return result.ToString();
        }

        private static string bitArrayToString(System.Collections.BitArray bits)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bits.Count ; i++)
                if (bits[i])
                    sb.Append(1);
                else
                    sb.Append(0);
            return sb.ToString();
        }
    }
}
