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
using MadWizard.WinUSBNet;

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
		
		// [HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\AutoplayHandlers\KnownDevices]
		// USB\VID_05AC&PID_129E&REV_0410 (iPod4)
		private const uint MyDeviceVID = 0X05AC; //добавлено 0Х...
        private const uint MyDevicePID = 0X129E; //добавлено 0Х...
        
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
			/*
			if (USBClass.GetUSBDevice(MyDeviceVID, MyDevicePID, ref USBDeviceProperties, false))
			{
   				// Мое устройство подключено
   				MyUSBDeviceConnected = true;
			}
			*/
		}
		
		
		
		
		void MainFormLoad(object sender, EventArgs e)
		{
			
		}
		
		/* Реализация присоединение и отсоединение устройств: */
		private void USBPort_USBDeviceAttached(object sender, USBClass.USBDeviceEventArgs e)
		{
			this.Visible = true;
			label1.Text = "Подключено устройство к USB порту! " + e.Cancel.ToString()  + System.Environment.NewLine + "В данное время устройство недоступно, сделать его доступным?";
			if (USBClass.GetUSBDevice(MyDeviceVID, MyDevicePID, ref USBDeviceProperties, false))
			{
				MessageBox.Show(USBDeviceProperties.FriendlyName.ToString());
				
			}
			
   		}

		private void USBPort_USBDeviceRemoved(object sender, USBClass.USBDeviceEventArgs e)
		{
			label1.Text = "USB Устройство отключено!";
		}
		
		protected override void WndProc(ref Message m)
		{
   			bool IsHandled = false;

   			USBPort.ProcessWindowsMessage(m.Msg, m.WParam, m.LParam, ref IsHandled);

   			base.WndProc(ref m);
		}
		
		
		void MainFormShown(object sender, EventArgs e)
		{
			this.Visible = false;
		}
		
		void ЗакрытьПрограммуToolStripMenuItemClick(object sender, EventArgs e)
		{
			Application.Exit();
		}
		
		void Button1Click(object sender, EventArgs e)
		{
			
		}
	}
}
