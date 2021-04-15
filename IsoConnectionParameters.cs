using System;
using System.Collections.Specialized;
using System.Text;

namespace lib61850net
{
    internal class IsoConnectionParameters
    {
        internal IsoAcse.AcseAuthenticationParameter acseAuthParameter;

        internal string hostname;
        internal int port;

        internal byte[] remoteApTitle = new byte[10];
        internal string remoteApTitleS = "";
        internal int remoteApTitleLen;
        internal int remoteAEQualifier;
        internal uint remotePSelector;
        internal ushort remoteSSelector;
        internal IsoCotp.TSelector remoteTSelector;

        internal byte[] localApTitle = new byte[10];
        internal string localApTitleS = "";
        internal int localApTitleLen;
        internal int localAEQualifier;
        internal uint localPSelector;
        internal ushort localSSelector;
        internal IsoCotp.TSelector localTSelector;

        internal IsoConnectionParameters(string host, int port)
        {
            Init(null, host, port);
        }

        internal IsoConnectionParameters(IsoAcse.AcseAuthenticationParameter acseAuthPar)
        {
            Init(acseAuthPar);
        }

        void Init(IsoAcse.AcseAuthenticationParameter acseAuthPar, string host = "localhost", int port = 102)
        {
            // Defaults
            hostname = host;
            this.port = port;

            IsoCotp.TSelector selector1 = new IsoCotp.TSelector(2, 0);
            IsoCotp.TSelector selector2 = new IsoCotp.TSelector(2, 1);
            setLocalAddresses(1, 1, selector1);
            setLocalApTitle("1.1.1.999", 12);
            setRemoteAddresses(1, 1, selector2);
            setRemoteApTitle("1.1.1.999.1", 12);
            acseAuthParameter = acseAuthPar;
        }

        internal IsoConnectionParameters(StringDictionary stringDictionary)
        {
            Init(null);
            int remoteTSelectorVal = remoteTSelector.value;
            int remoteTSelectorSize = remoteTSelector.value;
            foreach (string key in stringDictionary.Keys)
            {
                switch (key.ToLower())
                {
                    case "hostname":
                        hostname = stringDictionary[key];
                        break;
                    case "port":
                        int.TryParse(stringDictionary[key], out port);
                        break;
                    case "remoteaptitle":
                        remoteApTitleS = stringDictionary[key];
                        remoteApTitleLen = IsoUtil.BerEncoder_encodeOIDToBuffer(remoteApTitleS, remoteApTitle, 10);
                        break;
                    case "remoteaequalifier":
                        int.TryParse(stringDictionary[key], out remoteAEQualifier);
                        break;
                    case "remotepselector":
                        uint.TryParse(stringDictionary[key], out remotePSelector);
                        break;
                    case "remotesselector":
                        ushort.TryParse(stringDictionary[key], out remoteSSelector);
                        break;
                    case "remotetselectorvalue":
                        int.TryParse(stringDictionary[key], out remoteTSelector.value);
                        break;
                    case "remotetselectorsize":
                        byte.TryParse(stringDictionary[key], out remoteTSelector.size);
                        break;
                    case "localaptitle":
                        localApTitleS = stringDictionary[key];
                        localApTitleLen = IsoUtil.BerEncoder_encodeOIDToBuffer(localApTitleS, localApTitle, 10);
                        break;
                    case "localaequalifier":
                        int.TryParse(stringDictionary[key], out localAEQualifier);
                        break;
                    case "localpselector":
                        uint.TryParse(stringDictionary[key], out localPSelector);
                        break;
                    case "localsselector":
                        ushort.TryParse(stringDictionary[key], out localSSelector);
                        break;
                    case "localtselectorvalue":
                        int.TryParse(stringDictionary[key], out localTSelector.value);
                        break;
                    case "localtselectorsize":
                        byte.TryParse(stringDictionary[key], out localTSelector.size);
                        break;
                    case "authenticationmechanism":
                        if (acseAuthParameter == null) acseAuthParameter = new IsoAcse.AcseAuthenticationParameter();
                        Enum.TryParse<IsoAcse.AcseAuthenticationMechanism>(stringDictionary[key], out acseAuthParameter.mechanism);
                        break;
                    case "authenticationpassword":
                        if (acseAuthParameter == null) acseAuthParameter = new IsoAcse.AcseAuthenticationParameter();
                        acseAuthParameter.password = stringDictionary[key];
                        acseAuthParameter.paswordOctetString = Encoding.ASCII.GetBytes(acseAuthParameter.password);
                        acseAuthParameter.passwordLength = acseAuthParameter.paswordOctetString.Length;
                        break;
                }
            }
        }

        internal void Save(StringDictionary stringDictionary)
        {
            if (stringDictionary == null) stringDictionary = new StringDictionary();
            stringDictionary.Clear();
            stringDictionary.Add("hostname", hostname);
            stringDictionary.Add("port", port.ToString());
            stringDictionary.Add("remoteApTitle", remoteApTitleS);
            stringDictionary.Add("remoteAEQualifier", remoteAEQualifier.ToString());
            stringDictionary.Add("remotePSelector", remotePSelector.ToString());
            stringDictionary.Add("remoteSSelector", remoteSSelector.ToString());
            stringDictionary.Add("remoteTSelectorValue", remoteTSelector.value.ToString());
            stringDictionary.Add("remoteTSelectorSize", remoteTSelector.size.ToString());
            stringDictionary.Add("localApTitle", localApTitleS);
            stringDictionary.Add("localAEQualifier", localAEQualifier.ToString());
            stringDictionary.Add("localPSelector", localPSelector.ToString());
            stringDictionary.Add("localSSelector", localSSelector.ToString());
            stringDictionary.Add("localTSelectorValue", localTSelector.value.ToString());
            stringDictionary.Add("localTSelectorSize", localTSelector.size.ToString());
            if (acseAuthParameter != null)
            {
                stringDictionary.Add("authenticationMechanism", acseAuthParameter.mechanism.ToString());
                if (acseAuthParameter.mechanism == IsoAcse.AcseAuthenticationMechanism.ACSE_AUTH_PASSWORD)
                    stringDictionary.Add("authenticationPassword", acseAuthParameter.password);
            }
        }

        internal void setLocalAddresses(uint pSelector, ushort sSelector, IsoCotp.TSelector tSelector)
        {
            localPSelector = pSelector;
            localSSelector = sSelector;
            localTSelector = tSelector;
        }
        internal void setLocalApTitle(string ApTitle, int AEQualifier)
        {
            localApTitleS = ApTitle;
            localApTitleLen = IsoUtil.BerEncoder_encodeOIDToBuffer(ApTitle, localApTitle, 10);
            localAEQualifier = AEQualifier;
        }

        internal void setRemoteAddresses(uint pSelector, ushort sSelector, IsoCotp.TSelector tSelector)
        {
            remotePSelector = pSelector;
            remoteSSelector = sSelector;
            remoteTSelector = tSelector;
        }

        internal void setRemoteApTitle(string ApTitle, int AEQualifier)
        {
            remoteApTitleS = ApTitle;
            remoteApTitleLen = IsoUtil.BerEncoder_encodeOIDToBuffer(ApTitle, remoteApTitle, 10);
            remoteAEQualifier = AEQualifier;
        }

        internal string getRemoteApTitle()
        {
            return remoteApTitleS;
        }

        internal string geLocalApTitle()
        {
            return localApTitleS;
        }

    }
}
