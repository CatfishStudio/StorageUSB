/*
 * Сделано в SharpDevelop.
 * Пользователь: Catfish
 * Дата: 16.04.2014
 * Время: 9:47
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using USBClassLibrary;

namespace StorageUSB
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		/* Объявите экземпляр USBClass. */
		private USBClassLibrary.USBClass USBPort;
		/* Объявите экземпляр класса DeviceProperties, если вы хотите, чтобы прочитать свойства ваших устройств. */
		private USBClassLibrary.USBClass.DeviceProperties USBDeviceProperties;
		/* Флаг подключения устройства */
		private bool MyUSBDeviceConnected;
		
		private const uint MyDeviceVID = 0X04D8; //Microchip ICD2 VID
        private const uint MyDevicePID = 0X8001; //Microchip ICD2 PID
        
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			// Создайте экземпляр класса USBClass.
			USBPort = new USBClass();
			// Создайте экземпляр класса DeviceProperties.
			USBDeviceProperties = new USBClass.DeviceProperties(); 
			
			// Добавьте обработчики для событий, предоставляемых классом USBClass.
			USBPort.USBDeviceAttached += new USBClass.USBDeviceEventHandler(USBPort_USBDeviceAttached);
			USBPort.USBDeviceRemoved += new USBClass.USBDeviceEventHandler(USBPort_USBDeviceRemoved);
			
			// Зарегистрируйте форму для приема сообщений Windows, когда устройства добавляются или удаляются.
			USBPort.RegisterForDeviceChange(true, this.Handle);
			
			// Затем проверьте, если ваше устройство не подключено:
			if (USBClass.GetUSBDevice(MyDeviceVID, MyDevicePID, ref USBDeviceProperties, false))
			{
   				// Мое устройство подключено
   				MyUSBDeviceConnected = true;
			}
		}
		
		
		
		
		void MainFormLoad(object sender, EventArgs e)
		{
			
			
		}
		
		/* Реализация Присоединение и отсоединение обработчиков: */
		private void USBPort_USBDeviceAttached(object sender, USBClass.USBDeviceEventArgs e)
		{
			label1.Text = "Устройство подключено!";
   			if (!MyUSBDeviceConnected)
   			{
      			if (USBClass.GetUSBDevice(MyDeviceVID, MyDevicePID, ref USBDeviceProperties, false))
      			{
        			// Мое устройство подключено
        			MyUSBDeviceConnected = true;
         		}
   			}
		}

		private void USBPort_USBDeviceRemoved(object sender, USBClass.USBDeviceEventArgs e)
		{
			label1.Text = "Устройство отключено!";
      		if (!USBClass.GetUSBDevice(MyDeviceVID, MyDevicePID, ref USBDeviceProperties, false))
   			{
      			// Мое устройство отключено
      			MyUSBDeviceConnected = false;
      		}
		}
		
		protected override void WndProc(ref Message m)
		{
   			bool IsHandled = false;

   			USBPort.ProcessWindowsMessage(m.Msg, m.WParam, m.LParam, ref IsHandled);

   			base.WndProc(ref m);
		}
		
	}
}
