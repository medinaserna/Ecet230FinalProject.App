
using System.IO.Ports;
using System.Text;

namespace Ecet230FinalProject;

public partial class MainPage : ContentPage
{
    private bool bPortOpen = false;
    private string newPacket = "";
    private int oldPacketNumber = -1;
    private int newPacketNumber = 0;
    private int lostPacketCount = 0;
    private int packetRollover = 0;
    private int chkSumError = 0;


    StringBuilder stringBuilderSend = new StringBuilder("###1111196");

    SerialPort serialPort = new SerialPort();

   // SolarCalc solarCalc = new SolarCalc(); //instancia de la clase


    public MainPage()
    {
        InitializeComponent();

        string[] ports = SerialPort.GetPortNames();
        portPicker.ItemsSource = ports;
        portPicker.SelectedIndex = ports.Length;
        Loaded += MainPage_Loaded;
    }

    private void MainPage_Loaded(object sender, EventArgs e)
    {
        serialPort.BaudRate = 115200;
        serialPort.ReceivedBytesThreshold = 1;
        serialPort.DataReceived += SerialPort_DataReceived;
        setUpSerialPort();
    }

    private void setUpSerialPort()
    {

    }

    private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        newPacket = serialPort.ReadLine();
        //labelRXdata.Text = text;
        MainThread.BeginInvokeOnMainThread(MyMainThreadCode);
    }

    private void MyMainThreadCode()
    {
        //code to run on the main thread
        if (checkBoxHistory.IsChecked == true)
        {
            labelRxdata.Text = newPacket + labelRxdata.Text;
        }
        else
        {
            labelRxdata.Text = newPacket;
        }
        int calChkSum = 0;
        if (newPacket.Length > 37)
        {
            if (newPacket.Substring(0, 3) == "###")
            {
                newPacketNumber = Convert.ToInt32(newPacket.Substring(3, 3));


                if (oldPacketNumber > -1)
                {
                    if (newPacketNumber < oldPacketNumber)
                    {
                        packetRollover++;
                        if (oldPacketNumber != 999)
                        {
                            lostPacketCount += 999 - oldPacketNumber + newPacketNumber;
                        }
                    }
                    else
                    {
                        if (newPacketNumber != oldPacketNumber + 1)
                        {
                            lostPacketCount += newPacketNumber - oldPacketNumber - 1;
                        }
                    }
                }

                for (int i = 3; i < 34; i++)
                {
                    calChkSum += (byte)newPacket[i];
                }
                calChkSum %= 1000;
                int recChkSum = Convert.ToInt32(newPacket.Substring(34, 3));
                if (recChkSum == calChkSum)
                {
               //     DisplaySolarData(newPacket);
                    oldPacketNumber = newPacketNumber;
                }
                else
                {
                    chkSumError++;
                }

                string parsedData = $"{newPacket.Length,-14}" +
                     $"{newPacket.Substring(0, 3),-14}" +
                     $"{newPacket.Substring(3, 3),-14}" +
                     $"{newPacket.Substring(6, 4),-14}" +  //aqui arranca el Analog0
                     $"{newPacket.Substring(10, 4),-14}" +
                     $"{newPacket.Substring(14, 4),-14}" +
                     $"{newPacket.Substring(18, 4),-14}" +
                     $"{newPacket.Substring(22, 4),-14}" +
                     $"{newPacket.Substring(26, 4),-14}" +   //Aqui finaliza con el Analog5
                     $"{newPacket.Substring(30, 4),-14}" +
                     $"{newPacket.Substring(34, 3),-14}" +
                     $"{calChkSum,-14}" +
                     $"{lostPacketCount,-14}" +
                     $"{chkSumError,-14}" +
                     $"{packetRollover}\r\n";


                if (checkBoxParsedHistory.IsChecked == true)
                {
                    labelParsedData.Text = parsedData + labelParsedData.Text;
                }
                else
                {
                    labelParsedData.Text = parsedData;
                }

            }
        }
    }

  /*  private void DisplaySolarData(string validPacket)
    {
        solarCalc.ParseSolarData(validPacket);
        labelSolarVolt.Text = solarCalc.GetVoltage(solarCalc.analogVoltageArray[0]);
        labelBatteryVolt.Text = solarCalc.GetVoltage(solarCalc.analogVoltageArray[2]);
        labelBatteryCurrent.Text = solarCalc.GetCurrent(solarCalc.analogVoltageArray[1], solarCalc.analogVoltageArray[2]);
        labelLED1Current.Text = solarCalc.GetLEDCurrent(solarCalc.analogVoltageArray[1], solarCalc.analogVoltageArray[4]);
        labelLED2Current.Text = solarCalc.GetLEDCurrent(solarCalc.analogVoltageArray[1], solarCalc.analogVoltageArray[3]);
    }
  */
    private void btnOpenClose_Clicked(object sender, EventArgs e)
    {
        if (!bPortOpen)
        {

            serialPort.PortName = portPicker.SelectedItem.ToString();
            serialPort.Open();
            btnOpenClose.Text = "Close";
            bPortOpen = true;
        }
        else
        {
            serialPort.Close();
            btnOpenClose.Text = "Open";
            bPortOpen = false;

        }

    }

    private void btnClear_Clicked(object sender, EventArgs e)
    {

    }

    private void btnSend_Clicked(object sender, EventArgs e)
    {

        try
        {
            string messageOut = entrySend.Text;
            messageOut += "\r\n";
            byte[] messageBytes = Encoding.UTF8.GetBytes(messageOut);
            serialPort.Write(messageBytes, 0, messageBytes.Length);
        }
        catch (Exception ex)
        {
            DisplayAlert("Alert", ex.Message, "Ok");
        }
    }

    private void btnBit3_Clicked(object sender, EventArgs e)
    {
        ButtonClicked(3);
    }

    private void btnBit2_Clicked(object sender, EventArgs e)
    {
        ButtonClicked(2);
    }

    private void btnBit1_Clicked(object sender, EventArgs e)
    {
        ButtonClicked(1);
    }

    private void btnBit0_Clicked(object sender, EventArgs e)
    {
        ButtonClicked(0);
    }

    private void ButtonClicked(int i)
    {
        Button[] btnBit = new Button[] { btnBit0, btnBit1, btnBit2, btnBit3 };
        if (btnBit[i].Text == "0")
        {
            btnBit[i].Text = "1";
            stringBuilderSend[i + 3] = '1';

            switch (i)
            {
                case 0:
                    imgLED1.Source = "ledoff.png";
                    break;
                case 1:
                    imgLED2.Source = "ledoff.png";
                    break;
            }

        }
        else
        {
            btnBit[i].Text = "0";
            stringBuilderSend[i + 3] = '0';
            switch (i)
            {
                case 0:
                    imgLED1.Source = "ledon.png";
                    break;
                case 1:
                    imgLED2.Source = "ledon.png";
                    break;
            }


        }
        sendPacket();
    }

    private void sendPacket()
    {
        int calSendChkSum = 0;

        try
        {
            for (int i = 3; i < 7; i++)
            {
                calSendChkSum += (byte)stringBuilderSend[i];
            }
            calSendChkSum %= 1000;
            stringBuilderSend.Remove(7, 3);
            stringBuilderSend.Insert(7, calSendChkSum.ToString());
            string messageOut = stringBuilderSend.ToString();
            entrySend.Text = stringBuilderSend.ToString();
            messageOut += "\r\n";
            byte[] messageBytes = Encoding.UTF8.GetBytes(messageOut);
            serialPort.Write(messageBytes, 0, messageBytes.Length);
        }
        catch (Exception ex)
        {
            DisplayAlert("Alert", ex.Message, "Ok");
        }
    }

    private void imgLED1_Clicked(object sender, EventArgs e)
    {
        ButtonClicked(0);
    }

    private void imgLED2_Clicked(object sender, EventArgs e)
    {
        ButtonClicked(1);
    }

}