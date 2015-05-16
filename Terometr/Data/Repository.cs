using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terometr.Data
{
    //Да да, у него должно быть другое применение, однако я захотел назвать синглтон так
    class Repository
    {
        //Помню както первый раз услышал про синглтон, долго не мог понять с чем его едять
        //про патерны вобще молчу, хоть бы в университете когда учили расказали про них
        //И вот я прихожу на собеседование, и меня спрашивают, какие патерны знаете...... Чё?
        //Сейчас я знаю порядка 10 вариантов синглтона :)
        //Такой вариант предлагают мелкомягкие, хотя я встречал более рациональный, этот мне нравиться
        private static volatile Repository instance;
        private static object syncRoot = new object();

        public static Repository Instance
        {
            get
            {
                if(instance == null)
                {
                    lock(syncRoot)
                    {
                        if (instance == null)
                            instance = new Repository();
                    }
                }
                return instance;
            }
        }


        private Repository()
        {

        }

    }
}
