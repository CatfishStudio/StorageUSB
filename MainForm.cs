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
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.Win32;
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
		private bool DeviceConnect = false;
		
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			// Создайте экземпляр класса USBClass.
			USBPort = new USBClass();
			
			// Добавьте обработчики для событий, предоставляемых классом USBClass.
			USBPort.USBDeviceAttached += new USBClass.USBDeviceEventHandler(USBPort_USBDeviceAttached);
			USBPort.USBDeviceRemoved += new USBClass.USBDeviceEventHandler(USBPort_USBDeviceRemoved);
						
			// Зарегистрируйте форму для приема сообщений Windows, когда устройства добавляются или удаляются.
			USBPort.RegisterForDeviceChange(true, this.Handle);
			
			
		}
		
		private void USB_getStatus()
        {

            RegistryKey key;
            try
            {
                key = Registry.LocalMachine.OpenSubKey
                        ("SYSTEM\\CurrentControlSet\\Control\\StorageDevicePolicies");

                if (System.Convert.ToInt16(key.GetValue("WriteProtect", null)) == 1)
                	label1.Text = "USB порту только для чтения! ";
				else
                    label1.Text = "USB порту всё разрешено! ";
            }
            catch (NullReferenceException )
            {
                key = Registry.LocalMachine.OpenSubKey
                        ("SYSTEM\\CurrentControlSet\\Control", true);
                key.CreateSubKey("StorageDevicePolicies");
                key.Close();
            }
            catch (Exception) { }
            try
            {
                key = Registry.LocalMachine.OpenSubKey
                ("SYSTEM\\CurrentControlSet\\Services\\UsbStor");

                if (System.Convert.ToInt16(key.GetValue("Start", null)) == 4)
                {
                    label1.Text = "USB порту заблокирован! ";
                    return;
                }
            }
            catch (NullReferenceException )
            {
                key = Registry.LocalMachine.OpenSubKey
                ("SYSTEM\\CurrentControlSet\\Services", true);
                key.CreateSubKey("USBSTOR");

                key = Registry.LocalMachine.OpenSubKey
                ("SYSTEM\\CurrentControlSet\\Services\\UsbStor", true);

                key.SetValue("Type", 1, RegistryValueKind.DWord);
                key.SetValue("Start", 3, RegistryValueKind.DWord);
                key.SetValue
                    ("ImagePath", "system32\\drivers\\usbstor.sys", RegistryValueKind.ExpandString);
                key.SetValue("ErrorControl", 1, RegistryValueKind.DWord);
                key.SetValue
                    ("DisplayName", "USB Mass Storage Driver", RegistryValueKind.String);

                key.Close();
            }
            
           catch( Exception ) {}

        }
        
		
		/* БЛОКИРОВКА USB ПОРТА */
		private void USB_disableAllStorageDevices()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey
                ("SYSTEM\\CurrentControlSet\\Services\\UsbStor",true);
            if (key != null)
            {
                key.SetValue("Start", 4, RegistryValueKind.DWord);
            }
            key.Close();
        }
        
		/* РАЗБЛОКИРОВКА USB ПОРТА */
		private void USB_enableAllStorageDevices()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey
                ("SYSTEM\\CurrentControlSet\\Services\\UsbStor", true);
            if (key != null)
            {
                key.SetValue("Start", 3, RegistryValueKind.DWord);
            }
            key.Close();
        }
        
		
		void MainFormLoad(object sender, EventArgs e)
		{
			// Блокировка USB Порта
			USB_getStatus();
		}
		
		/* Реализация присоединение и отсоединение устройств: */
		private void USBPort_USBDeviceAttached(object sender, USBClass.USBDeviceEventArgs e)
		{
			if(!DeviceConnect){
				DeviceConnect = true;
				this.Visible = true;
				USB_getStatus();
				label1.Text = label1.Text + System.Environment.NewLine + "USB устройство подключено!";
				// блокировать USB порт
				USB_disableAllStorageDevices();
				USBPort_USBDeviceRemoved(null, new USBClass.USBDeviceEventArgs());
			}
   		}

		private void USBPort_USBDeviceRemoved(object sender, USBClass.USBDeviceEventArgs e)
		{
			//USB_enableAllStorageDevices();
			USB_getStatus();
			label1.Text = label1.Text + System.Environment.NewLine + "USB Устройство отключено!";
			DeviceConnect = false;
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
			// разблокировать USB устройство
			USB_enableAllStorageDevices();
			USB_getStatus();
			USBPort_USBDeviceRemoved(null, new USBClass.USBDeviceEventArgs());
			MessageBox.Show("USB устройство разблокированно!");
		}
		
		void Button2Click(object sender, EventArgs e)
		{
			// блокировать USB стройство
			USB_disableAllStorageDevices();
			USB_getStatus();
			USBPort_USBDeviceRemoved(null, new USBClass.USBDeviceEventArgs());
			MessageBox.Show("USB устройство заблокированно!");
		}
		
		void РазблокироватьUSBПортToolStripMenuItemClick(object sender, EventArgs e)
		{
			this.Visible = true;			
		}
	}
}
