using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SnifferGUI
{
    class PacketWithStructure
    {
        /*
         * Мне нужно описать файл таким образом: я гружу xml файл потом вызываю функцию и получаю форматированую строку с цветами а ниже описание
         * 
         * файл называется "{Название пакета}.xml"
         * <?xml?>
         *  <PacketStructure>
         *      <ushort number="0" Color = "black" BgColor = "white" >Длина</ushort>
         *      <ushort number="2" Color = "blue" >Номер пакета</ushort>
         *      <uint number="4" Color ="white" BgColor = "Blue">Сверхномер</uint>
         *      <hex number = "10" Color ="yellow" size ="8">ID</hex>
         *      <string number = "10">Имя игрока<string>
         *  </PacketStructure>
         */

        struct PacketElement
        {
            internal string type;
            internal ushort num;
            internal ushort size;
            internal int color;
            internal int bgcolor;
            internal string name;
            internal byte shift;
        }
        /* 
         * + Обязательно, - Лишний, * Опциональный
         * Поддерживаемые типы: |Номер|Размер|Имя|Цвет|Фон|бит сдвиг|Резерв
         * 1) bit array         |  +  |  +   | + | *  | * |+
         * 2) byte              |  +  |  -   | + | *  | * |
         * 3) sbyte             |  +  |  -   | + | *  | * |
         * 4) ushort            |  +  |  -   | + | *  | * |
         * 5) short             |  +  |  -   | + | *  | * |
         * 6) int               |  +  |  -   | + | *  | * |
         * 7) uint              |  +  |  -   | + | *  | * |
         * 8) long              |  +  |  -   | + | *  | * |
         * 9) ulong             |  +  |  -   | + | *  | * |
         * 10) float            |  +  |  -   | + | *  | * |
         * 11) double           |  +  |  -   | + | *  | * |
         * 12) char             |  +  |  -   | + | *  | * |
         * 13) string           |  +  |  -   | + | *  | * |
         * 14) bool             |  +  |  -   | + | *  | * |+
         * 15) hex              |  +  |  +   | + | *  | * |
         * <{type:string} number = "{ushort}" size = "{ushort}" color = "{int:FF000000}" bgcolor = "{int:FFFFFFFF}">{name:string}</>
         */
        PacketElement[] elements;

        public PacketWithStructure(string packetNameForFile)
        {
            using (XmlReader xr = XmlReader.Create(Path.Combine("assets", "packets",packetNameForFile+".xml")))
            {
                XmlDocument xd = new XmlDocument();
                xd.Load(xr);
                XmlNodeList elemetsXml = xd.GetElementsByTagName("PacketElements");
                if (elemetsXml.Count > 0)
                {
                    List<PacketElement> elementsList = new List<PacketElement>();
                    foreach (XmlNode el in elemetsXml[0].ChildNodes)
                    {
                        PacketElement pe;
                        pe.type = el.Name;
                        if (el.Attributes["number"] != null)
                            pe.num = UInt16.Parse(el.Attributes["number"].Value);
                        else
                            pe.num = 0;
                        if (el.Attributes["size"] != null)
                            pe.size = UInt16.Parse(el.Attributes["size"].Value);
                        else
                            pe.size = 0;
                        if (el.Attributes["color"] != null)
                            pe.color = Convert.ToInt32(el.Attributes["color"].Value, 16);
                        else
                            pe.color = Convert.ToInt32("FF000000", 16);
                        if(el.Attributes["bgcolor"]!=null)
                            pe.bgcolor = Convert.ToInt32(el.Attributes["bgcolor"].Value, 16);
                        else
                            pe.bgcolor = Convert.ToInt32("FFFFFF", 16);
                        if (el.InnerText != null)
                            pe.name = el.InnerText;
                        else
                            pe.name = "";
                        if (el.Attributes["shift"] != null)
                            pe.shift = byte.Parse(el.Attributes["shift"].Value);
                        else
                            pe.shift = 0;
                        elementsList.Add(pe);
                    }
                    elements = elementsList.ToArray();
                }
            }
        }

        internal void getRtf(ref System.Windows.Forms.RichTextBox rtf,byte[] data)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Offset 00 01 02 03 04 05 06 07 | 08 09 0A 0B 0C 0D 0E 0F  0123456789ABCDEF\n");
            for(int i = 0; i<data.Length;i+=16)
            {
                sb.AppendFormat(" {0:X4}: {1,-24}| {2,-24} {3,-16}\n", i, byteArrayToHexString(data, i, 8), byteArrayToHexString(data, i + 8, 8), byteArrayToCharArray(data,i,16));
            }
            sb.AppendFormat("\n{0:X4} - Размер: {1}\n",0, BitConverter.ToUInt16(data, 0));
            int opCode = BitConverter.ToUInt16(data, 2);
            string opCodeName = Config.Instance.packetName[opCode];
            if (opCodeName == "") opCodeName = "_I_UNKNOWN_RLY_SORRY";
            sb.AppendFormat("{0:X4} - Код: {1} : {2}\n",2, opCode, opCodeName);
            foreach (var el in elements)
            {
                string str = "{0:X4} - {1}: {2} : {3}";
                string str_with_shift = "{0:X4}+{1} - {2}: {3} : {4}";
                switch(el.type)
                { 
                    case "bitarray":
                        str = String.Format(str_with_shift,el.num,el.shift,el.name,byteArrayToBitArray(data,el.num,el.size,el.shift),el.type);
                        break;
                    case "byte":
                        str = String.Format(str,el.num,el.name,data[el.num],el.type);
                        break;
                    case "sbyte":
                        str = String.Format(str,el.num,el.name,(sbyte)data[el.num],el.type);
                        break;
                    case "ushort":
                        str = String.Format(str,el.num,el.name,BitConverter.ToUInt16(data,el.num),el.type);
                        break;
                    case "short":
                        str = String.Format(str,el.num,el.name,BitConverter.ToInt16(data,el.num),el.type);
                        break;
                    case "uint":
                        str = String.Format(str,el.num,el.name,BitConverter.ToUInt32(data,el.num),el.type);
                        break;
                    case "int":
                        str = String.Format(str,el.num,el.name,BitConverter.ToInt32(data,el.num),el.type);
                        break;
                    case "long":
                        str = String.Format(str,el.num,el.name,BitConverter.ToInt64(data,el.num),el.type);
                        break;
                    case "ulong":
                        str = String.Format(str,el.num,el.name,BitConverter.ToUInt64(data,el.num),el.type);
                        break;
                    case "float":
                        str = String.Format(str,el.num,el.name,BitConverter.ToSingle(data,el.num),el.type);
                        break;
                    case "double":
                        str = String.Format(str,el.num,el.name,BitConverter.ToDouble(data,el.num),el.type);
                        break;
                    case "char":
                        str = String.Format(str,el.num,el.name,byteArrayToChar(data,el.num),el.type);
                        break;
                    case "string":
                        str = String.Format(str,el.num,el.name,byteArrayToString(data,el.num),el.type);
                        break;
                    case "bool":
                        str = String.Format(str_with_shift,el.num,el.shift,el.name,byteArrayToBoolean(data,el.num,el.shift),el.type);
                        break;
                    case "hex":
                        str = String.Format(str, el.num, el.name, byteArrayToHexString(data, el.num,el.size), el.type);
                        break;
                }
            }
            rtf.Text = sb.ToString();
        }

        private object byteArrayToBoolean(byte[] data, ushort start,byte shift)
        {
            System.Collections.BitArray bits = new System.Collections.BitArray(data);
            return bits[start * 8 + shift];
        }

        private object byteArrayToString(byte[] data, ushort start)
        {
            StringBuilder result = new StringBuilder();
            for (int i = start; i < data.Length ; i+=2)
            {
                char c = Convert.ToChar(BitConverter.ToUInt16(data,i));
                if (c == '\0') break;
                if (char.IsControl(c))
                    result.Append('.');
                else
                    result.Append(c);
            }
            return result.ToString();
        }

        private static string byteArrayToHexString( byte[] data,int start,int length)
        {
            StringBuilder result = new StringBuilder();
            for (int i = start; i < data.Length && i < start + length; i++)
                result.AppendFormat("{0:X2} ", data[i]);
            return result.ToString();
        }
        private static string byteArrayToCharArray( byte[] data,int start,int length)
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
        private static string byteArrayToBitArray(byte[] data,int start,int length,byte shift)
        {
            System.Collections.BitArray bits = new System.Collections.BitArray(data);
            StringBuilder sb = new StringBuilder();
            for (int i = start*8+shift; i < bits.Count && i < start + length; i++)
                if (bits[i])
                    sb.Append(1);
                else
                    sb.Append(0);
            return sb.ToString();
        }

        private static char byteArrayToChar(byte[] data, int start)
        {
            char c = Convert.ToChar(data[start]);
            if (char.IsControl(c))
                return '.';
            return c;
        }
    }
}
