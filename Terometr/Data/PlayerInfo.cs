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
        public double dD;// часть урона
        public double n;


        internal void addDamage(ushort type, uint value)
        {
            if (type == 1)
            {

                double delay = (DateTime.Now - last).TotalMilliseconds / 1000.0;
                if (delay > 15)
                {
                        n = 0.0;
                        dps = 0;
                        damage = 0;
                        last = DateTime.Now;
                        dD = 0.0;
                        return;
                }
                n+=1;
                dD += value;
                damage += value;
                if (delay <= 0.0)
                    return;
                dps = dps + (dD/delay - dps) / n;
                dD = 0;
                last = DateTime.Now;
            }
        }
    }
}
