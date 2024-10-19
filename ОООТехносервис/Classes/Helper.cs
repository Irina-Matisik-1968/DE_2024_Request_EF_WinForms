using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ОООТехносервис.Classes
{
    internal class Helper
    {
        public static Model.DBRequests DB { get; set; } //Для связи с БД
        public static Model.User User { get; set; }     //Пользователь
    }
}
