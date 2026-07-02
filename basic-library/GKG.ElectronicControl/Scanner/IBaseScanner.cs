namespace GKG
{
    namespace ElectronicControl
    {
        namespace General
        {
            public interface IBaseScanner
            {
                void InitScanner(byte[] initCfg);

                string Scan();
            }
        }
    }
}