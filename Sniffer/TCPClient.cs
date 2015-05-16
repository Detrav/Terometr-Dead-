using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sniffer
{
    internal class TcpClient
    {
        //Основное
        public Client teraClient {get; private set;}
        //Сделаем как описанно в http://www.theforce.dk/hearthstone/, спасибо автору
        //Пришлось код писать с 0, т.к. на http://www.theforce.dk/hearthstone/ ошибка :(
        internal struct tcp_frag
        {
            public uint seq;
            public int len;
            public byte[] data;
        };
 
        List<tcp_frag>[] frags = new List<tcp_frag>[2];
        uint[] seq = new uint[2];
        ushort[] src_port = new ushort[2];
        bool closed = false;


        public TcpClient()
        {
            // TODO: Complete member initialization
            frags[0] = new List<tcp_frag>();
            frags[1] = new List<tcp_frag>();
            reset_tcp_reassembly();
            teraClient = new Client();
        }
        public void Close()
        {
            if (!closed)
            {
                if(tw!=null)
                {
                    try{tw.Flush();}
                    catch { }
                    try { tw.Close(); }
                    catch { }
                    tw = null;
                }
                reset_tcp_reassembly();
                closed = true;
            }
        }

        ~TcpClient()
        {
            Close();
        }

        internal void reConstruction(PacketDotNet.TcpPacket tcpPacket)
        {
            if (tcpPacket.PayloadData == null || tcpPacket.PayloadData.Length == 0) return;

            reassemble_tcp(
                tcpPacket.SequenceNumber,//Номер пакета
                tcpPacket.PayloadData.Length,//Длина данных
                (byte[])tcpPacket.PayloadData.Clone(),//Копия данных пакета, т.к. я уверен могут быть проблемы
                tcpPacket.SourcePort, tcpPacket.DestinationPort//Номера портов, т.к. мне нужен реконструктор а не снифер
                );
        }



        private void write_packet_data(uint port, byte[] data)
        {
            // ignore empty packets
            if (data.Length == 0) return;
            //Сервер -> Клиент
            if (port == 7801) teraClient.recv((byte[])data.Clone());
            else teraClient.send((byte[])data.Clone());
        }

        private void reassemble_tcp(uint sequence, int length, byte[] data, ushort srcport, ushort dstport)
        {
            int src_index = -1;//Номер обрабатываемого случая
            bool first = false;//Первые пакеты, если да то присваеваем значения
            //Теперь идём по чужой фукнции и пишем свой вариант
            //сэкономим место сделаем без скобок
            for (int j = 0; j < 2; j++)
                if (src_port[j] == srcport)
                    src_index = j;
            //Если на нашли нужный случай
            if (src_index < 0)
                for (int j = 0; j < 2; j++)
                    if (src_port[j] == 0)
                    {
                        src_port[j] = srcport;
                        src_index = j;
                        first = true;
                        break;
                    }
            //оставим throw, хотя я думаю такой ситуации не может быть
            if (src_index < 0) throw new Exception("ERROR in reassemble_tcp: Too many ports!");

            //Далее возможны 4 случая
            //1)У нас пакет первый
            //2)Начало полученного пакета перекрывается с уже имеющимися
            //3)Всё ок и пакет можно добавить в поток данных
            //4)Получили окно и тогда нужно кидать пакет в список
            if (first)//1
            {
                debug("Первый пакет от {0} с номером {1} и длиной {2}", srcport, sequence, length);
                seq[src_index] = sequence + (uint)length;
                write_packet_data(src_port[src_index], data);
                return;
            }
            if (sequence < seq[src_index])//2
            {
                //Нужно отрезать кусок или выкинуть пакет
                //Если пакет оказался меньше чем нужно то выкидваем его
                if (sequence + length <= seq[src_index])
                {
                    debug("Лишний пакет от {0} с номером {1} и длиной {2}", srcport, sequence, length);
                    return;
                }
                //иначе вычислваем длину перекрывающегося куска
                uint new_len = seq[src_index] - sequence;

                length -= (int)new_len;
                byte[] tmpData = new byte[length];
                for (int i = 0; i < length; i++)
                    tmpData[i] = data[i + new_len];
                //данные присваеваем
                data = tmpData;
                //Теперь нужно подправить данные, чтобы прога думала что пакет подходит
                sequence = seq[src_index];
                debug("Обрезаный пакет от {0} с номером {1} и длиной {2}", srcport, sequence, length);
            }
            if (sequence == seq[src_index])//3
            {
                //debug("Обычный пакет от {0} с номером {1} и длиной {2}",srcport,sequence,length);
                seq[src_index] += (uint)length;
                write_packet_data(src_port[src_index], data);
                while (check_fragments(src_index)) ;//И опять нужно может переписать см. предыдущие комиты
                return;
            }
            //остался 1 случай if(sequence > seq[src_index])//4
            debug("Оконный пакет от {0} с номером {1} и длиной {2}", srcport, sequence, length);
            frags[src_index].Add(new tcp_frag() { data = data, len = length, seq = sequence });
        }

        TextWriter tw = new StreamWriter(String.Format("{0}.debug", DateTime.Now.ToString("MMM_dd_HH_mm_ss")));
        //TextWriter tw = null;
        private void debug(string str, params object[] strs)
        {
            if (tw != null)
            {
                tw.WriteLine("{0} {1}",DateTime.Now.ToString("HH:mm:ss"),String.Format(str, strs));
                tw.Flush();
            }
        }

        bool check_fragments(int index)
        {
            tcp_frag frag;
            for(int i = 0; i< frags[index].Count;i++)
            {
                frag = frags[index][i];
                //и опять несколько случаев (3) :)
                if (frag.seq < seq[index])//1 - Перекрывает
                {
                    if (frag.seq + frag.len <= seq[index])
                    {
                        debug("Check лишний пакет от {0} с номером {1} и длиной {2}", src_port[index], frag.seq, frag.len);
                        frags[index].RemoveAt(i);
                        return true;
                    }
                    uint new_len = seq[index] - frag.seq;

                    frag.len -= (int)new_len;
                    byte[] tmpData = new byte[frag.len];
                    for (int j = 0; j < frag.len; j++)
                        tmpData[j] = frag.data[j + new_len];
                    frag.data = tmpData;
                    frag.seq = seq[index];
                    debug("Check обрезаный пакет от {0} с номером {1} и длиной {2}", src_port[index], frag.seq, frag.len);
                }
                if (frag.seq == seq[index])//2 - Подходить
                {
                    debug("Check нашёлся пакет от {0} с номером {1} и длиной {2}", src_port[index], frag.seq, frag.len);
                    seq[index] += (uint)frag.len;
                    write_packet_data(src_port[index], frag.data);
                    frags[index].RemoveAt(i);
                    return true;
                }
                //3 - Окно :(
                debug("Check оконный пакет от {0} с номером {1} и длиной {2}", src_port[index], frag.seq, frag.len);
            }
            return false;
        }

        
        void reset_tcp_reassembly()
        {
            for (int i = 0; i < 2; i++)
            {
                seq[i] = 0;
                src_port[i] = 0;
                frags[i].Clear();
            }
        }
    }
}
