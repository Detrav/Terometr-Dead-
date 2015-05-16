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
        public DateTime first = DateTime.MinValue;


        internal void addDamage(ushort type, uint value)
        {
            if (type == 1)
            {
                double delay = (DateTime.Now - last).TotalMilliseconds / 1000.0;
                if (delay > 15)
                {
                        dps = 0;
                        damage = 0;
                        last = DateTime.Now;
                        first = DateTime.Now;
                        return;
                }
                damage += value;
                double dt = (DateTime.Now - first).TotalMilliseconds / 1000.0;
                if (dt <= 0.0)
                    return;
                dps = damage /dt;
                last = DateTime.Now;
            }
        }
    }
}
