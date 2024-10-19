using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ОООТехносервис.Classes;
using ОООТехносервис.Model;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ОООТехносервис.View
{
    public partial class Request : Form
    {
        int requestId;  //Номер переданной заявки
        int mode;       //0 - новая, 2 - редактирование мастером, 3 - редактирование оператором
        Model.Request request;  //Сама заявка

        public Request()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Конструктор с параметрами
        /// </summary>
        /// <param name="mode">Режим работы окна</param>
        /// <param name="requestId">0 или номер выбранной заявки для редактирования</param>
        public Request(int mode,int requestId)
        {
            InitializeComponent();
            this.mode = mode;
            this.requestId = requestId;
            if (mode == 0)
            {
                request = new Model.Request();      //Создать новую пустую заявку, с которой будем работать
            }
            else
            {
                //Создать заявку с существующими данными
                request = Helper.DB.Request.Where(r => r.RequestId == requestId).FirstOrDefault();
            }
            //textBoxID.DataBindings.Add("Text", request, "RequestId");
            //textBoxDate.DataBindings.Add("Text", request, "RequestDate");
            //textBoxDescription.DataBindings.Add("Text", request, "RequestDescription");
            //textBoxTime.DataBindings.Add("Text", request, "RequestTime");
            //textBoxComment.DataBindings.Add("Text", request, "RequestComment");
        }

        private void bntExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// Вспомогательный метод для заполнения списков из таблиц БД
        /// </summary>
        /// <typeparam name="T">Класс таблицы</typeparam>
        /// <param name="comboBox">Элемент интерфейса для списка</param>
        /// <param name="displayMember">Название поля для отображения</param>
        /// <param name="valueMember">Название поля для получения значения</param>
        /// <param name="list">Формируемый список</param>
        private void SetComboBox<T>(System.Windows.Forms.ComboBox comboBox, string displayMember, string valueMember, List<T> list)
        {
            comboBox.DataSource = list;
            comboBox.DisplayMember = displayMember;
            comboBox.ValueMember = valueMember;
            comboBox.Enabled = false;
        }

        /// <summary>
        /// При отображении формы подготовить все элементы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Request_Shown(object sender, EventArgs e)
        {
            //Настройка списков из БД
            SetComboBox<Model.Equipment>(comboBoxEquipment, "EquipmentName", "EquipmentId", Helper.DB.Equipment.ToList());
            SetComboBox<Model.User>(comboBoxClient, "UserFullName", "UserId", Helper.DB.User.Where(u => u.UserRoleId == 1).ToList());
            SetComboBox<Model.User>(comboBoxMaster, "UserFullName", "UserId", Helper.DB.User.Where(u => u.UserRoleId == 2).ToList());
            SetComboBox<Model.Priority>(comboBoxPriory, "PriorityName", "PriorityId", Helper.DB.Priority.ToList());
            SetComboBox<Model.Stage>(comboBoxStage, "StageName", "StageId", Helper.DB.Stage.ToList());
            SetComboBox<Model.Status>(comboBoxStatus, "StatusName", "StatusId", Helper.DB.Status.ToList());
            SetComboBox<Model.Problem>(comboBoxProblem, "ProblemName", "ProblemId", Helper.DB.Problem.ToList());

            //List<Model.Equipment> listEquipments = Helper.DB.Equipment.ToList();
            //comboBoxEquipment.DataSource = listEquipments;
            //comboBoxEquipment.DisplayMember = "EquipmentName";
            //comboBoxEquipment.ValueMember = "EquipmentId";

            //Определить доступные для изменения элементы интерфейса
            textBoxID.Enabled = textBoxDate.Enabled = false;
            if (mode==0)        //Новая заявка
            {
                textBoxComment.Enabled = textBoxTime.Enabled = false;
                textBoxID.Text = "0";
                textBoxDate.Text = DateTime.Now.ToShortDateString();
                comboBoxEquipment.Enabled = comboBoxClient.Enabled = comboBoxMaster.Enabled =
                    comboBoxPriory.Enabled = textBoxDescription.Enabled = comboBoxProblem.Enabled = true;
                textBoxTime.Text = "0";
                comboBoxStatus.SelectedValue=2;
                comboBoxStage.SelectedValue=2;
            }
            else            //Редактирование выбранной заявки
            {
                //Заполнить все элементы данными редактируемой заявки
                textBoxID.Text = requestId.ToString();
                textBoxDate.Text = request.RequestDate.ToShortDateString();
                comboBoxEquipment.SelectedValue = request.RequestEquipmentId;
                comboBoxClient.SelectedValue = request.RequestUserId;
                textBoxDescription.Text = request.RequestDescription;
                comboBoxMaster.SelectedValue = request.RequestMasterId;
                textBoxTime.Text = request.RequestTime.ToString();
                comboBoxPriory.SelectedValue = request.RequestPriorityId;
                comboBoxStatus.SelectedValue = request.RequestStatusId;
                comboBoxStage.SelectedValue = request.RequestStageId;
                textBoxComment.Text = request.RequestComment;
                comboBoxProblem.SelectedValue = request.RequestProblemId;
                if (mode == 3)      //Редактирует Оператор
                {
                    comboBoxMaster.Enabled = true;
                }
                else               //Редактирует Мастер
                {
                    textBoxDescription.Enabled = textBoxTime.Enabled = comboBoxStage.Enabled = textBoxComment.Enabled = true;
                }
            }
        }
        /// <summary>
        /// Фиксировать изменения в БД
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonFixed_Click(object sender, EventArgs e)
        {
            //Заполнение обязательных полей кроме автоинкрементируемых
            request.RequestComment = textBoxComment.Text;
            request.RequestDate = Convert.ToDateTime(textBoxDate.Text);
            request.RequestDescription = textBoxDescription.Text;
            request.RequestEquipmentId = (int)comboBoxEquipment.SelectedValue;
            request.RequestUserId = (int)comboBoxClient.SelectedValue;
            request.RequestMasterId = (int)comboBoxMaster.SelectedValue;
            request.RequestProblemId = (int)comboBoxProblem.SelectedValue;
            request.RequestComment = textBoxComment.Text;
            request.RequestPriorityId = (int)comboBoxStatus.SelectedValue;
            request.RequestStageId = (int)comboBoxStage.SelectedValue;
            request.RequestStatusId = (int)comboBoxStatus.SelectedValue;
            request.RequestTime = Convert.ToInt32(textBoxTime.Text);
            if (mode == 0)
            {
                ////////////Заполнение обязательных полей кроме автоинкрементируемых
                ////////////request.RequestComment = textBoxComment.Text;
                //////////request.RequestDate = Convert.ToDateTime(textBoxDate.Text);
                ////////////request.RequestDescription = textBoxDescription.Text;
                //////////request.RequestEquipmentId = (int)comboBoxEquipment.SelectedValue;
                //////////request.RequestUserId = (int)comboBoxClient.SelectedValue;
                //////////request.RequestMasterId = (int)comboBoxMaster.SelectedValue;
                //////////request.RequestProblemId = (int)comboBoxProblem.SelectedValue;
                ////////////request.RequestComment = textBoxComment.Text;
                //////////request.RequestPriorityId = (int)comboBoxStatus.SelectedValue;
                //////////request.RequestStageId = (int)comboBoxStage.SelectedValue;
                //////////request.RequestStatusId = (int)comboBoxStatus.SelectedValue;
                ////////////request.RequestTime = Convert.ToInt32(textBoxTime.Text);
                //Добавление объекта к списку заявок
                Helper.DB.Request.Add(request);
            }
            try
            {
                //Обновление БД
                Helper.DB.SaveChanges();
                MessageBox.Show("Данные успешно обновлены");
            }
            catch
            {
                MessageBox.Show("При обновлении возникла ошибка");
                return;
            }
        }

        private void Request_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "dBMatisikRequestsDataSet.Equipment". При необходимости она может быть перемещена или удалена.
            this.equipmentTableAdapter.Fill(this.dBMatisikRequestsDataSet.Equipment);

        }
    }
}
