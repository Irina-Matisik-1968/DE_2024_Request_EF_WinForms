using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ОООТехносервис.Classes;
using ОООТехносервис.Model;

namespace ОООТехносервис
{
    public partial class Authorization : Form
    {
        public Authorization()
        {
            InitializeComponent();
            try
            {
                Helper.DB = new Model.DBRequests();
                //MessageBox.Show("Удалось подключиться к БД");
            }
            catch
            {
                MessageBox.Show("Не удалось подключиться к БД");
                Environment.Exit(0);    
            }
            panelTitle.BackColor = Color.FromArgb(0x67, 0xBA, 0x80);
        }
        /// <summary>
        /// Завершить приложение
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bntExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Авторизация
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAuto_Click(object sender, EventArgs e)
        {
            string login = textBoxLogin.Text;
            string password=textBoxPassword.Text;
            StringBuilder sb = new StringBuilder(); 
            Helper.User=null;
            //Получить единственную запись, подходящую под условие или не получить совсем
            Helper.User = Helper.DB.User.Where(u=>u.UserLogin==login&& u.UserPassword==password).FirstOrDefault();
            if (Helper.User==null)      //Запись не найдена
            {
                MessageBox.Show("Такой пользователь не зарегистрирован", "Результат авторизации", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else               //Запись найдена - выводим информацию о пользователе
            {
                sb.AppendLine("Система приветствует Вас, " + Helper.User.UserFullName);
                sb.AppendLine("Вам доступен функционал с ролью " + Helper.User.Role.RoleName);
            }
            MessageBox.Show(sb.ToString(), "Результат авторизации", MessageBoxButtons.OK,MessageBoxIcon.Information);
            //Отобразить окно с заявками
            this.Hide();
            new View.ListRequests().ShowDialog();
            this.Show();
        }
    }
}
