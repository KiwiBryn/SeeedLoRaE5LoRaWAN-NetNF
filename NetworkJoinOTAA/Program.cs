//---------------------------------------------------------------------------------
// Copyright (c) May 2021, devMobile Software
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
//---------------------------------------------------------------------------------
// nanoff --target ST_STM32F769I_DISCOVERY --update
namespace devMobile.IoT.SeeedLoRaE5.NetworkJoinOTAA
{
   using System;
   using System.Diagnostics;
   using System.Threading;

   using Windows.Devices.SerialCommunication;
   using Windows.Storage.Streams;

   public class Program
   {
      private const string SerialPortId = "COM6";

      private const string AppKey = "................................";
      private const string AppEui = "................";

      private const byte MessagePort = 15;

      //private const string Payload = "48656c6c6f204c6f526157414e"; // Hello LoRaWAN
      private const string Payload = "01020304"; // AQIDBA==
      //private const string Payload = "04030201"; // BAMCAQ==

      public static void Main()
      {
         SerialDevice serialDevice;
         uint bytesWritten;
         uint txByteCount;
         uint bytesRead;

         Debug.WriteLine("devMobile.IoT.SeeedLoRaE5.NetworkJoinOTAA starting");

         Debug.WriteLine($"Ports available: {Windows.Devices.SerialCommunication.SerialDevice.GetDeviceSelector()}");

         try
         {
            serialDevice = SerialDevice.FromId(SerialPortId);

            // set parameters
            serialDevice.BaudRate = 9600;
            serialDevice.Parity = SerialParity.None;
            serialDevice.StopBits = SerialStopBitCount.One;
            serialDevice.Handshake = SerialHandshake.None;
            serialDevice.DataBits = 8;

            serialDevice.ReadTimeout = new TimeSpan(0, 0, 5);
            serialDevice.WriteTimeout = new TimeSpan(0, 0, 4);

            DataWriter outputDataWriter = new DataWriter(serialDevice.OutputStream);
            DataReader inputDataReader = new DataReader(serialDevice.InputStream);

            // set a watch char to be notified when it's available in the input stream
            serialDevice.WatchChar = '\n';

            // clear out the RX buffer
            bytesRead = inputDataReader.Load(128);
            while (bytesRead > 0)
            {
               string response = inputDataReader.ReadString(bytesRead);
               Debug.WriteLine($"RX :{response}");

               bytesRead = inputDataReader.Load(128);
            }

            // Set the Region to AS923
            bytesWritten = outputDataWriter.WriteString("AT+DR=AS923\r\n");
            Debug.WriteLine($"TX: region {outputDataWriter.UnstoredBufferLength} bytes to output stream.");
            txByteCount = outputDataWriter.Store();
            Debug.WriteLine($"TX: {txByteCount} bytes via {serialDevice.PortName}");

            // Read the response
            bytesRead = inputDataReader.Load(128);
            if (bytesRead > 0)
            {
               String response = inputDataReader.ReadString(bytesRead);
               Debug.WriteLine($"RX :{response}");
            }

            // Set the Join mode
            bytesWritten = outputDataWriter.WriteString("AT+MODE=LWOTAA\r\n");
            Debug.WriteLine($"TX: mode {outputDataWriter.UnstoredBufferLength} bytes to output stream.");
            txByteCount = outputDataWriter.Store();
            Debug.WriteLine($"TX: {txByteCount} bytes via {serialDevice.PortName}");

            // Read the response
            bytesRead = inputDataReader.Load(128);
            if (bytesRead > 0)
            {
               string response = inputDataReader.ReadString(bytesRead);
               Debug.WriteLine($"RX :{response}");
            }

            // Set the appEUI
            bytesWritten = outputDataWriter.WriteString($"AT+ID=AppEui,\"{AppEui}\"\r\n");
            Debug.WriteLine($"TX: AppEui {outputDataWriter.UnstoredBufferLength} bytes to output stream.");
            txByteCount = outputDataWriter.Store();
            Debug.WriteLine($"TX: {txByteCount} bytes via {serialDevice.PortName}");

            // Read the response
            bytesRead = inputDataReader.Load(128);
            if (bytesRead > 0)
            {
               String response = inputDataReader.ReadString(bytesRead);
               Debug.WriteLine($"RX :{response}");
            }

            // Set the appKey
            bytesWritten = outputDataWriter.WriteString($"AT+KEY=APPKEY,{AppKey}\r\n");
            Debug.WriteLine($"TX: AppKey {outputDataWriter.UnstoredBufferLength} bytes to output stream.");
            txByteCount = outputDataWriter.Store();
            Debug.WriteLine($"TX: {txByteCount} bytes via {serialDevice.PortName}");

            // Read the response
            bytesRead = inputDataReader.Load(128);
            if (bytesRead > 0)
            {
               String response = inputDataReader.ReadString(bytesRead);
               Debug.WriteLine($"RX :{response}");
            }

            // Set the port number
            bytesWritten = outputDataWriter.WriteString($"AT+PORT={MessagePort}\r\n");
            Debug.WriteLine($"TX: port {outputDataWriter.UnstoredBufferLength} bytes to output stream.");
            txByteCount = outputDataWriter.Store();
            Debug.WriteLine($"TX: {txByteCount} bytes via {serialDevice.PortName}");

            // Read the response
            bytesRead = inputDataReader.Load(128);
            if (bytesRead > 0)
            {
               String response = inputDataReader.ReadString(bytesRead);
               Debug.WriteLine($"RX :{response}");
            }

            // Join the network
            bytesWritten = outputDataWriter.WriteString("AT+JOIN\r\n");
            Debug.WriteLine($"TX: join {outputDataWriter.UnstoredBufferLength} bytes to output stream.");
            txByteCount = outputDataWriter.Store();
            Debug.WriteLine($"TX: {txByteCount} bytes via {serialDevice.PortName}");

            // Read the response, need loop as multi line response
            bytesRead = inputDataReader.Load(128);
            while (bytesRead > 0)
            {
               String response = inputDataReader.ReadString(bytesRead);
               Debug.WriteLine($"RX :{response}");

               bytesRead = inputDataReader.Load(128);
            }

            while (true)
            {
               bytesWritten = outputDataWriter.WriteString($"AT+MSGHEX=\"{Payload}\"\r\n");
               Debug.WriteLine($"TX: send {outputDataWriter.UnstoredBufferLength} bytes to output stream.");

               txByteCount = outputDataWriter.Store();
               Debug.WriteLine($"TX: {txByteCount} bytes via {serialDevice.PortName}");

               // Read the response, need loop as multi line response
               bytesRead = inputDataReader.Load(128);
               while (bytesRead > 0)
               {
                  String response = inputDataReader.ReadString(bytesRead);
                  Debug.WriteLine($"RX :{response}");

                  bytesRead = inputDataReader.Load(128);
               }

               Thread.Sleep(300000);
            }
         }
         catch (Exception ex)
         {
            Debug.WriteLine(ex.Message);
         }
      }
   }
}