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
        }
        /* 
         * + Обязательно, - Лишний, * Опциональный
         * Поддерживаемые типы: |Номер|Размер|Имя|Цвет|Фон|Резерв
         * 1) bit array         |  +  |  +   | + | *  | * |
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
         * 14) bool             |  +  |  -   | + | *  | * |
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
                        elementsList.Add(pe);
                    }
                    elements = elementsList.ToArray();
                }
            }
        }
    }
}
