using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.Terometr.TeraApi.Data
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

        internal bool addDamage(ushort type, uint value, int behavior ,double timeout, ulong selfId)
        {
            if(type != 1) return false;
            switch(behavior)
            {
                case 0://Добавляем ДПС всегда и по таймауту выходим из боя
                    addDamage(value, timeout);
                    return false;
                case 1://Сбрасываем значение ДПС каждый раз при выходе из боя
                    addDamageWithReSet(value, timeout);
                    return false;
                case 2://Сбрасываем все занчения ДПС :(
                    if(selfId == id)
                    return true;
                    addDamage(value, timeout);
                    return false;
            }
            return false;
        }

        private void addDamage(uint value, double timeout)
        {

            double delay = (DateTime.Now - last).TotalMilliseconds / 1000.0;
            if (delay > timeout)
            {
                damage += value;
                first += DateTime.Now - last - TimeSpan.FromSeconds(timeout);
                last = DateTime.Now;
                return;
            }
            damage += value;
            double dt = (DateTime.Now - first).TotalMilliseconds / 1000.0;
            if (dt <= 0.0)
                return;
            dps = damage / dt;
            last = DateTime.Now;
        }
        private void addDamageWithReSet(uint value, double timeout)
        {

            double delay = (DateTime.Now - last).TotalMilliseconds / 1000.0;
            if (delay > timeout)
            {
                dps = value;
                damage = value;
                last = DateTime.Now;
                first = DateTime.Now - TimeSpan.FromSeconds(timeout);
                return;
            }
            damage += value;
            double dt = (DateTime.Now - first).TotalMilliseconds / 1000.0;
            if (dt <= 0.0)
                return;
            dps = damage / dt;
            last = DateTime.Now;
        }
        internal void Clear()
        {
            last = DateTime.MinValue;
            first = DateTime.MinValue;
            dps = 0;
            damage = 0;
        }

    }
}
