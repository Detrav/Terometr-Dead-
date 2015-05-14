using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sniffer.Tera
{
    class TeraPacketParser : TeraPacket
    {

        MemoryStream ms;
        BinaryReader br;
        List<PacketElement> elements;


        public TeraPacketParser(TeraPacket packet)
            : base(packet.data, packet.type)
        {
            ms = new MemoryStream(packet.data);
            br = new BinaryReader(ms);
            elements = new List<PacketElement>();
            readUInt16("size");
            readUInt16("opcode");
        }

        protected void readBitArray(string p)
        {
            PacketElement el = new PacketElement
            {
                name = p,
                start = (ushort)ms.Position,
                value = new System.Collections.BitArray(new byte[1] { br.ReadByte() }),
                type = "bitarray"
            }; 
            elements.Add(el);
        }
        protected void readByte(string p)
        {
            PacketElement el = new PacketElement
            {
                name = p,
                start = (ushort)ms.Position,
                value = br.ReadByte(),
                type = "byte"
            }; 
            elements.Add(el);
        }
        protected void readSByte(string p)
        {
            PacketElement el = new PacketElement
            {
                name = p,
                start = (ushort)ms.Position,
                value = br.ReadSByte(),
                type = "sbyte"
            }; 
            elements.Add(el);
        }
        protected void readUInt16(string p)
        {
            PacketElement el = new PacketElement
            {
                name = p,
                start = (ushort)ms.Position,
                value = br.ReadUInt16(),
                type = "ushort"
            }; 
            elements.Add(el);
        }
        protected void readInt16(string p)
        {
            PacketElement el = new PacketElement
            {
                name = p,
                start = (ushort)ms.Position,
                value = br.ReadInt16(),
                type = "short"
            };
            elements.Add(el);
        }
        protected void readUInt32(string p)
        {
            PacketElement el = new PacketElement
            {
                name = p,
                start = (ushort)ms.Position,
                value = br.ReadUInt32(),
                type = "uint"
            };
            elements.Add(el);
        }
        protected void readInt32(string p)
        {
            PacketElement el = new PacketElement
            {
                name = p,
                start = (ushort)ms.Position,
                value = br.ReadInt32(),
                type = "int"
            };
            elements.Add(el);
        }
        protected void readUInt64(string p)
        {
            PacketElement el = new PacketElement
            {
                name = p,
                start = (ushort)ms.Position,
                value = br.ReadUInt64(),
                type = "ulong"
            };
            elements.Add(el);
        }
        protected void readInt64(string p)
        {
            PacketElement el = new PacketElement
            {
                name = p,
                start = (ushort)ms.Position,
                value = br.ReadInt64(),
                type = "long"
            };
            elements.Add(el);
        }
        protected void readSingle(string p)
        {
            PacketElement el = new PacketElement
            {
                name = p,
                start = (ushort)ms.Position,
                value = br.ReadSingle(),
                type = "float"
            };
            elements.Add(el);
        }
        protected void readDouble(string p)
        {
            PacketElement el = new PacketElement
            {
                name = p,
                start = (ushort)ms.Position,
                value = br.ReadDouble(),
                type = "double"
            };
            elements.Add(el);
        }
        protected void readChar(string p)
        {
            PacketElement el = new PacketElement
            {
                name = p,
                start = (ushort)ms.Position,
                value = br.ReadChar(),
                type = "char"
            };
            elements.Add(el);
        }
        protected void readString(string p)
        {
            PacketElement el = new PacketElement
            {
                name = p,
                start = (ushort)ms.Position,
                value = br.ReadString(),
                type = "string"
            };
            elements.Add(el);
        }

        protected void readBoolean(string p, byte s)
        {
            if (s > 7)
                return;
            var temp = new System.Collections.BitArray(new byte[1] { br.ReadByte() });
            PacketElement el = new PacketElement
            {
                name = p,
                start = (ushort)ms.Position,
                value = temp[s],
                shift = s,
                type = "boolean"
            };
            elements.Add(el);
        }

        protected void readHex(string p, int lenght)
        {
            PacketElement el = new PacketElement
            {
                name = p,
                start = (ushort)ms.Position,
                type = "hex"
            };
            StringBuilder result = new StringBuilder();
            for (; lenght > 0; lenght--)
                result.AppendFormat("{0:X2}", br.ReadByte());
            el.value = result.ToString();
            elements.Add(el);
        }

        protected void close()
        {
            if (ms != null)
            {
                ms.Close();
                ms = null;
            }
        }

        ~TeraPacketParser()
        {
            close();
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
                        sb.AppendFormat(str, el.start, el.shift, el.name, bitArrayToString(el.value as System.Collections.BitArray), el.type);
                        break;
                    default:
                        sb.AppendFormat(str, el.start, el.shift, el.name, el.value, el.type);
                        break;
                }
            }
            return sb.ToString();
        }

        private static string byteArrayToHexString(byte[] data, int start, int length)
        {
            StringBuilder result = new StringBuilder();
            for (int i = start; i < data.Length && i < start + length; i++)
                result.AppendFormat("{0:X2} ", data[i]);
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
