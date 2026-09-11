using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace MVCDIS.Models
{
    public class TrialMaker
    {
        private string _BaseString;
        private string _Password;
        private string _SoftName;
        private string _RegFilePath;
        private string _HideFilePath;
        private int _DefDays;
        private int _Runed;
        private string _Text;
        private string _Identifier;



        public TrialMaker(string SoftwareName,
           string RegFilePath, string HideFilePath,
           string Text, int TrialDays, int TrialRunTimes,
           string Identifier)
        {
            _SoftName = SoftwareName;
            _Identifier = Identifier;

            SetDefaults();

            _DefDays = TrialDays;
            _Runed = TrialRunTimes;

            _RegFilePath = RegFilePath;
            _HideFilePath = HideFilePath;
            _Text = Text;
        }


        private void SetDefaults()
        {
            SystemInfo.UseBaseBoardManufacturer = false;
            SystemInfo.UseBaseBoardProduct = true;
            SystemInfo.UseBiosManufacturer = false;
            SystemInfo.UseBiosVersion = true;
            SystemInfo.UseDiskDriveSignature = true;
            SystemInfo.UsePhysicalMediaSerialNumber = false;
            SystemInfo.UseProcessorID = true;
            SystemInfo.UseVideoControllerCaption = false;
            SystemInfo.UseWindowsSerialNumber = true;

            MakeBaseString();
            MakePassword();
        }
        private void MakeBaseString()
        {
            _BaseString = Encryption.Boring(Encryption.InverseByBase(SystemInfo.GetSystemInfo(_SoftName), 10));
        }

        private void MakePassword()
        {
            _Password = Encryption.MakePassword(_BaseString, _Identifier);
        }

        private void MakeRegFile()
        {
            FileReadWrite.WriteFile(_RegFilePath, _Password);
        }


        private bool CheckRegister()
        {
            string Password = FileReadWrite.ReadFile(_RegFilePath);

            if (_Password == Password)
                return true;
            else
                return false;
        }

        private int DaysToEnd()
        {
            FileInfo hf = new FileInfo(_HideFilePath);
            if (hf.Exists == false)
            {
                MakeHideFile();
                return _DefDays;
            }
            return CheckHideFile();
        }

        private void MakeHideFile()
        {
            string HideInfo;
            HideInfo = DateTime.Now.Ticks + ";";
            HideInfo += _DefDays + ";" + _Runed + ";" + _BaseString;
            FileReadWrite.WriteFile(_HideFilePath, HideInfo);
        }

        private int CheckHideFile()
        {
            string[] HideInfo;
            HideInfo = FileReadWrite.ReadFile(_HideFilePath).Split(';');
            long DiffDays;
            int DaysToEnd;

            if (_BaseString == HideInfo[3])
            {
                DaysToEnd = Convert.ToInt32(HideInfo[1]);
                if (DaysToEnd <= 0)
                {
                    _Runed = 0;
                    _DefDays = 0;
                    return 0;
                }
                DateTime dt = new DateTime(Convert.ToInt64(HideInfo[0]));
                DiffDays = DateAndTime.DateDiff(DateInterval.Day,
                    dt.Date, DateTime.Now.Date,
                    FirstDayOfWeek.Saturday,
                    FirstWeekOfYear.FirstFullWeek);

                DaysToEnd = Convert.ToInt32(HideInfo[1]);
                _Runed = Convert.ToInt32(HideInfo[2]);
                _Runed -= 1;

                DiffDays = Math.Abs(DiffDays);

                _DefDays = DaysToEnd - Convert.ToInt32(DiffDays);
            }
            return _DefDays;
        }

        public RunTypes test()
        {

            if (CheckRegister() == true)
            {
                return RunTypes.Full;
            }
            else return RunTypes.Expired;
        }

        public String GetMashineCode()
        {
            MakeHideFile();
            return _BaseString;
        }

        public String GetPassword()
        {
            return _Password;
        }

        public bool GetAllResult(bool flag)
        {
            if (flag)
            {
                MakeRegFile();
            }
            return flag;
        }


        public enum RunTypes
        {
            Trial = 0,
            Full,
            Expired,
            UnKnown
        }




        public string RegFilePath
        {
            get
            {
                return _RegFilePath;
            }
            set
            {
                _RegFilePath = value;
            }
        }

        public string HideFilePath
        {
            get
            {
                return _HideFilePath;
            }
            set
            {
                _HideFilePath = value;
            }
        }


        public int TrialPeriodDays
        {
            get
            {
                return _DefDays;
            }
        }


        public byte[] TripleDESKey
        {
            get
            {
                return FileReadWrite.key;
            }
            set
            {
                FileReadWrite.key = value;
            }
        }




        public bool UseProcessorID
        {
            get
            {
                return SystemInfo.UseProcessorID;
            }
            set
            {
                SystemInfo.UseProcessorID = value;
            }
        }

        public bool UseBaseBoardProduct
        {
            get
            {
                return SystemInfo.UseBaseBoardProduct;
            }
            set
            {
                SystemInfo.UseBaseBoardProduct = value;
            }
        }

        public bool UseBaseBoardManufacturer
        {
            get
            {
                return SystemInfo.UseBiosManufacturer;
            }
            set
            {
                SystemInfo.UseBiosManufacturer = value;
            }
        }

        public bool UseDiskDriveSignature
        {
            get
            {
                return SystemInfo.UseDiskDriveSignature;
            }
            set
            {
                SystemInfo.UseDiskDriveSignature = value;
            }
        }

        public bool UseVideoControllerCaption
        {
            get
            {
                return SystemInfo.UseVideoControllerCaption;
            }
            set
            {
                SystemInfo.UseVideoControllerCaption = value;
            }
        }

        public bool UsePhysicalMediaSerialNumber
        {
            get
            {
                return SystemInfo.UsePhysicalMediaSerialNumber;
            }
            set
            {
                SystemInfo.UsePhysicalMediaSerialNumber = value;
            }
        }

        public bool UseBiosVersion
        {
            get
            {
                return SystemInfo.UseBiosVersion;
            }
            set
            {
                SystemInfo.UseBiosVersion = value;
            }
        }

        public bool UseBiosManufacturer
        {
            get
            {
                return SystemInfo.UseBiosManufacturer;
            }
            set
            {
                SystemInfo.UseBiosManufacturer = value;
            }
        }

        public bool UseWindowsSerialNumber
        {
            get
            {
                return SystemInfo.UseWindowsSerialNumber;
            }
            set
            {
                SystemInfo.UseWindowsSerialNumber = value;
            }
        }



    }
}