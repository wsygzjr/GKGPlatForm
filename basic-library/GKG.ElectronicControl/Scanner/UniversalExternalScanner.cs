using System.Text;
using System.Text.Json;

namespace GKG
{
    namespace ElectronicControl
    {
        namespace General
        {
            public class UniversalExternalScanner : IBaseScanner
            {
                public void InitScanner(byte[] initCfg)
                {
                    serialPortCommunicate.Init(initCfg);
                    serialPortCommunicate.Open(500);
                }

                public string Scan()
                {
                    serialPortCommunicate.Write("ION");
                    byte[] readBytes = null;
                    string codeRet = "";
                    bool bRtn = serialPortCommunicate.ReadTimeout(2000, out readBytes);
                    if (bRtn)
                    {
                        if (readBytes.Length > 0)
                        {
                            string strBytes = Encoding.ASCII.GetString(readBytes);
                            bool bFindFirstFlag = strBytes.Contains("@");
                            bool bFindSecondFlag = strBytes.Contains("$");
                            if (bFindFirstFlag && bFindSecondFlag)
                            {
                                int startIdx = strBytes.IndexOf("@") + 1;
                                int length = strBytes.IndexOf("$") - startIdx;
                                codeRet = strBytes.Substring(startIdx, length);
                                if (codeRet.Length > 0)
                                {
                                    if (codeRet.Contains("ERROR"))
                                    {
                                        throw new Exception();
                                    }
                                }
                                else
                                {
                                    throw new Exception();
                                }
                            }
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                    else
                    {
                        throw new Exception();
                    }
                    return codeRet;
                }
                private SerialPortCommunicate serialPortCommunicate = new SerialPortCommunicate();
            }
        }
    }
}