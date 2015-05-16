using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terometr.Data
{
    class PlayerInfo
    {
        /*
         * Нужны след переменный:
         * 1) Имя
         * 2) Номер
         * 3) В пати(рейде)
         * 4) Урон в скунду
         * 4.1) Количество урона? :)
         * 5) Шанс крита
         * 6) Последняя активность чтобы считать таймаут метра
         * 7) Нужно удалить?
         * Не знаю
         */
        public string name;
        public ulong id;
        public double dps;
        public ulong damage;
        public DateTime last = DateTime.MinValue;

        internal void addDamage(ushort type, uint value)
        {
            if (type == 1)
            {
                if (dps == 0)
                {
                    dps = value;
                    damage += value;
                    last = DateTime.Now;
                    return;
                }
                double delay = (DateTime.Now - last).TotalMilliseconds/1000.0;
                if(delay< 0.1)
                {
                    int test;
                }
                /*if (delay > 15)
                {
                    damage += value;
                    //last = DateTime.Now;
                    return;
                }*/
                dps = dps + ((double)value - dps) / delay;
                damage += value;
                //last = DateTime.Now;
            }
        }
    }
}
