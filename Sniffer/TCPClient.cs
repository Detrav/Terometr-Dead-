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
        internal class tcp_frag
        {
            public ulong seq = 0;
            public ulong len = 0;
            public ulong data_len = 0;
            public byte[] data = null;
            public tcp_frag next = null;
        };

        // holds two linked list of the session data, one for each direction    
        tcp_frag[] frags = new tcp_frag[2];
        // holds the last sequence number for each direction
        ulong[] seq = new ulong[2];
        long[] src_addr = new long[2];
        uint[] src_port = new uint[2];
        //bool empty_tcp_stream = true;
        uint[] tcp_port = new uint[2];
        //int[] bytes_written = new int[2];
        //System.IO.FileStream data_out_file = null; Не нужен т.к. обрабатываем сразу
        bool incomplete_tcp_stream = false;
        bool closed = false;

        public bool IncompleteStream
        {
            get { return incomplete_tcp_stream; }
        }

        /*
        public bool EmptyStream
        {
            get { return empty_tcp_stream; }
        }
         */


        public TcpClient()
        {
            // TODO: Complete member initialization
            reset_tcp_reassembly();
            teraClient = new Client();
        }
        public void Close()
        {
            if (!closed)
            {
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

            reassemble_tcp((ulong)tcpPacket.SequenceNumber, (ulong)tcpPacket.PayloadData.Length,
                tcpPacket.PayloadData, (ulong)tcpPacket.PayloadData.Length, tcpPacket.Syn,
                BitConverter.ToUInt32((tcpPacket.ParentPacket as PacketDotNet.IPv4Packet).SourceAddress.GetAddressBytes(), 0),
                BitConverter.ToUInt32((tcpPacket.ParentPacket as PacketDotNet.IPv4Packet).DestinationAddress.GetAddressBytes(), 0),
                (uint)tcpPacket.SourcePort, (uint)tcpPacket.DestinationPort);
        }

        private void write_packet_data(uint port, byte[] data)
        {
            // ignore empty packets
            if (data.Length == 0) return;
            //Сервер -> Клиент
            if (port == 7801) teraClient.recv((byte[])data.Clone());
            else teraClient.send((byte[])data.Clone());
        }

        private void reassemble_tcp(ulong sequence, ulong length, byte[] data,
        ulong data_length, bool synflag, long net_src,
        long net_dst, uint srcport, uint dstport)
        {
            long srcx/*, dstx*/;
            int src_index, j;
            bool first = false;
            ulong newseq;
            tcp_frag tmp_frag;

            src_index = -1;

            /* Now check if the packet is for this connection. */
            srcx = net_src;
            //dstx = net_dst;

            /* Check to see if we have seen this source IP and port before.
          (Yes, we have to check both source IP and port; the connection
          might be between two different ports on the same machine.) */
            for (j = 0; j < 2; j++)
            {
                if (src_addr[j] == srcx && src_port[j] == srcport)
                {
                    src_index = j;
                }
            }
            /* we didn't find it if src_index == -1 */
            if (src_index < 0)
            {
                /* assign it to a src_index and get going */
                for (j = 0; j < 2; j++)
                {
                    if (src_port[j] == 0)
                    {
                        src_addr[j] = srcx;
                        src_port[j] = srcport;
                        src_index = j;
                        first = true;
                        break;
                    }
                }
            }
            if (src_index < 0)
            {
                throw new Exception("ERROR in reassemble_tcp: Too many addresses!");
            }

            if (data_length < length)
            {
                incomplete_tcp_stream = true;
            }

            /* now that we have filed away the srcs, lets get the sequence number stuff
          figured out */
            if (first)
            {
                /* this is the first time we have seen this src's sequence number */
                seq[src_index] = sequence + length;
                if (synflag)
                {
                    seq[src_index]++;
                }
                /* write out the packet data */

                write_packet_data(src_port[src_index], data);
                return;
            }
            /* if we are here, we have already seen this src, let's
          try and figure out if this packet is in the right place */
            
            //Если пришедший пакет находится левее чем ожидалось
            if (sequence < seq[src_index])
            {
                /* this sequence number seems dated, but
              check the end to make sure it has no more
              info than we have already seen */
                //Получаем номер, который будем ожидать в следующий раз
                newseq = sequence + length;
                //Если ожидаемый номер в след пакете больше чем ожидалось сейчас
                if (newseq > seq[src_index])
                {
                    ulong new_len;

                    /* this one has more than we have seen. let's get the
               payload that we have not seen. */
                    //Ввычисляем ожидаемый сейчас номер - полученый номер =  длина, чтобы отрезать кусок
                    new_len = seq[src_index] - sequence;
                    //Непойму зачем вторая проверка, дело в том что автор задумывал по другому
                    //Поидее здесь мы проверяем длину пакета из ipPacket но т.к. проверка
                    //происходить выше здесь всегда будет false
                    if (data_length <= new_len)
                    {
                        data = null;
                        data_length = 0;
                        incomplete_tcp_stream = true;
                    }
                    else
                    {
                        //получили перекрытие в пакетах и нужно отрезать кусок, для этого
                        //вычислаем размер не перекрытия
                        //и копируем неперекрывающую часть
                        data_length -= new_len;
                        byte[] tmpData = new byte[data_length];
                        for (ulong i = 0; i < data_length; i++)
                            tmpData[i] = data[i + new_len];
                        //данные присваеваем
                        data = tmpData;
                    }
                    //Отрезали кусок и получили пакет нужной длины :)
                    sequence = seq[src_index];
                    length = newseq - seq[src_index];

                    /* this will now appear to be right on time :) */
                }
                //И всётаки полностью перекрытые пакеты нужно выбросить
                else return;
            }
            //Проверяем является ли номер пакета, который мы ожидали
            if (sequence == seq[src_index])
            {
                /* right on time */
                seq[src_index] += length;
                if (synflag) seq[src_index]++;//Опять же не пойму такой пакет не дойдёт до сюда
                if (data != null)//Такой пакет тоже не дойдёт, всегда будет тру
                {
                    write_packet_data(src_port[src_index], data);
                }
                /* done with the packet, see if it caused a fragment to fit */
                //А тут поидее мы проверяем все фрагменты пакетов скопленых программой, а вдруг они дополнят поток
                while (check_fragments(src_index))
                    ;
            }
            else
            {
                //поидее мы получили пакет дальше чем ожидалось, и поэтому у нас получается окно в потоке
                /* out of order packet */
                //Всегда тру т.к. первое условие отсеивается выше, а второе условия отсеивается на else
                if (data_length > 0 && sequence > seq[src_index])
                {
                    //Создаём фрагмент изза окна
                    tmp_frag = new tcp_frag();
                    tmp_frag.data = data;
                    tmp_frag.seq = sequence;
                    tmp_frag.len = length;
                    tmp_frag.data_len = data_length;

                    //а тут создаём обратный список
                    if (frags[src_index] != null)
                    {
                        tmp_frag.next = frags[src_index];
                    }
                    else
                    {
                        tmp_frag.next = null;
                    }
                    frags[src_index] = tmp_frag;
                }
            }
        }
        //На данном моменте всё верно, единственно что не проверено мной, это 4 ситуация, когда получаем пакеты с окнами, а потом заклёпки окон.
        bool check_fragments(int index)
        {
            //Автор явно брал код с языка "С", потому что в дотнете уже продуманы структуры и ими удобнее пользоваться,
            //Не думаю, что речь заходила об скорости, т.к. предыдущая функция сильно не оптимизированы
            //храним данные о текущем и предыдущем пакете
            tcp_frag prev = null;
            tcp_frag current;
            current = frags[index];
            //Пока не доходим до конца обратного списка do while может был бы лучше
            while (current != null)
            {
                //Если пакет входит тютилька в тютильку, а как же случай где пакет не входит или перекрывает?
                if (current.seq == seq[index])
                {
                    /* this fragment fits the stream */
                    // Да такой точно заполнит поток, и если данные ещё существуют то записываем их
                    if (current.data != null)
                    {
                        write_packet_data(src_port[index], current.data);
                    }
                    seq[index] += current.len;
                    //зачемто храним предыдущий
                    if (prev != null)
                    {
                        prev.next = current.next;
                    }
                    else
                    {
                        frags[index] = current.next;
                    }
                    current.data = null;
                    current = null;
                    return true;
                }
                prev = current;
                current = current.next;
                //Отлично, всё ок за 2 исключениями
                //1) если у нас пакет перекрывает часть, а такие я встречал(удивительно что я встретил такой случайно, у него ещё длина была невообразимая 1360, и он перекрывал предыдущий и последующие потоки)
                //2) Мы не анализируем все части, а только 1 раз проходим по ним, нужен do while и флаг, или же while true и ретёрн.
            }
            return false;
        }

        void reset_tcp_reassembly()
        {
            tcp_frag current, next;
            int i;

            //empty_tcp_stream = true;
            incomplete_tcp_stream = false;
            for (i = 0; i < 2; i++)
            {
                seq[i] = 0;
                src_addr[i] = 0;
                src_port[i] = 0;
                tcp_port[i] = 0;
                //bytes_written[i] = 0;
                current = frags[i];
                while (current != null)
                {
                    next = current.next;
                    current.data = null;
                    current = null;
                    current = next;
                }
                frags[i] = null;
            }
        }
    }
}
