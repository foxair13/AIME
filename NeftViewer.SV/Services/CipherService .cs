using System.Security.Cryptography;
using Hardware.Info;
namespace NeftViewer.SV.Services
{
    public class CipherService : List<TapeService>
    {
        static readonly IHardwareInfo hardwareInfo = new HardwareInfo();
        public CipherService()
        {
            Add(new TapeService("A0B1C2D3E4F5G6H7I8J9KLMNOPQRSTUVWXYZ"));
        }

        public string Encode()
        {

            String proc = "";
            hardwareInfo.RefreshAll();
            foreach (var cpu in hardwareInfo.CpuList)
            {
                proc = cpu.ProcessorId;

            }
            return proc;
        }
        
        public string Codeс(string symbol, int key)
        {
            string res = "", tmp = "";
            for (int i = 0; i < symbol.Length; i++)
            {
                foreach (TapeService v in this)
                {
                    tmp = v.Replacement(symbol.Substring(i, 1), key);
                    if (tmp != "")
                    {
                        res += tmp;
                        break;
                    }
                }
                if (tmp == "") res += symbol.Substring(i, 1);
            }
            return res;
        }
       
    }

}