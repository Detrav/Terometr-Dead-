using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.Sniffer.Tera
{
    public class TeraPacketParser : TeraPacket
    {

        //MemoryStream ms;
        //BinaryReader br;
        List<PacketElement> elements;


        public TeraPacketParser(TeraPacket packet)
            : base(packet.data, packet.type)
        {
            //ms = new MemoryStream(packet.data);
            //br = new BinaryReader(ms);
            elements = new List<PacketElement>();
            readUInt16(0,"size");
            readUInt16(2,"opcode");
        }

        public PacketElement this[string str]
        {
            get
            {
                foreach (var el in elements)
                    if (el.name == str)
                        return el;
                return null;
            }
            set
            {
                foreach (var el in elements)
                    if (el.name == str)
                    {
                        el.shift = value.shift;
                        el.start = value.start;
                        el.type = value.type;
                        el.value = value.value;
                        return;
                    }
                elements.Add(value);
            }
        }

        protected System.Collections.BitArray readBitArray(ushort pos,string p)
        {
            PacketElement el = new PacketElement
            {
                name = p,
                start = pos,
                value = new System.Collections.BitArray(new byte[1] { data[pos] }),
                type = "bitarray"
            };
            this[p] = el;
            return (System.Collections.BitArray)el.value;
        }
        protected byte readByte(ushort pos,string p)
        {
            PacketElement el = new PacketElement
            {
                name = p,
                start = pos,
                value = data[pos],
                type = "byte"
            }; 
            this[p] =el;
            return (byte)el.value;
        }
        protected sbyte readSByte(ushort pos,string p)
        {
            PacketElement el = new PacketElement
            {
                name = p,
                start = pos,
                value = (sbyte)data[pos],
                type = "sbyte"
            };
            this[p] = el;
            return (sbyte)el.value;
        }
        protected ushort readUInt16(ushort pos,string p)
        {
            PacketElement el = new PacketElement
            {
                name = p,
                start = pos,
                value = BitConverter.ToUInt16(data,pos),
                type = "ushort"
            };
            this[p] = el;
            return (ushort)el.value;
        }
        protected short readInt16(ushort pos,string p)
        {
            PacketElement el = new PacketElement
            {
                name = p,
                start = pos,
                value = BitConverter.ToInt16(data, pos),
                type = "short"
            };
            this[p] = el;
            return (short)el.value;
        }
        protected uint readUInt32(ushort pos, string p)
        {
            PacketElement el = new PacketElement
            {
                name = p,
                start = pos,
                value = BitConverter.ToUInt32(data,pos),
                type = "uint"
            };
            this[p] = el;
            return (uint)el.value;
        }
        protected int readInt32(ushort pos ,string p)
        {
            PacketElement el = new PacketElement
            {
                name = p,
                start = pos,
                value = BitConverter.ToInt32(data,pos),
                type = "int"
            };
            this[p] = el;
            return (int)el.value;
        }
        protected ulong readUInt64(ushort pos,string p)
        {
            PacketElement el = new PacketElement
            {
                name = p,
                start = pos,
                value = BitConverter.ToUInt64(data,pos),
                type = "ulong"
            };
            this[p] = el;
            return (ulong)el.value;
        }
        protected long readInt64(ushort pos,string p)
        {
            PacketElement el = new PacketElement
            {
                name = p,
                start = pos,
                value = BitConverter.ToInt64(data,pos),
                type = "long"
            };
            this[p] = el;
            return (long)el.value;
        }
        protected float readSingle(ushort pos,string p)
        {
            PacketElement el = new PacketElement
            {
                name = p,
                start = pos,
                value = BitConverter.ToSingle(data,pos),
                type = "float"
            };
            this[p] = el;
            return (float)el.value;
        }
        protected double readDouble(ushort pos, string p)
        {
            PacketElement el = new PacketElement
            {
                name = p,
                start = pos,
                value = BitConverter.ToDouble(data,pos),
                type = "double"
            };
            this[p] = el;
            return (double)el.value;
        }
        protected char readChar(ushort pos, string p)
        {
            PacketElement el = new PacketElement
            {
                name = p,
                start = pos,
                value = BitConverter.ToChar(data,pos),
                type = "char"
            };
            this[p] = el;
            return (char)el.value;
        }
        protected string readString(ushort pos,string p, int len = int.MaxValue)
        {
            PacketElement el = new PacketElement
            {
                name = p,
                start = pos,
                type = "string"
            };
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
            el.value = result.ToString();
            this[p] = el;
            return (string)el.value;
        }

        protected bool readBoolean(ushort pos,string p, byte s = 0)
        {
            if (s > 7)
                return false;
            var temp = new System.Collections.BitArray(new byte[1] { data[pos] });
            PacketElement el = new PacketElement
            {
                name = p,
                start = pos,
                value = temp[s],
                shift = s,
                type = "boolean"
            };
            this[p] = el;
            return (bool)el.value;
        }

        protected string readHex(ushort pos,string p, int lenght)
        {
            PacketElement el = new PacketElement
            {
                name = p,
                start = pos,
                type = "hex"
            };
            StringBuilder result = new StringBuilder();
            for (int i = 0; i< lenght; i++)
                result.AppendFormat("{0:X2}", data[i]);
            el.value = result.ToString();
            this[p] = el;
            return (string)el.value;
        }

        public class PacketElement
        {
            public string name = null;
            public ushort start = 0;//в байтах
            public byte shift = 0;//в битах
            public object value = null;
            public string type = null;
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
            foreach (var el in elements)
            {
                string str = "{0:X4} - {1} : {2} : {3}\n";
                string str_with_shift = "{0:X4}+{1} - {2} : {3} : {4}\n";
                switch (el.type)
                {
                    case "boolean":
                        sb.AppendFormat(str_with_shift, el.start, el.shift, el.name, el.value, el.type);
                        break;
                    case "bitarray":
                        sb.AppendFormat(str, el.start, el.name, bitArrayToString(el.value as System.Collections.BitArray), el.type);
                        break;
                    default:
                        sb.AppendFormat(str, el.start, el.name, el.value, el.type);
                        break;
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
