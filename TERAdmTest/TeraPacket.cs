using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TERAdmTest
{
    class TeraPacket
    {
        public ushort len;
        public ushort opCode;
        public byte[] data;
        public bool recv;
        public const ushort S_SPAWN_NPC = 0xf278;
        public const ushort S_SPAWN_USER = 0xf478;
        public const ushort S_EACH_SKILL_RESULT = 0xd925;

        public TeraPacket(ushort length, ushort opCode1, byte[] p, bool r)
        {
            // TODO: Complete member initialization
            this.len = length;
            this.opCode = opCode1;
            this.data = p;
            this.recv = r;
        }

        public override string ToString()
        {
            if (recv)
                return String.Format("R  {0,6} {1,6} {2}", len, opCode, BitConverter.ToString(data).Replace("-", ""));
            else
                return String.Format(" S {0,6} {1,6} {2}", len, opCode, BitConverter.ToString(data).Replace("-", ""));
            /*
            switch(opCode)
            { 
                case 0xD925:
                    Console.WriteLine("damage: {0,16} - {1,5}", BitConverter.ToUInt16(data, 28), BitConverter.ToUInt16(data, 48));
                    return String.Format("damage: {0,16} - {1,5}", BitConverter.ToUInt16(data, 28), BitConverter.ToUInt16(data, 48));
                default: return "";
                    if (recv)
                        return String.Format("recv     {0,6} {1,6} {2}", len, opCode, BitConverter.ToString(data).Replace("-", ""));
                    else
                        return String.Format("    send {0,6} {1,6} {2}", len, opCode, BitConverter.ToString(data).Replace("-", ""));
            }*/
        }

        internal void log(System.IO.TextWriter tw)
        {
             switch(opCode)
             {
                     /* D925:
                      *  0, 2 size
                      *  2, 2 type
                      *  4, 2 visual
                      *  6, 2 shift
                      *  8, 8 attacker
                      * 16, 8 target
                      * 24, 4 modelId creature or effect
                      * 28, 4 skill id
                      * 32, 8 some q ( don't know)
                      * 40, 4 attak id
                      * 44, 4 shift 0
                      * 48, 4 damage
                      * 52, 4 crit
                      * more dont know
                      */
                 case 0xD925:
                     tw.WriteLine("from {0,16} to {1,16} skill {2,8} by {3,10}",
                         BitConverter.ToString(data, 8, 8).Replace("-", ""),//Кто
                         BitConverter.ToString(data, 16, 8).Replace("-", ""),//Кого
                         BitConverter.ToString(data, 28, 4).Replace("-", ""),//Скилл
                         BitConverter.ToUInt16(data, 48));//Урон
                     break;
                 case 0xF478:
                     tw.WriteLine("find player {0,16} name {1}",
                         BitConverter.ToString(data, 34, 8).Replace("-", ""),
                         readName(data));
                     break;
                 default:
                     break;
             }
             tw.Flush();
        }
        internal void log2(System.IO.TextWriter tw)
        {
            switch (opCode)
            {
                case 0xF478:
                    tw.WriteLine(BitConverter.ToString(data).Replace("-", ""));
                    break;
                default:
                    break;
            }
            tw.Flush();
        }

        internal string readName(byte[] data)
        {
            //return BitConverter.st (data,256);
            
            int k = 256; StringBuilder sb = new StringBuilder();
            int l = BitConverter.ToChar(data,k);
            while( l!=0)
            {                
                l = BitConverter.ToUInt16(data, k);
                sb.Append(BitConverter.ToChar(data, k));
                k += 2;
            }
            return sb.ToString();
            
        }
    }
}
