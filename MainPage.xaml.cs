
using Microsoft.Graphics.Canvas.Effects;
using System.ComponentModel;
using System.IO.Ports;
using System.Text;
using System.Timers;

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
    private string stringXacc = "";
    private string stringYacc = "";
    private string stringZacc = "";
    private int accValueX = 0;
    private int accValueY = 0;
    private int accValueZ = 0;

    //Graph class code:
    public int Yaxis = 0;
    public double degrees = 0;
    public int count = 0;
    public int graphHeight = 500;




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

        //graph:
        var timer = new System.Timers.Timer(16);
        timer.Elapsed += new ElapsedEventHandler(DrawNewPointOnGraph);
        timer.Start();

    }

    private void DrawNewPointOnGraph(object sender, ElapsedEventArgs e)
    {
        var graphicsView = this.LineGraphView;
        var lineGraphDrawable = (LineDrawable)graphicsView.Drawable;

 

        lineGraphDrawable.lineGraphs[0].Yaxis = (int)(((-accValueX/10))+ (graphHeight / 2));
        lineGraphDrawable.lineGraphs[1].Yaxis = (int)((-accValueY/10)+(graphHeight / 2));
        lineGraphDrawable.lineGraphs[2].Yaxis = (int)((-accValueZ/10)+ (graphHeight / 2));
        lineGraphDrawable.lineGraphs[3].Yaxis = (int)( 0 + (graphHeight / 2));

        graphicsView.Invalidate();
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


                //in this part convert the hex unsigned to signed int32

                stringXacc = newPacket.Substring(6, 4);  //get the small string related to axis
                stringYacc = newPacket.Substring(10, 4);
                stringZacc = newPacket.Substring(14, 4);

                accValueX = Convert.ToInt16(stringXacc, 16);
                accValueY = Convert.ToInt16(stringYacc, 16);
                accValueZ = Convert.ToInt16(stringZacc, 16);


                /*

                accValueX = int.Parse(stringXacc, System.Globalization.NumberStyles.HexNumber);
                accValueY = int.Parse(stringYacc, System.Globalization.NumberStyles.HexNumber);
                accValueZ = int.Parse(stringZacc, System.Globalization.NumberStyles.HexNumber);

                if (accValueX > 32767) {
                    int difAccX = accValueX - 32768;
                    if (difAccX == 0) {
                        accValueX *= -1;
                    } else {
                        accValueX = -32768 + difAccX;
                      }
                    }

                if (accValueY > 32767)
                {
                    int difAccY = accValueY - 32768;
                    if (difAccY == 0)
                    {
                        accValueY *= -1;
                    }
                    else
                    {
                        accValueY = -32768 + difAccY;
                    }
                }


                if (accValueZ > 32767)
                {
                    int difAccZ = accValueZ - 32768;
                    if (difAccZ == 0)
                    {
                        accValueZ *= -1;
                    }
                    else
                    {
                        accValueZ = -32768 + difAccZ;
                    }
                }
                */


                labelXaxis.Text =Convert.ToString(accValueX); // waynes code
                labelYaxis.Text = Convert.ToString(accValueY);
                labelZaxis.Text = Convert.ToString(accValueZ);
                //labelXaxis.Text = string.Format("{0:N1}", accValueX);
                //labelYaxis.Text = string.Format("{0:N1}", accValueY);
                //labelZaxis.Text = string.Format("{0:N1}", accValueZ);



                // accValueX = Convert.ToInt32(newPacket.Substring(6, 4));
                //accValueY = Convert.ToInt32(newPacket.Substring(10, 4));
                // accValueZ = Convert.ToInt32(newPacket.Substring(14, 4));


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

 

}