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
        string name;
        ulong id;
        bool inParty;
        double dps;
        ulong damage;
        double crit;
        DateTime last;
        bool delete;
    }
}
