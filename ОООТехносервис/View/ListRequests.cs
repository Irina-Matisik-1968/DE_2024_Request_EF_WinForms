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

namespace ОООТехносервис.View
{
    public partial class ListRequests : Form
    {
        //Все заявки
        List<Model.Request> listrequests = new List<Model.Request>();       //Из таблицы
        //List<Model.ViewAllRequest> listrequests = new List<Model.ViewAllRequest>();   //Из представления

        public ListRequests()
        {
            InitializeComponent();
            panelTitle.BackColor = Color.FromArgb(0x67, 0xBA, 0x80);
        }

        private void bntExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //Метод показать все заявки с учетом параметров
        private void ShowRequests()
        {
            //Получаем список всех заявок
            listrequests = Helper.DB.Request.ToList();

            //listrequests = Helper.DB.ViewAllRequest.ToList();
            //dataGridViewRequests.DataSource = listrequests;

            //Проводим поиск
            if (!string.IsNullOrEmpty(textBoxSearch.Text))  //Непустое поле для поиска
            {
                //Поиск по частичному совпадению
                listrequests = listrequests.Where(r => r.RequestId.ToString().Contains(textBoxSearch.Text)).ToList();
            }
            //Проводим фильтрацию по статусу
            if (comboBoxFilter.SelectedIndex != 0)   //Не статус "Все статусы"
            {
                listrequests = listrequests.Where(r => r.RequestStatusId == comboBoxFilter.SelectedIndex).ToList();

            }
            //Заявки заказчика, вошедшего в систему
            if (Helper.User.UserRoleId == 1)
            {
                listrequests = listrequests.Where(r => r.RequestUserId == Helper.User.UserId).ToList();
            }
            //Заявки мастера, вошедшего в систему
            if (Helper.User.UserRoleId == 2)
            {
                listrequests = listrequests.Where(r => r.RequestMasterId == Helper.User.UserId).ToList();
            }

            //Подготавливаем таблицу для вывода
            dataGridViewRequests.Rows.Clear();  //Очищаем все строкив таблице
            //Перебор всех записей
            for (int i = 0; i < listrequests.Count; i++)
            {
                dataGridViewRequests.Rows.Add();    //Добавить пустую строку
                //Заполлняем столбцы каждой записи
                dataGridViewRequests.Rows[i].Cells[0].Value = listrequests[i].RequestId;
                dataGridViewRequests.Rows[i].Cells[1].Value = listrequests[i].RequestDate.ToShortDateString();
                dataGridViewRequests.Rows[i].Cells[2].Value = listrequests[i].Equipment.EquipmentName;
                dataGridViewRequests.Rows[i].Cells[3].Value = listrequests[i].User1.UserFullName;
                dataGridViewRequests.Rows[i].Cells[4].Value = listrequests[i].Status.StatusName;
                dataGridViewRequests.Rows[i].Cells[5].Value = listrequests[i].User.UserFullName;
                dataGridViewRequests.Rows[i].Cells[6].Value = listrequests[i].Stage.StageName;
            }
            //Настраиваем таблицу
            dataGridViewRequests.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;    //По ширине
            dataGridViewRequests.AllowUserToAddRows = false;        //Без добавления строк
            dataGridViewRequests.ReadOnly = true;           //Только для просмотра
            dataGridViewRequests.SelectionMode = DataGridViewSelectionMode.FullRowSelect;//Выделение всей строки таблицы
        }

        /// <summary>
        /// Событие отображение формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListRequests_Shown(object sender, EventArgs e)
        {
            //Настройка ComboBox для отображения списка статусов
            //Список статусов
            List<Model.Status> statuses = Helper.DB.Status.ToList();
            //Добавить новый статус, которого нет в таблице - Все статусы
            Model.Status statusAll =new Model.Status();
            statusAll.StatusId = 0;
            statusAll.StatusName = "Все статусы";
            statuses.Insert(0,statusAll);
            comboBoxFilter.DataSource = statuses;
            comboBoxFilter.SelectedIndex = 0;
            comboBoxFilter.DisplayMember = "StatusName";
            comboBoxFilter.ValueMember = "StatusId";
            //Отображение списка заявок на ремонт в таблице с учетом критериев выбора
            ShowRequests();
            //Сначала все кнопки невидимы
            buttonNew.Visible = buttonEdit.Visible = buttonReport.Visible = false;
            //Отобразить нужные кнопки с учетом роли вошедшего пользователя
            switch (Helper.User.UserRoleId)
            {
                case 1:         //Клиент
                    break;
                case 2:         //Мастер
                    buttonEdit.Visible = true;
                    break;
                case 3:         //Оператор
                    buttonNew.Visible = buttonEdit.Visible = true;  
                    break;
                case 4:         //Менеджер
                    buttonReport.Visible = true;
                    break;
                default:
                    break;
            }
        }

        //Поиск по номеру
        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {
            ShowRequests();
        }

        /// <summary>
        /// Фильтраия по статусу
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowRequests();
        }

        /// <summary>
        /// Новая заявка переход на форму режимом "Добавить" и пустой заявкой
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonNew_Click(object sender, EventArgs e)
        {
            //Отобразить окно одной завки для добавления новой заявки
            View.Request request = new View.Request(0, 0);
            this.Hide();
            request.ShowDialog();
            this.Show();
        }
        /// <summary>
        /// Редактирование выбранной заявки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonEdit_Click(object sender, EventArgs e)
        {
            //Проверка, что для редктирования выбрана строка в тблице
            if (dataGridViewRequests.SelectedCells.Count!=0)
            {
                //Отобразить окно одной заявки для передактирвоания выбранной 
                View.Request request = new View.Request(Helper.User.UserRoleId, (int)dataGridViewRequests.CurrentRow.Cells[0].Value);
                this.Hide();
                request.ShowDialog();
                this.Show();
            }
            else
            {
                MessageBox.Show("Вы не выбрали заявку для редактирования");
                return;
            }
        }
        /// <summary>
        /// Отображение окна после внесения изменения в БД
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListRequests_Activated(object sender, EventArgs e)
        {
            ShowRequests();
        }

        /// <summary>
        /// Отображение окна отчетов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonReport_Click(object sender, EventArgs e)
        {
            View.Reports reports = new View.Reports();
            this.Hide();
            reports.ShowDialog();
            this.Show();
        }
    }
}
