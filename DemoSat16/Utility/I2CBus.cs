using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace DemoSat16.Utility {
    public class I2CBus : IDisposable
    {
        //Singleton OOP Model
        private static I2CBus _instance;
        public static I2CBus Instance => _instance ?? (_instance = new I2CBus());
        private I2CBus() {}

        //a reference to the current device we're talking to
        private static readonly I2CDevice _slave = new I2CDevice(new I2CDevice.Configuration(0, 0));

        //for thread synchronization
        private static readonly object locker = new object();



        internal void Write(I2CDevice.Configuration config, byte[] writeBuffer, int transactionTimeout = 1000)
        {
            int transferred = 0;

            _slave.Config = config;

            I2CDevice.I2CTransaction[] writeXAction = {
                I2CDevice.CreateWriteTransaction(writeBuffer)
            };

            lock (locker)
            {
                transferred = _slave.Execute(writeXAction, transactionTimeout);
            }

            if (transferred != writeBuffer.Length)
                Debug.Print("Possible I2C write failure... transferred: " + transferred + " .. buffer: " + writeBuffer.Length);
        }
        public void Write(I2CDevice.Configuration i2CConfig, byte byteToWrite, int transactionTimeout = 1000)
        {
            Debug.Print("About to write to " + (byte)i2CConfig.Address);
            Write(i2CConfig, new[] { byteToWrite }, transactionTimeout);
        }
        internal void Read(I2CDevice.Configuration config, byte[] readBuffer, int transactionTimeout = 1000)
        {
            _slave.Config = config;
            var transferred = 0;
            I2CDevice.I2CTransaction[] readXAction = {
                I2CDevice.CreateReadTransaction(readBuffer)
            };

            lock (_slave)
            {
                transferred = _slave.Execute(readXAction, transactionTimeout);
            }
            if (transferred != readBuffer.Length)
                Debug.Print("Possible I2C read failure... transferred: " + transferred + " .. buffer: " + readBuffer.Length);
        }

        public void WriteAndRead(I2CDevice.Configuration config, byte[] writeBuffer, ref byte[] readBuffer,
            int transactionTimeout = 2000)
        {
            //var test_config = new I2CDevice.Configuration(0x7F, 100);
            _slave.Config = config;
            I2CDevice.I2CTransaction[] i2cTxRx = {
                    I2CDevice.CreateWriteTransaction(writeBuffer),
                    I2CDevice.CreateReadTransaction(readBuffer),
                };
            lock (_slave)
            {
                var transferred = _slave.Execute(i2cTxRx, transactionTimeout);

            }
        }

        public void ReadRegister(I2CDevice.Configuration config, byte register, byte[] readBuffer, int transactionTimeout)
        {
            byte[] registerBuffer = { register };
            Write(config, registerBuffer, transactionTimeout);
            Read(config, readBuffer, transactionTimeout);
        }
        public void WriteRegister(I2CDevice.Configuration config, byte register, byte[] writeBuffer, int transactionTimeout)
        {
            byte[] registerBuffer = { register };
            Write(config, registerBuffer, transactionTimeout);
            Write(config, writeBuffer, transactionTimeout);
        }
        public void WriteRegister(I2CDevice.Configuration config, byte register, byte value, int transactionTimeout)
        {
            byte[] writeBuffer = { register, value };
            Write(config, writeBuffer, transactionTimeout);
        }


        //IDisposable
        public void Dispose()
        {
            lock (locker)
                _slave.Dispose();
        }
    }
}