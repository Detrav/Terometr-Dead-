using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sniffer.Tera
{
    public class TeraPacket
    {
        public Type type;
        [PacketAttribute(0)]
        public ushort size { get; set; }
        [PacketAttribute(2)]
        public ushort opCode { get; set; }
        public byte[] data;

        public TeraPacket(byte[] _data, Type _type)
        {
            data = (byte[])_data.Clone();
            type = _type;

            foreach (PropertyInfo property in this.GetType().GetProperties())
            {
                var attr = (PacketAttribute)property.GetCustomAttribute(typeof(PacketAttribute));
                if (attr != null)
                {
                    switch (property.PropertyType.ToString())
                    {
                        case "BitArray":
                            property.SetValue(this, new System.Collections.BitArray(new byte[1] { data[attr.start] }));
                            break;
                        case "byte":
                            property.SetValue(this, data[attr.start]);
                            break;
                        case "sbyte":
                            property.SetValue(this, (sbyte)data[attr.start]);
                            break;
                        case "System.UInt16":
                            property.SetValue(this, BitConverter.ToUInt16(data, attr.start));
                            break;
                        case "short":
                            property.SetValue(this, BitConverter.ToInt16(data, attr.start));
                            break;
                        case "uint":
                            property.SetValue(this, BitConverter.ToUInt32(data, attr.start));
                            break;
                        case "int":
                            property.SetValue(this, BitConverter.ToInt32(data, attr.start));
                            break;
                        case "ulong":
                            property.SetValue(this, BitConverter.ToUInt64(data, attr.start));
                            break;
                        case "long":
                            property.SetValue(this, BitConverter.ToInt64(data, attr.start));
                            break;
                        case "float":
                            property.SetValue(this, BitConverter.ToSingle(data, attr.start));
                            break;
                        case "double":
                            property.SetValue(this, BitConverter.ToDouble(data, attr.start));
                            break;
                        case "char":
                            property.SetValue(this, Convert.ToChar(BitConverter.ToUInt16(data, attr.start)));
                            break;
                        case "string":
                            property.SetValue(this, byteArrayToString(data, attr.start, attr.start+attr.size));
                            break;
                        case "boolean"://Нужно исправить :)
                            property.SetValue(this, BitConverter.ToBoolean(data, attr.start));
                            break;
                        case "byte[]":
                            property.SetValue(this, copyByteArray(data,attr.start,attr.size));
                            break;
                    }
                }
            }
        }

        public TeraPacket(TeraPacket packet) : this(packet.data, packet.type) { }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Offset 00 01 02 03 04 05 06 07 | 08 09 0A 0B 0C 0D 0E 0F  0123456789ABCDEF\n");
            for (int i = 0; i < data.Length; i += 16)
            {
                sb.AppendFormat(" {0:X4}: {1,-24}| {2,-24} {3,-16}\n", i, byteArrayToHexString(data, i, 8), byteArrayToHexString(data, i + 8, 8), byteArrayToCharArray(data, i, 16));
            }
            sb.Append("\n");
            foreach (PropertyInfo property in this.GetType().GetProperties())
            {
                var attr = (PacketAttribute)property.GetCustomAttribute(typeof(PacketAttribute));
                if (attr != null)
                {
                    string str = "{0:X4} - {1} : {2} : {3}\n";
                    string str_with_shift = "{0:X4}+{1} - {2} : {3} : {4}\n";
                    switch (property.PropertyType.ToString())
                    {
                        case "boolean":
                            sb.AppendFormat(str_with_shift, attr.start, attr.shift, property.Name, property.GetValue(this), property.PropertyType.ToString());
                            break;
                        case "bitarray":
                            sb.AppendFormat(str, attr.start, property.Name, bitArrayToString(property.GetValue(this) as System.Collections.BitArray), property.PropertyType.ToString());
                            break;
                        default:
                            sb.AppendFormat(str, attr.start, property.Name, property.GetValue(this), property.PropertyType.ToString());
                            break;
                    }
                }
            }
            return sb.ToString();
        }

        public static byte[] copyByteArray(byte[] data, ushort p1, ushort len)
        {
            byte[] b = new byte[len];
            for (int i = 0; i < len; i++)
                b[i] = data[p1 + i];
            return b;
        }

        public static string byteArrayToString(byte[] data, int start, int len = int.MaxValue)
        {
            StringBuilder result = new StringBuilder();
            for (int i = start; i < data.Length && i < len; i += 2)
            {
                char c = Convert.ToChar(BitConverter.ToUInt16(data,start));
                if (c == '\0')
                    return result.ToString();
                else
                    result.Append(c);
            }
            return result.ToString();
        }

        public static string byteArrayToHexString(byte[] data, int start, int length)
        {
            StringBuilder result = new StringBuilder();
            for (int i = start; i < data.Length && i < start + length; i++)
                result.AppendFormat("{0:X2} ", data[i]);
            return result.ToString();
        }
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

        private static string bitArrayToString(System.Collections.BitArray bits)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bits.Count; i++)
                if (bits[i])
                    sb.Append(1);
                else
                    sb.Append(0);
            return sb.ToString();
        }

        public enum Type { Send, Recv }

        /*
         * В предыдущей версии я пробовал считывать данные из файла, но потом всёже передумал
         * и решил что ридер будет в самом пакете и сразу будет проходить возможный парсинг.
         * Сделаю наследование от TeraPacket
         */ 
    }
}
